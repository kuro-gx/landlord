public class NetDefine {
    public const string IP = "127.0.0.1";
    public const int ServerPort = 10086;
    
    // ====================== 请求码 ============================
    public const ushort CMD_ErrCode = 1001; // 错误码
    public const ushort CMD_RegisterCode = 801; // 注册请求码
    public const ushort CMD_LoginCode = 802; // 登录请求码
    public const ushort CMD_UpdateUserInfoCode = 803; // 修改用户信息请求码
    public const ushort CMD_MatchCode = 811; // 匹配请求码
    public const ushort CMD_DispatchCardCode = 821; // 发牌
    public const ushort CMD_CallLordCode = 822; // 叫地主
    public const ushort CMD_GrabLordCode = 823; // 抢地主
}

/// <summary>
/// 连接状态
/// </summary>
public enum ConnState {
    Connected,
    Disconnected
}
