using System.Collections.Generic;

/// <summary>
/// 房间
/// </summary>
public class Room {
    /// <summary>
    /// 6位数的房间号
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 底分
    /// </summary>
    public int BaseScore {　get; set;　}

    /// <summary>
    /// 倍率
    /// </summary>
    public int Multiples { get; set; }

    /// <summary>
    /// 房主
    /// </summary>
    public Player RoomMaster { get; set; }

    /// <summary>
    /// 房间中的玩家数量
    /// </summary>
    public int Count => Seats.Values.Count;

    /// <summary>
    /// 房间状态
    /// </summary>
    public RoomState State { get; set; }

    /// <summary>
    /// 坐位  玩家ID:坐位ID
    /// </summary>
    public Dictionary<int, int> Seats { get; set; }

    /// <summary>
    /// 玩家列表
    /// </summary>
    public Player[] Players { get; set; }

    /// <summary>
    /// 54张牌
    /// </summary>
    public Card[] Cards { get; set; }

    public Room() {
        Seats = new Dictionary<int, int>();
        Players = new Player[3];
        Cards = new Card[54];
    }
}