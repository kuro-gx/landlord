using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家面板
/// </summary>
public class SelfPlayerPanel : UIBase {
    [SerializeField, Header("昵称")] private Text usernameEl;
    [SerializeField, Header("豆子")] private Text moneyEl;
    [SerializeField, Header("地主图片标识")] private Image landlordIconEl;
    [SerializeField, Header("人物形象")] private Image characterEl;
    [SerializeField, Header("聊天气泡")] private Image chatContainerEl;
    [SerializeField, Header("提示文字")] private Image tipTextEl;

    private List<Card> _selfCardList = new List<Card>(); // 自己的手牌
    private List<Card> _prepareCardList = new List<Card>(); // 将要打出去的牌

    public override void Init() {
        // 设置用户信息
        if (Global.LoginUser != null) {
            usernameEl.text = Global.LoginUser.Username;
            moneyEl.text = Global.LoginUser.Money.ToString();
        }
        
        _selfCardList.Clear();
        _prepareCardList.Clear();
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
        characterEl.sprite = Resources.Load<Sprite>("Character/Tex_0" + playerInfo.Pos);
    }

    /// <summary>
    /// 显示or隐藏提示消息
    /// </summary>
    public void ChangeTipVisibility(string tipImageName, bool visibility = true) {
        if (tipImageName != null) {
            tipTextEl.sprite = Resources.Load<Sprite>("TipText/" + tipImageName);
            tipTextEl.SetNativeSize();
        }
        tipTextEl.gameObject.SetActive(visibility);
    }
}