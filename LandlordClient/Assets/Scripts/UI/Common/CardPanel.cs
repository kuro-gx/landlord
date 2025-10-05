using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 卡牌窗体
/// </summary>
public class CardPanel : UIBase {
    [SerializeField, Header("卡牌背景")] private Image cardBg;
    [SerializeField, Header("卡牌数值1")] private Image cardValue1Image;
    [SerializeField, Header("卡牌数值2")] private Image cardValue2Image;
    [SerializeField, Header("地主图标")] private Image landlordIcon;

    // 卡牌的数值，排序使用
    public int cardValue;
    // 卡牌是否被选中
    public bool isSelected;

    protected override void Init() {
    }

    /// <summary>
    /// 设置卡牌信息
    /// </summary>
    /// <param name="card">卡牌数据</param>
    /// <param name="index">索引</param>
    /// <param name="isLord">是否是地主</param>
    public void SetCardInfo(Card card, int index, bool isLord = false) {
        landlordIcon.gameObject.SetActive(isLord);

        cardValue = (int)card.Point * 10 + (int)card.Suit;
        // 设置预制体名称为索引号，方便选牌时获取索引
        SetCardName(index);
        // 所有的卡牌图片
        Sprite[] cardImages = Resources.LoadAll<Sprite>("Sprites/card_big");
        // 大小王
        if (card.Point == CardPoint.JokerSmall || card.Point == CardPoint.JokerBig) {
            cardBg.sprite = card.Point == CardPoint.JokerSmall ? cardImages[52] : cardImages[53];
            cardValue1Image.gameObject.SetActive(false);
            cardValue2Image.gameObject.SetActive(false);
            return;
        }

        // 卡牌图片的名字
        string cardName = $"{(int)card.Point}_{(int)card.Suit}";
        foreach (var img in cardImages) {
            if (img.name == cardName) {
                cardValue1Image.sprite = img;
                cardValue2Image.sprite = img;
                break;
            }
        }
    }

    /// <summary>
    /// 平移卡牌预制体
    /// </summary>
    /// <param name="time">平移的间隔时间</param>
    /// <param name="offset">平移距离</param>
    /// <param name="cb">回调函数</param>
    public void MoveLocalPosInTime(float time, Vector3 offset, Action cb = null) {
        RectPosTween component = GetOrAddComponent<RectPosTween>(gameObject);
        component.MoveLocalPosInTime(time, offset, cb);
    }

    public void MoveTargetPosInTime(float time, Vector3 target, Action cb = null) {
        RectPosTween component = GetOrAddComponent<RectPosTween>(gameObject);
        component.MoveTargetPosInTime(time, target, cb);
    }

    /// <summary>
    /// 设置卡牌名称为索引值，方便做选中时获取索引
    /// </summary>
    /// <param name="index">索引</param>
    public void SetCardName(int index) {
        gameObject.name = index.ToString();
    }

    /// <summary>
    /// 设置卡牌选中状态
    /// </summary>
    /// <param name="selected">是否选中</param>
    /// <param name="move">是否需要移动</param>
    /// <param name="hasAnima">是否启用动画</param>
    public void SetCardSelected(bool selected, bool move = true, bool hasAnima = true) {
        if (isSelected == selected) {
            return;
        }

        isSelected = selected;
        if (!move) return;
        
        // 卡牌当前位置
        var nowPosition = gameObject.transform.localPosition;
        // 新的Y坐标
        float positionY = selected ? 5 : Constant.CardVDistance;
        if (hasAnima) {
            MoveTargetPosInTime(0.1f, new Vector3(nowPosition.x, positionY));
        } else {
            gameObject.transform.localPosition = new Vector3(nowPosition.x, positionY);
        }
    }
}