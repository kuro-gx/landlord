using Google.Protobuf;

/// <summary>
/// 快捷聊天控制器
/// </summary>
public class ChatController : IContainer {
    public void OnInit() {
    }

    public void OnServerCommand(BasePackage package) {
        switch (package.Code) {
            case NetDefine.CMD_ChatCode:
                OnChatHandle(package);
                break;
        }
    }

    /// <summary>
    /// 快捷聊天
    /// </summary>
    private void OnChatHandle(BasePackage package) {
        Session session = SessionMgr.Instance.GetSession(package.SessionId);
        Room room = Cache.Instance.UserRoomDict[session.UserId];

        if (room.RoomState < RoomState.CallLord || room.RoomState == RoomState.GameEnd) {
            return;
        }

        var form = ChatForm.Parser.ParseFrom(package.Data);

        // 将消息广播给全体玩家
        foreach (var p in room.Players) {
            Cache.Instance.SessionDict[p.Id].SendData(package, package.Code, form.ToByteString());
        }
    }
}