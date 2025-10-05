public class Constant {
    // 卡牌预制体的宽度
    public const float CardPanelWidth = 135;
    // 打出的牌的预制体的宽度
    public const float CardDisplayWidth = 66.6f;
    // 卡牌之间的横向距离
    public const float CardHDistance = 50;
    // 卡牌选中之后向上位移的距离
    public const float CardVDistance = -20;
    // 底牌从上往下滑动的时间
    public const float CardSlideTime = 1.2f;
    // 插入底牌后原手牌挪动的耗时
    public const float CardMoveTime = 0.3f;
    // 卡牌移动的时间间隔
    public const float CardMoveDelay = 0.14f;
    // 叫/抢地主倒计时
    public const int CallLordCountDown = 30;
    // 加倍倒计时
    public const int RaiseCountDown = 15;
    // 出牌倒计时
    public const int PlayHandCountDown = 20;
    // BGM
    public const string WinBGM = "MusicEx_Win";
    public const string LostBGM = "MusicEx_Lose";
    public const string LobbyBGM = "MusicEx_Welcome";
    public const string GameBGM = "MusicEx_Normal";
    // UI音频
    public const string NormalClick = "SpecOk";
    // 特效音频
    public const string CardDispatch = "Special_DiscardDispatch";
    public const string SelectedCard = "SpecSelectCard";
    public const string PlayHand = "Special_Give";
    public const string HandSeq = "Special_Seq";
    public const string HandPlane = "Special_Plane";
    public const string HandBomb = "Special_Bomb";
    public const string HandBombJoker = "Special_Long_Bomb";
    // 操作音频
    public const string CallLord = "CallLord";
    public const string NotCall = "NotCall";
    public const string NotGrab = "NotGrab";
    public const string Raise = "Raise";
    public const string NotRaise = "NotRaise";
    public static string[] PassArr = { "Pass1", "Pass2", "Pass3", "Pass4" };
    public const string Alarm = "Alarm"; // 剩下1、2张牌警告
}