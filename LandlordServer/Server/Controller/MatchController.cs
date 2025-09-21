using Google.Protobuf;

public class MatchController : IContainer {
    // 玩家在房间内的位置索引
    private int _posIndex = 0;
    private Room _matchingRoom = null;
    private int _roomId = 0;

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
            Pos = _posIndex
        };

        if (_posIndex == 0) {
            _matchingRoom = new Room {
                RoomId = ++_roomId,
                Player = { player },
                RoomState = RoomState.Matching
            };
        } else {
            _matchingRoom.Player.Insert(_posIndex, player);
        }
        _posIndex++;
        
        // 用户加入房间后，将消息发送给房间内的所有玩家
        for (var i = 0; i < _matchingRoom.Player.Count; i++) {
            Player roomPlayer = _matchingRoom.Player[i];
            MatchResponse matchResult = new MatchResponse {
                RoomId = _matchingRoom.RoomId,
                SelfPos = roomPlayer.Pos
            };
            matchResult.Player.AddRange(_matchingRoom.Player);
            Cache.Instance.SessionDict[roomPlayer.Id].SendData(package, package.Code, matchResult.ToByteString());
        }
        
        // 匹配满3个人之后开始游戏
        if (_matchingRoom.RoomState == RoomState.Matching && _posIndex > 2) {
            _matchingRoom.RoomState = RoomState.Matched;
            // todo 开始游戏

            _matchingRoom = null;
            _posIndex = 0;
            _roomId = 0;
        }
    }
}