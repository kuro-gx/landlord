using System;
using System.Net.Sockets;
using System.Threading;

public class NetClient : ServerBase {
    private string _ip;
    private int _port;
    private Timer _reconnectTimer; // 重连间隔
    public bool _isNeedReconn = true; // 是否需要重连

    public NetClient(string ip, int port) {
        _ip = ip;
        _port = port;
        _connState = ConnState.Disconnected;
    }

    /// <summary>
    /// 开始连接服务器
    /// </summary>
    public void StartConnect() {
        try {
            if (_connState != ConnState.Disconnected) {
                return;
            }

            if (_socket == null) {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            // 开始连接服务器
            _socket.BeginConnect(_ip, _port, OnConnectHandle, null);
        } catch (Exception e) {
            DisconnectHandle();
            Console.WriteLine(e.Message);
            throw;
        }
    }

    /// <summary>
    /// 连接服务器成功后的处理
    /// </summary>
    private void OnConnectHandle(IAsyncResult ar) {
        try {
            _socket.EndConnect(ar);
            _connState = ConnState.Connected;
            // 接收服务器发送的数据
            BeginReceive();
        } catch (Exception e) {
            DisconnectHandle();
            Console.WriteLine(e.Message);
            throw;
        }
    }

    /// <summary>
    /// 断开连接的处理
    /// </summary>
    public override void DisconnectHandle() {
        SetReconnectTimer();
        base.DisconnectHandle();
    }

    protected override void HandleCommand(BasePackage package) {
        OnReceiveMsg?.Invoke(package.Code, package.Data);
    }

    /// <summary>
    /// 设置重连时间间隔
    /// </summary>
    private void SetReconnectTimer() {
        if (_reconnectTimer == null) {
            _reconnectTimer = new Timer(Reconnect);
        }

        _reconnectTimer.Change(3000, 1000);
    }

    private void Reconnect(object state) {
        if (_isNeedReconn) {
            StartConnect();
        }
    }
}