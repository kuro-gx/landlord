using System.Collections.Generic;

/// <summary>
/// 缓存
/// </summary>
public class Cache : Singleton<Cache> {
    /// <summary>
    /// 用户ID 与 User类的映射
    /// </summary>
    public readonly Dictionary<int, User> UserDict = new Dictionary<int, User>();

    /// <summary>
    /// 用户ID 与 连接对象的映射
    /// </summary>
    public readonly Dictionary<int, Session> SessionDict = new Dictionary<int, Session>();

    /// <summary>
    /// 用户ID 与 游戏房间的映射
    /// </summary>
    public readonly Dictionary<int, Room> UserRoomDict = new Dictionary<int, Room>();
}