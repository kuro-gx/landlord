using System.Collections.Generic;

/// <summary>
/// 玩家
/// </summary>
public class Player {
    /// <summary>
    /// 用户ID
    /// </summary>
    public int Id {　get; set;　}

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string Username {　get; set;　}

    /// <summary>
    /// 豆子
    /// </summary>
    public string Amount {　get; set;　}

    /// <summary>
    /// 坐位索引
    /// </summary>
    public int SeatIndex {　get; set;　}

    /// <summary>
    /// 是否已经准备
    /// </summary>
    public bool IsReady {　get; set;　}

    /// <summary>
    /// 手牌
    /// </summary>
    public List<Card> Cards { get; set; }
}