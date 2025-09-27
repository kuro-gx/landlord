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

    public List<Card> SelfCardList; // 自己的手牌
    private List<Card> _prepareCardList = new(); // 将要打出去的牌

    protected override void Init() {
        // 设置用户信息
        if (Global.LoginUser != null) {
            usernameEl.text = Global.LoginUser.Username;
            moneyEl.text = Global.LoginUser.Money.ToString();
        }

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

    /// <summary>
    /// 显示 or 隐藏地主图标
    /// </summary>
    public void ChangeLordIconVisibility(bool visibility = true) {
        // 缩放显示，普通隐藏
        if (visibility) {
            landlordIconEl.gameObject.SetActive(true);

            var scaleTween = GetOrAddComponent<RectScaleTween>(landlordIconEl.gameObject);
            scaleTween.ScaleTo(0.3f, Vector3.zero * 1.5f,
                () => { scaleTween.ScaleTo(0.15f, Vector3.zero, () => { Destroy(scaleTween); }); });
        } else {
            landlordIconEl.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 计算底牌该插入手牌的位置
    /// </summary>
    /// <param name="card">底牌</param>
    /// <returns>该插入的位置索引</returns>
    public int GetCardPos(Card card) {
        int index = -1;
        for (int i = 0; i < SelfCardList.Count; i++) {
            Card c = SelfCardList[i];
            if ((int)c.Point * 10 + (int)c.Suit >= (int)card.Point * 10 + (int)card.Suit) {
                index = i;
                break;
            }
        }

        return index;
    }
}