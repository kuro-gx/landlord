using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class NetServer {
    private Socket _socket;
    private readonly Dictionary<int, IContainer> _cmdDict = new Dictionary<int, IContainer>();

    public void StartServer(string ip, int port) {
        // 创建Socket
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // 设置IP和端口
        EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        // 绑定IP和端口
        _socket.Bind(endPoint);
        // 设置最大连接数
        _socket.Listen(50);

        Console.WriteLine("服务器启动成功：" + endPoint);

        // 监听客户端连接
        Thread listenThread = new Thread(ListenConnectSocket) {
            IsBackground = true
        };
        listenThread.Start();
    }

    /// <summary>
    /// 开始监听客户端连接
    /// </summary>
    private void ListenConnectSocket() {
        // 开始监听
        _socket.BeginAccept(ClientConnectHandle, null);
    }

    /// <summary>
    /// 处理户端连接
    /// </summary>
    private void ClientConnectHandle(IAsyncResult ar) {
        try {
            // 结束监听
            Socket clientSocket = _socket.EndAccept(ar);
            Console.WriteLine($"客户端：{clientSocket.RemoteEndPoint} 连接成功...");
            // 开始接收客户端数据
            Session session = new Session(_cmdDict);
            session.ReceiveData(clientSocket);
            // 处理完当前客户端连接后，继续处理下一个客户端的连接请求
            ListenConnectSocket();
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }
    
    /// <summary>
    /// 注册指令
    /// </summary>
    public void RegisterCommand(int cmd, IContainer container) {
        _cmdDict.Add(cmd, container);
    }
}