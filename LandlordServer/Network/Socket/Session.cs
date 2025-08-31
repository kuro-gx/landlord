using System;
using System.Collections.Generic;
using System.Net.Sockets;

public class Session : ServerBase {
    public int SessionId { get; set; }
    public int UserId { get; set; }
    private readonly Dictionary<int, IContainer> _cmdDict;

    public Session(Dictionary<int, IContainer> cmdDict) {
        _cmdDict = cmdDict;
        UserId = 0;
        SessionMgr.Instance.AddSession(this);
    }

    /// <summary>
    /// 接收客户端数据
    /// </summary>
    /// <param name="socket">客户端连接对象</param>
    public void ReceiveData(Socket socket) {
        _socket = socket;
        BeginReceive();
    }

    protected override void HandleCommand(BasePackage package) {
        IContainer container = _cmdDict[package.Code];
        if (container == null) {
            Console.WriteLine("command not register...");
            return;
        }

        package.SessionId = SessionId;
        container.OnServerCommand(package);
    }

    public override void DisconnectHandle() {
        Console.WriteLine("Disconnect: " + _socket.RemoteEndPoint + "断开了连接...");
        SessionMgr.Instance.RemoveSession(SessionId);
        UserId = 0;
        base.DisconnectHandle();
    }
}