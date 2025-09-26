using System.Linq;
using Google.Protobuf;

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
        // 请求次数为3，表示现在是最后一个人叫地主
        if (room.CallTimes == 3) {
            if (form.IsCall) {
                // 只有最后一人“叫地主”，则直接成为地主进入加倍环节
                // todo 成为地主
                room.CallPos = form.Pos;
                room.CurLordPos = form.Pos;
                room.RoomState = RoomState.Raise;
            } else {
                // 没人"叫地主"，重新发牌
                room.CallTimes = 0;
                room.Players[0].CanGrab = true;
                room.Players[1].CanGrab = true;
                room.Players[2].CanGrab = true;
                // 重新发牌
                _fightService.StartDispatchCardHandle(room);
                return;
            }
        }

        // 如果玩家选择“叫地主”，将其坐位索引记录下来
        if (form.IsCall) {
            room.CallPos = form.Pos;
            room.CurLordPos = form.Pos;
            // 进入抢地主环节
            room.RoomState = RoomState.GrabLord;
        } else {
            // 玩家不叫地主，则无法参与抢地主
            callLordPlayer.CanGrab = false;
        }

        var response = new CallLordResponse {
            LastPos = form.Pos,
            IsCall = form.IsCall,
            CallPos = room.CallPos
        };
        // 将进行抢地主操作的玩家广播给房间内的玩家
        foreach (var p in room.Players) {
            Cache.Instance.SessionDict[p.Id].SendData(package, package.Code, response.ToByteString());
        }
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
        // 用户选择“抢”或“不抢”，都无法再进行抢地主，只能抢1次
        grabLordPlayer.CanGrab = false;

        var response = new GrabLordResponse {
            LastPos = form.Pos,
            IsGrab = form.IsGrab,
            Multiple = room.Multiple,
            GrabTimes = room.GrabTimes
        };
        
        /*
         * 遍历玩家列表，查看是否还有玩家可以抢地主
         * 玩家1：叫地主                                    CurLordPos = 0
         * 玩家2：不抢，玩家3：抢                            CurLordPos = 2
         * 玩家1：抢 -> 玩家1成为地主，不抢 -> 玩家3成为地主
         */
        int nextCanGrabPlayerPos = -1;
        foreach (var p in room.Players) {
            if (p.CanGrab) {
                nextCanGrabPlayerPos = p.Pos;
            }
        }

        // 已经没有人可以"抢地主"了
        if (nextCanGrabPlayerPos == -1) {
            // 游戏进入加倍状态
            room.RoomState = RoomState.Raise;
            // todo 设置地主
        } else {
            // 设置下一个可以抢地主的玩家的坐位
            response.CanGrabPos = nextCanGrabPlayerPos;
        }
        
        // 将进行抢地主操作的玩家广播给房间内的玩家
        foreach (var p in room.Players) {
            Cache.Instance.SessionDict[p.Id].SendData(package, package.Code, response.ToByteString());
        }
    }
}