
/// <summary>
/// 出牌组合或方式
/// </summary>
public enum CardType {
    Hand_Unknow,                // 未知 or 非法
    Hand_Pass,                  // 过

    Hand_Single,                // 单
    Hand_Pair,                  // 对

    Hand_Triple,                // 三个
    Hand_Triple_Single,         // 三带一
    Hand_Triple_Pair,           // 三带二

    Hand_Plane,                 // 飞机，555_666
    Hand_Plane_Two_Single,      // 飞机带单，555_666_3_4
    Hand_Plane_Two_Pair,        // 飞机带双，555_666_33_44

    Hand_Seq_Pair,              // 连对，33_44_55(_66...)
    Hand_Seq_Single,            // 顺子，34567(8...)

    Hand_Bomb,                  // 炸弹
    Hand_Bomb_Single,           // 炸弹带一张单牌
    Hand_Bomb_Pair,             // 炸弹带一对
    Hand_Bomb_Two_Single,       // 炸弹带两单

    Hand_Bomb_Jokers,           // 王炸
    Hand_Bomb_Jokers_Single,    // 王炸带一个
    Hand_Bomb_Jokers_Pair,      // 王炸带一对
    Hand_Bomb_Jokers_Two_Single	// 王炸带两单
}

/// <summary>
/// 卡牌花色
/// </summary>
public enum CardSuit {
    None,
    Diamond,    // 方块
    Club,       // 梅花
    Heart,      // 红桃
    Spade       // 黑桃
}

/// <summary>
/// 卡牌点数
/// </summary>
public enum CardPoint {
    None,
    Three,      // 3
    Four,       // 4
    Five,       // 5
    Six,        // 6
    Seven,      // 7
    Eight,      // 8
    Nine,       // 9
    Ten,        // 10
    Jack,       // J
    Queen,      // Q
    King,       // K
    One,        // A
    Two,        // 2
    JokerSmall, // 小王
    JokerBig    // 大王
}
