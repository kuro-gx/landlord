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
    [SerializeField, Header("手牌区域")] private RectTransform selfCardArea;

    public List<Card> SelfCardList; // 自己的手牌
    public readonly List<CardPanel> SelfCardPanelList = new(20); // 手牌的预制体列表
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

            // 地主图片从屏幕中间移动到昵称上面
            var component = GetOrAddComponent<RectPosTween>(landlordIconEl.gameObject);
            component.MoveLocalPosTime(1.6f, new Vector3(-605, -305, 0));
        } else {
            landlordIconEl.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 销毁卡牌预制体列表
    /// </summary>
    public void DestroyCardPanelList() {
        foreach (var child in SelfCardPanelList) {
            Destroy(child.gameObject);
        }

        SelfCardPanelList.Clear();
    }

    /// <summary>
    /// 初始化手牌的预制体列表
    /// </summary>
    /// <param name="list">手牌</param>
    public void InitCardPanelList(List<Card> list) {
        // 对手牌进行从大到小排序后保存手牌
        list.Sort(CardSort);
        SelfCardList = list;

        // 创建卡牌预制体
        for (var i = 0; i < SelfCardList.Count; i++) {
            var cardPanel = CreateCardPanel(SelfCardList[i]);
            // 设置卡牌的位置
            cardPanel.transform.localPosition = new Vector2(GetFirstCardX() + i * Constant.CardHDistance, Constant.CardVDistance);
            // 保存卡牌预制体，做位移动画和插入底牌时使用
            SelfCardPanelList.Add(cardPanel);
        }
    }

    /// <summary>
    /// 成为地主
    /// </summary>
    /// <param name="lastCard">底牌</param>
    public void BecomeLord(List<Card> lastCard) {
        // 将底牌加入手牌后，将手牌进行从大到小排序
        SelfCardList.AddRange(lastCard);
        SelfCardList.Sort(CardSort);

        // 保存底牌的预制体集合
        var pocketCardPanelList = new List<CardPanel>();
        foreach (var card in lastCard) {
            var cardPanel = CreateCardPanel(card);
            cardPanel.Show();

            // 获取底牌加入手牌后的位置索引
            int insertIndex = GetCardPos(card);
            // 设置底牌的位置，底牌不需要做向右的位移动画，因此位置需要多加上 1个卡牌间距
            cardPanel.transform.localPosition = new Vector2(GetFirstCardX() + insertIndex * Constant.CardHDistance + Constant.CardHDistance, 0);
            // 将预制体插入集合
            SelfCardPanelList.Add(cardPanel);
            pocketCardPanelList.Add(cardPanel);
        }

        SelfCardPanelList.Sort((a, b) => b.CardValue.CompareTo(a.CardValue));
        // 插入底牌后，需重新调整所有手牌的位置
        for (var i = 0; i < SelfCardPanelList.Count; i++) {
            // 重新设置卡牌预制体在父组件中的次序
            SelfCardPanelList[i].transform.SetSiblingIndex(i);
            // 卡牌的新位置
            var newPositionX = GetFirstCardX() + i * Constant.CardHDistance + Constant.CardHDistance;
            // 卡牌旧的位置
            var oldPositionX = SelfCardPanelList[i].transform.localPosition.x;
            // 卡牌的新旧位置相等，说明该牌是底牌，无须再次挪动
            if (Mathf.Approximately(newPositionX, oldPositionX)) {
                continue;
            }

            // 移动的距离为：新X坐标 - 旧X坐标
            SelfCardPanelList[i].MovePosInTime(Constant.CardMoveTime, new Vector3(newPositionX - oldPositionX, 0, 0));
        }

        // 底牌往下位移
        foreach (var item in pocketCardPanelList) {
            item.MovePosInTime(Constant.CardSlideTime, new Vector3(0, Constant.CardVDistance, 0));
        }
    }

    /// <summary>
    /// 获取底牌在手牌中的索引
    /// </summary>
    /// <param name="card">底牌</param>
    /// <returns>位置索引</returns>
    private int GetCardPos(Card card) {
        int index = -1;
        for (int i = 0; i < SelfCardList.Count; i++) {
            if (card.Point == SelfCardList[i].Point && card.Suit == SelfCardList[i].Suit) {
                index = i;
                break;
            }
        }

        return index;
    }

    /// <summary>
    /// 卡牌排序，从大到小
    /// </summary>
    private int CardSort(Card a, Card b) {
        return ((int)b.Point * 10 + (int)b.Suit).CompareTo((int)a.Point * 10 + (int)a.Suit);
    }

    /// <summary>
    /// 在手牌区域创建卡牌预制体
    /// </summary>
    /// <param name="card">卡牌信息</param>
    private CardPanel CreateCardPanel(Card card) {
        var cardPanel = Resources.Load<CardPanel>("Prefabs/CardPanel");
        if (cardPanel == null) {
            return null;
        }

        var panel = Instantiate(cardPanel, selfCardArea);
        panel.SetCardInfo(card);

        return panel;
    }

    /// <summary>
    /// 获取最左边第一张牌的起始X坐标
    /// </summary>
    private float GetFirstCardX() {
        float screenWidth = selfCardArea.rect.width;
        // 手牌距离左侧屏幕的宽度：(屏幕宽度 - ((卡牌数量 - 1) * 卡牌间距 + 卡牌宽度)) / 2
        float distanceLeftWidth = (screenWidth - (Constant.CardHDistance * (SelfCardList.Count - 1) + Constant.CardPanelWidth)) / 2;
        // 最左边第一张牌的起始X坐标(卡牌预制体的中心点为0,0.5)：出牌区域起始X坐标 - 手牌距离左侧屏幕的宽度 - 卡牌位移距离
        return distanceLeftWidth - screenWidth / 2 - Constant.CardHDistance;
    }
}