public class Constant {
    // 卡牌之间的横向距离
    public const float CardHDistance = 50;
    // 卡牌选中之后向上位移的距离
    public const float CardVDistance = 20;
    // 卡牌滑动的时间
    public const float CardSlideTime = 0.3f;
    // 卡牌移动的时间间隔
    public const float CardMoveDelay = 0.14f;
    // 叫/抢地主倒计时
    public const int CallLordCountDown = 30;
    // 加倍倒计时
    public const int RaiseCountDown = 15;
    // 出牌倒计时
    public const int PlayHandCountDown = 30;
    // UI音频
    public const string NormalClick = "SpecOk";
    // 特效音频
    public const string CardDispatch = "Special_DiscardDispatch";
    // 操作音频
    public const string CallLord = "CallLord";
    public const string NotCall = "NotCall";
    public const string NotGrab = "NotGrab";
    public const string Raise = "Raise";
    public const string NotRaise = "NotRaise";
    public static string[] PassArr = { "Pass1", "Pass2", "Pass3", "Pass4" };
    public const string Alarm1 = "Alarm1"; // 剩1张牌
    public const string Alarm2 = "Alarm1"; // 剩2张牌
}