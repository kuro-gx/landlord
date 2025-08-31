/// <summary>
/// 房间状态
/// </summary>
public enum RoomState {
    WaitReady,      // 等待
    GameStart,      // 游戏开始
    DispatchCard,   // 发牌状态
    CallingLord,    // 叫地主状态
    PlayingHand,    // 出牌状态
}
