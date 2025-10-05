using System;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Collections;

public class FightController : IContainer {
    private FightService _fightService;

    public void OnInit() {
    }

    public void OnServerCommand(BasePackage package) {
        switch (package.Code) {
            case NetDefine.CMD_CallLordCode:
                OnCallLordHandle(package);
                break;
            case NetDefine.CMD_GrabLordCode:
                OnGrabLordHandle(package);
                break;
            case NetDefine.CMD_RaiseCode:
                OnRaiseHandle(package);
                break;
            case NetDefine.CMD_PlayHandCode:
                OnPlayHandHandle(package);
                break;
        }
    }

    public FightController(FightService service) {
        _fightService = service;
    }

    /// <summary>
    /// 叫地主请求
    /// </summary>
    private void OnCallLordHandle(BasePackage package) {
        Session session = SessionMgr.Instance.GetSession(package.SessionId);
        Room room = Cache.Instance.UserRoomDict[session.UserId];
        if (room.RoomState != RoomState.CallLord) {
            return;
        }

        // 遍历房间内的玩家列表，找到请求叫地主的玩家
        Player callLordPlayer = room.Players.FirstOrDefault(player => player.Id == session.UserId);
        if (callLordPlayer == null) {
            return;
        }

        // 获取请求参数
        CallLordBo form = CallLordBo.Parser.ParseFrom(package.Data);
        room.CallTimes++;
        // 没人"叫地主"，重新发牌
        if (room.CallTimes == 3 && !form.IsCall) {
            room.CallTimes = 0;
            room.Players[0].CanGrab = true;
            room.Players[1].CanGrab = true;
            room.Players[2].CanGrab = true;
            // 重新发牌
            _fightService.StartDispatchCardHandle(room);
            return;
        }

        // 如果玩家选择“叫地主”，将其坐位索引记录下来
        if (form.IsCall) {
            room.CallPos = form.Pos;
            room.CurLordPos = form.Pos;
            if (room.CallTimes == 3) {
                // 只有最后一人“叫地主”，则直接成为地主进入加倍环节
                _fightService.BecomeLord(room);
            } else {
                // 进入抢地主环节
                room.RoomState = RoomState.GrabLord;
            }
        } else {
            // 玩家不叫地主，则无法参与抢地主
            callLordPlayer.CanGrab = false;
        }

        var response = new CallLordResponse {
            LastPos = form.Pos,
            IsCall = form.IsCall,
            CallPos = room.CallPos,
            CallTimes = room.CallTimes
        };

        // 将进行抢地主操作的玩家广播给房间内的玩家
        Broadcast(room.Players, package, response.ToByteString());
    }

    /// <summary>
    /// 抢地主请求
    /// </summary>
    private void OnGrabLordHandle(BasePackage package) {
        Session session = SessionMgr.Instance.GetSession(package.SessionId);
        Room room = Cache.Instance.UserRoomDict[session.UserId];
        if (room.RoomState != RoomState.GrabLord) {
            return;
        }

        // 遍历房间内的玩家列表，找到请求抢地主的玩家
        var grabLordPlayer = room.Players.FirstOrDefault(player => player.Id == session.UserId);
        if (grabLordPlayer == null) {
            return;
        }

        if (!grabLordPlayer.CanGrab) {
            return;
        }

        var form = GrabLordBo.Parser.ParseFrom(package.Data);
        // 用户选择“抢地主”，倍数加倍、当前地主索引变为该玩家
        if (form.IsGrab) {
            room.Multiple *= 2;
            room.GrabTimes++;
            room.CurLordPos = form.Pos;
        }

        var response = new GrabLordResponse {
            LastPos = form.Pos,
            IsGrab = form.IsGrab,
            Multiple = room.Multiple,
            GrabTimes = room.GrabTimes,
            CanGrabPos = -1
        };

        // 获取下个玩家的坐位
        int nextPlayerPos = form.Pos + 1;
        if (nextPlayerPos > 2) {
            nextPlayerPos = 0;
        }

        // 下个玩家有资格抢地主
        if (room.Players[nextPlayerPos].CanGrab) {
            /*
             * 玩家1：叫地主                                    CurLordPos = 0
             * 玩家2：不抢，玩家3：抢                            CurLordPos = 2
             * 玩家1：抢 -> 玩家1成为地主，不抢 -> 玩家3成为地主
             */
            if (room.CurLordPos != nextPlayerPos) {
                // 设置下一个可以抢地主的玩家的坐位
                response.CanGrabPos = nextPlayerPos;
                Broadcast(room.Players, package, response.ToByteString());
            } else {
                // 走到这里说明没人抢地主，“叫地主”的玩家可直接成为地主
                response.CanGrabPos = -1;
                Broadcast(room.Players, package, response.ToByteString());
                _fightService.BecomeLord(room);
            }
        } else {
            // 获取上个玩家的坐位
            int prevPlayerPos = nextPlayerPos + 1;
            if (prevPlayerPos > 2) {
                prevPlayerPos = 0;
            }

            // 上个玩家有资格抢地主
            if (room.Players[prevPlayerPos].CanGrab) {
                if (room.CurLordPos != prevPlayerPos) {
                    response.CanGrabPos = prevPlayerPos;
                    Broadcast(room.Players, package, response.ToByteString());
                } else {
                    response.CanGrabPos = -1;
                    Broadcast(room.Players, package, response.ToByteString());
                    _fightService.BecomeLord(room);
                }
            } else {
                response.CanGrabPos = -1;
                Broadcast(room.Players, package, response.ToByteString());
                _fightService.BecomeLord(room);
            }
        }

        // 用户选择“抢”或“不抢”，都无法再进行抢地主，只能抢1次
        grabLordPlayer.CanGrab = false;
    }

    /// <summary>
    /// 加倍请求
    /// </summary>
    private void OnRaiseHandle(BasePackage package) {
        Session session = SessionMgr.Instance.GetSession(package.SessionId);
        Room room = Cache.Instance.UserRoomDict[session.UserId];
        if (room.RoomState != RoomState.Raise) {
            return;
        }

        var form = RaiseBo.Parser.ParseFrom(package.Data);
        room.RaiseTimes++;
        if (form.IsRaise) {
            room.Multiple *= 2;
        }

        var response = new RaiseResponse {
            LastPos = form.Pos,
            IsRaise = form.IsRaise,
            Multiple = room.Multiple,
            CanRaise = room.RaiseTimes < 3,
            LordPos = room.CurLordPos
        };

        Broadcast(room.Players, package, response.ToByteString());

        // 最多进行3次加倍
        if (room.RaiseTimes >= 3) {
            room.RoomState = RoomState.PlayHand;
        }
    }

    /// <summary>
    /// 出牌请求
    /// </summary>
    private void OnPlayHandHandle(BasePackage package) {
        Session session = SessionMgr.Instance.GetSession(package.SessionId);
        Room room = Cache.Instance.UserRoomDict[session.UserId];
        if (room.RoomState != RoomState.PlayHand) {
            return;
        }

        var form = PlayHandBo.Parser.ParseFrom(package.Data);
        // 当前出牌人与请求参数不符合
        if (room.PendPos != form.Pos) {
            return;
        }

        // 请求出牌的玩家
        var playHandPlayer = room.Players[form.Pos];
        var response = new PlayHandResponse {
            LastPos = form.Pos,
            IsCover = false,
            IsEnd = false,
            IsPass = false,
            MonsterPos = room.MonsterPos,
            Multiple = room.Multiple
        };

        if (form.IsPass) {
            // 不出
            response.IsPass = true;
        } else {
            // 获取出牌的牌型
            var list = form.CardList.ToList();
            var playHand = CardMgr.Instance.GetCardType(list);
            // 打出的牌是炸弹，积分加倍
            if (playHand.Type == CardType.HandBomb || playHand.Type == CardType.HandBombJokers) {
                room.Multiple *= 2;
                response.Multiple = room.Multiple;
            }

            // 本次请求的玩家在上一轮中，打出的牌没人要
            if (form.Pos == room.MonsterPos) {
                // 现在请求的玩家是第一个出牌，将其打出的牌保存下来
                room.PlayCards.Clear();
                room.PlayCards.AddRange(list);
                // 移除手牌中已打出的牌，并检测是否出完牌了
                response.IsEnd = _fightService.RemovePlayerCards(playHandPlayer, list);
                response.PendCards.AddRange(list);
                response.IsCover = true;
                // 出牌次数+1
                playHandPlayer.PlayHandTimes++;
            } else {
                // 比较自己出的牌能否大过上家
                if (CardMgr.Instance.CanBeat(room.PlayCards.ToList(), list)) {
                    room.PlayCards.Clear();
                    room.PlayCards.AddRange(list);
                    response.IsEnd = _fightService.RemovePlayerCards(playHandPlayer, list);
                    // 自己压过了上家的牌，更新当前出牌最大的玩家
                    room.MonsterPos = form.Pos;
                    response.MonsterPos = form.Pos;
                    response.PendCards.AddRange(list);
                    response.IsCover = true;
                    // 出牌次数+1
                    playHandPlayer.PlayHandTimes++;
                }
            }
        }

        // 如果玩家的牌已经打完，游戏结束，开始结算
        if (response.IsEnd) {
            room.RoomState = RoomState.GameEnd;

            // 如果当前出牌玩家是地主，并且农民的出牌次数相加为0，则表示地主打出了“春天”
            if (playHandPlayer.Pos == room.CurLordPos) {
                int playHandCount = room.Players.Where(p => p.Pos != playHandPlayer.Pos).Sum(p => p.PlayHandTimes);

                // “春天”，积分加倍
                if (playHandCount == 0) {
                    response.IsSpring = true;
                    room.Multiple *= 2;
                    response.Multiple = room.Multiple;
                }
            } else {
                // 当前出牌玩家是农民，并且地主只出了1次牌，则表示当前玩家打出了“春天”
                var landPlayer = room.Players.FirstOrDefault(p => p.Pos == room.CurLordPos);
                if (landPlayer != null && landPlayer.PlayHandTimes == 1) {
                    response.IsSpring = true;
                    room.Multiple *= 2;
                    response.Multiple = room.Multiple;
                }
            }
        } else {
            // 轮到下一个玩家出牌
            room.PendPos++;
            if (room.PendPos == 3) {
                room.PendPos = 0;
            }
        }

        Broadcast(room.Players, package, response.ToByteString());
    }

    /// <summary>
    /// 群发消息给房间内的用户
    /// </summary>
    private void Broadcast(RepeatedField<Player> players, BasePackage package, ByteString response) {
        // 将进行抢地主操作的玩家广播给房间内的玩家
        foreach (var p in players) {
            Cache.Instance.SessionDict[p.Id].SendData(package, package.Code, response);
        }
    }
}