using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using SqlSugar;

public class FightService {
    private readonly SqlSugarClient _db;

    public FightService(SqlSugarClient db) {
        _db = db;
    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="room">房间</param>
    public void StartDispatchCardHandle(Room room) {
        // 清空玩家手牌
        foreach (var player in room.Players) {
            player.CardList.Clear();
        }

        // 清空底牌
        room.HoleCards.Clear();

        // 初始化54张牌
        List<Card> allCards = CardMgr.Instance.InitAllCards();
        // 设置3张底牌
        room.HoleCards.Add(allCards[0]);
        room.HoleCards.Add(allCards[1]);
        room.HoleCards.Add(allCards[2]);
        // 分发3副初始牌，每副17张
        for (int i = 3; i < allCards.Count; i++) {
            room.Players[i % 3].CardList.Add(allCards[i]);
        }

        // 将卡牌分发给用户
        var dispatchCardRes = new DispatchCardResponse { BaseScore = 3, Multiple = 1 };
        foreach (var p in room.Players) {
            dispatchCardRes.CardList.Clear();
            dispatchCardRes.CardList.AddRange(p.CardList);
            Cache.Instance.SessionDict[p.Id].SendData(NetDefine.CMD_DispatchCardCode, dispatchCardRes.ToByteString());
        }
    }

    /// <summary>
    /// 成为地主
    /// </summary>
    /// <param name="room">房间信息</param>
    public void BecomeLord(Room room) {
        // 设置出牌人坐位
        room.PendPos = room.CurLordPos;
        // 分发底牌
        var holeCards = room.HoleCards;
        room.Players[room.CurLordPos].CardList.AddRange(holeCards);
        // 修改房间状态为加倍
        room.RoomState = RoomState.Raise;

        var response = new BecomeLordResponse { LordPos = room.CurLordPos };
        response.HoleCards.AddRange(room.HoleCards);
        foreach (var p in room.Players) {
            Cache.Instance.SessionDict[p.Id].SendData(NetDefine.CMD_BecomeLordCode, response.ToByteString());
        }
    }

    /// <summary>
    /// 移除玩家手牌
    /// </summary>
    /// <param name="player">玩家</param>
    /// <param name="pendCards">打出的牌</param>
    /// <returns>true: 没牌了, false: 还有牌</returns>
    public bool RemovePlayerCards(Player player, List<Card> pendCards) {
        for (int i = player.CardList.Count - 1; i >= 0; i--) {
            foreach (var card in pendCards) {
                if (player.CardList[i].Point == card.Point && player.CardList[i].Suit == card.Suit) {
                    player.CardList.RemoveAt(i);
                    break;
                }
            }
        }

        return player.CardList.Count == 0;
    }

    /// <summary>
    /// 游戏结算
    /// </summary>
    /// <param name="player">最后请求出牌的玩家，即赢家</param>
    /// <param name="room">房间数据</param>
    public void CalcGameResult(Player player, Room room) {
        var response = new GameEndResponse { BaseScore = room.BaseScore };

        // 如果最后出牌玩家是地主，并且农民的出牌次数相加为0，则表示地主打出了“春天”
        if (player.Pos == room.CurLordPos) {
            // 农民出牌次数
            int playHandCount = room.Players.Where(p => p.Pos != player.Pos).Sum(p => p.PlayHandTimes);
            // “春天”，积分加倍
            if (playHandCount == 0) {
                room.Multiple *= 2;
            }

            foreach (var p in room.Players) {
                // 查询数据库内的用户信息
                User user = _db.Queryable<User>().Where(v => v.Id == p.Id).First();

                if (p.Pos == room.CurLordPos) {
                    response.Players.Add(new PlayerResult {
                        IsWin = true,
                        IsSpring = playHandCount == 0,
                        Pos = p.Pos,
                        Money = room.BaseScore * room.Multiple * 2,
                        IsLord = true,
                        Nickname = p.Username
                    });

                    if (user != null) {
                        user.Money += room.BaseScore * room.Multiple * 2;
                        user.WinCount++;
                    }
                } else {
                    response.Players.Add(new PlayerResult {
                        IsWin = false,
                        IsSpring = playHandCount == 0,
                        Pos = p.Pos,
                        Money = room.BaseScore * room.Multiple * -1,
                        IsLord = false,
                        Nickname = p.Username
                    });
                    
                    if (user != null) {
                        user.Money -= room.BaseScore * room.Multiple;
                        user.LoseCount++;
                    }
                }
                
                _db.Updateable(user).ExecuteCommand();
            }
        } else {
            // 当前出牌玩家是农民，并且地主只出了1次牌，则表示当前玩家打出了“春天”
            var landPlayer = room.Players.FirstOrDefault(p => p.Pos == room.CurLordPos);
            if (landPlayer != null && landPlayer.PlayHandTimes == 1) {
                room.Multiple *= 2;
            }

            foreach (var p in room.Players) {
                // 查询数据库内的用户信息
                User user = _db.Queryable<User>().Where(v => v.Id == p.Id).First();
                
                if (p.Pos == room.CurLordPos) {
                    response.Players.Add(new PlayerResult {
                        IsWin = false,
                        IsSpring = landPlayer != null && landPlayer.PlayHandTimes == 1,
                        Pos = p.Pos,
                        Money = room.BaseScore * room.Multiple * -1,
                        IsLord = true,
                        Nickname = p.Username
                    });

                    if (user != null) {
                        user.Money -= room.BaseScore * room.Multiple;
                        user.LoseCount++;
                    }
                } else {
                    response.Players.Add(new PlayerResult {
                        IsWin = true,
                        IsSpring = landPlayer != null && landPlayer.PlayHandTimes == 1,
                        Pos = p.Pos,
                        Money = room.BaseScore * room.Multiple / 2,
                        IsLord = false,
                        Nickname = p.Username
                    });
                    
                    if (user != null) {
                        user.Money += room.BaseScore * room.Multiple / 2;
                        user.WinCount++;
                    }
                }
                
                _db.Updateable(user).ExecuteCommand();
            }
        }

        response.Multiple = room.Multiple;

        foreach (var p in room.Players) {
            Cache.Instance.SessionDict[p.Id].SendData(NetDefine.CMD_GameEndCode, response.ToByteString());
        }
    }
}