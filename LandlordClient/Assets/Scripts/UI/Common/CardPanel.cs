using System;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour {
    [SerializeField, Header("卡牌背景")] private Image cardBg;
    [SerializeField, Header("卡牌数值1")] private Image cardValue1Image;
    [SerializeField, Header("卡牌数值2")] private Image cardValue2Image;
    [SerializeField, Header("地主图标")] private Image landlordIcon;

    /// <summary>
    /// 设置卡牌信息
    /// </summary>
    /// <param name="card">卡牌数据</param>
    /// <param name="isLord">是否是地主</param>
    public void SetCardInfo(Card card, bool isLord = false) {
        landlordIcon.gameObject.SetActive(isLord);
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
    public void MovePosInTime(float time, Vector3 offset, Action cb = null) {
        RectPosTween component = GetOrAddComponent<RectPosTween>(gameObject);
        component.MoveLocalPosTime(time, offset, cb);
    }

    private T GetOrAddComponent<T>(GameObject obj) where T : Component {
        T cpt = obj.GetComponent<T>();
        if (cpt == null) {
            cpt = obj.AddComponent<T>();
        }

        return cpt;
    }
}