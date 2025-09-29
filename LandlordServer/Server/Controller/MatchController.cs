using Google.Protobuf;
using System;

/// <summary>
/// 匹配控制器
/// </summary>
public class MatchController : IContainer {
    // 玩家在房间内的位置索引
    private int _posIndex;
    private Room _matchingRoom;
    private int _roomId;

    private FightService _fightService;

    public MatchController(FightService service) {
        _fightService = service;
    }

    public void OnInit() {
    }

    public void OnServerCommand(BasePackage package) {
        switch (package.Code) {
            case NetDefine.CMD_MatchCode:
                OnMatchHandle(package);
                break;
        }
    }

    /// <summary>
    /// 匹配
    /// </summary>
    private void OnMatchHandle(BasePackage package) {
        Session session = SessionMgr.Instance.GetSession(package.SessionId);
        User loginUser = Cache.Instance.UserDict[session.UserId];
        // 构造匹配玩家的数据
        Player player = new Player {
            Id = session.UserId,
            Username = loginUser.Username,
            Money = loginUser.Money,
            Pos = _posIndex,
            CanGrab = true
        };

        if (_posIndex == 0) {
            _matchingRoom = new Room {
                RoomId = ++_roomId,
                Players = { player },
                RoomState = RoomState.Matching,
                CallPos = -1,
                CallTimes = 0,
                BaseScore = 3,
                Multiple = 1,
                GrabTimes = 0,
                CurLordPos = -1,
                PendPos = -1,
                RaiseTimes = 0
            };
        } else {
            _matchingRoom.Players.Insert(_posIndex, player);
        }

        _posIndex++;

        // 用户加入房间后，将消息发送给房间内的所有玩家
        for (var i = 0; i < _matchingRoom.Players.Count; i++) {
            Player roomPlayer = _matchingRoom.Players[i];
            MatchResponse matchResult = new MatchResponse {
                RoomId = _matchingRoom.RoomId,
                SelfPos = roomPlayer.Pos
            };
            matchResult.Player.AddRange(_matchingRoom.Players);
            Cache.Instance.SessionDict[roomPlayer.Id].SendData(package, package.Code, matchResult.ToByteString());
        }
        // 缓存用户加入的房间
        Cache.Instance.UserRoomDict.Add(session.UserId, _matchingRoom);

        // 匹配满3个人之后开始游戏
        if (_matchingRoom.RoomState == RoomState.Matching && _posIndex > 2) {
            // 发牌
            _fightService.StartDispatchCardHandle(_matchingRoom);
            // 修改房间状态为“叫地主”
            _matchingRoom.RoomState = RoomState.CallLord;
            // 重置数据
            _matchingRoom = null;
            _posIndex = 0;
        }
    }
}