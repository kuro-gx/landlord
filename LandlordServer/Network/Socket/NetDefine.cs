public class NetDefine {
    public const string IP = "127.0.0.1";
    public const int ServerPort = 10086;
    public const ushort CMD_ErrCode = 1001; // 错误码
    public const ushort CMD_RegisterCode = 801; // 注册请求码
}

/// <summary>
/// 连接状态
/// </summary>
public enum ConnState {
    Connected,
    Disconnected
}
