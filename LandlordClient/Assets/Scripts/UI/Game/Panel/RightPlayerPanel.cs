using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 右侧玩家面板
/// </summary>
public class RightPlayerPanel : UIBase {
    [SerializeField, Header("昵称")] private Text usernameEl;
    [SerializeField, Header("豆子")] private Text moneyEl;
    [SerializeField, Header("手牌背景")] private Image cardStackImageEl;
    [SerializeField, Header("剩余卡牌")] private Text cardStackTextEl;
    [SerializeField, Header("地主图片标识")] private Image landlordIconEl;
    [SerializeField, Header("人物形象")] private Image characterEl;
    [SerializeField, Header("聊天气泡")] private Image chatContainerEl;
    [SerializeField, Header("提示文字")] private Image tipTextEl;
    [SerializeField, Header("倒计时图片")] private Image clockImageEl;

    public override void Init() {
    }
    
    /// <summary>
    /// 更新UI信息
    /// </summary>
    /// <param name="playerInfo">玩家信息</param>
    public void RefreshPanel(Player playerInfo) {
        if (playerInfo == null) {
            return;
        }
        
        usernameEl.text = playerInfo.Username;
        moneyEl.text = playerInfo.Money.ToString();
        characterEl.sprite = Resources.Load<Sprite>("Character/Tex_1" + playerInfo.Pos);
        
        // 显示UI
        usernameEl.gameObject.SetActive(true);
        moneyEl.gameObject.SetActive(true);
        characterEl.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 设置卡牌剩余数量
    /// </summary>
    /// <param name="num">数量</param>
    public void SetCardStack(int num) {
        cardStackTextEl.text = num.ToString();
        // 显示卡牌剩余数量
        if (!cardStackImageEl.gameObject.activeSelf) {
            cardStackImageEl.gameObject.SetActive(true);
        }
    }
}