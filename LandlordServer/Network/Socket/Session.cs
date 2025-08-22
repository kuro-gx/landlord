using System;
using System.Collections.Generic;
using System.Net.Sockets;

public class Session : ServerBase {
    public Session(Dictionary<int, IContainer> cmdDict) {
        _cmdDict = cmdDict;
    }
    
    /// <summary>
    /// 接收客户端数据
    /// </summary>
    /// <param name="socket">客户端连接对象</param>
    public void ReceiveData(Socket socket) {
        _socket = socket;
        BeginReceive();
    }

    protected override void CommandHandle(BasePackage package) {
        IContainer container = _cmdDict[package.Code];
        if (container == null) {
            Console.WriteLine("command not register...");
            return;
        }

        container.OnServerCommand(this, package);
    }
}