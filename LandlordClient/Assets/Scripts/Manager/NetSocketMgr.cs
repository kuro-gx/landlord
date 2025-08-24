using System.Threading;
using Google.Protobuf;

/// <summary>
/// 网络模块管理
/// </summary>
public class NetSocketMgr : Singleton<NetSocketMgr> {
    private static NetClient _client;
    private SynchronizationContext _context;

    public static NetClient Client {
        get => _client;
    }

    public void Init() {
        _context ??= SynchronizationContext.Current;

        ConnectServer(NetDefine.IP, NetDefine.ServerPort);
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="host">ip地址</param>
    /// <param name="port">端口</param>
    public void ConnectServer(string host, int port) {
        // Disconnect();

        _client = new NetClient(host, port);
        _client.OnReceiveMsg += OnReceiveMsgHandle;
        _client.StartConnect();
    }

    /// <summary>
    /// 收到服务器发送的数据
    /// </summary>
    /// <param name="code">协议码</param>
    /// <param name="data">数据</param>
    private void OnReceiveMsgHandle(int code, ByteString data) {
        // 切换到主线程，派发数据
        _context.Post(_ => { SocketDispatcher.Instance.DispatcherEvent(code, data); }, null);
    }

    public void Disconnect() {
        if (_client != null) {
            _client._isNeedReconn = false;
            _client.DisconnectHandle();
            _client = null;
        }
    }
}