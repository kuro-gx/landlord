/// <summary>
/// 游戏状态
/// </summary>
public enum GameState {
    CallLord, // 叫地主
    GrabLord, // 抢地主
    Raise, // 加倍
    PlayingHand, // 出牌
    GameEnd // 游戏结束
}

/// <summary>
/// 出牌按钮页应该显示的按钮
/// </summary>
public enum PlayHandBtnEnum {
    None,
    OnlyPass, // 只显示不出
    OnlyPlayHand, // 只显示出牌
    ShowAll // 全部显示
}