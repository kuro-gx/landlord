using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 展示玩家打出的牌
/// </summary>
public class CardDisplay : UIBase {
    [SerializeField, Header("背景")] private Image cardBackground;
    [SerializeField, Header("卡牌点数")] private Image pointImage;
    
    protected override void Init() {
    }

    /// <summary>
    /// 设置卡牌信息
    /// </summary>
    /// <param name="card">卡牌数据</param>
    public void SetCardInfo(Card card) {
        // 所有的卡牌图片
        Sprite[] cardImages = Resources.LoadAll<Sprite>("Sprites/card_big");
        // 大小王
        if (card.Point == CardPoint.JokerSmall || card.Point == CardPoint.JokerBig) {
            cardBackground.sprite = card.Point == CardPoint.JokerSmall ? cardImages[52] : cardImages[53];
            pointImage.gameObject.SetActive(false);
            return;
        }
        
        // 卡牌图片的名字
        string cardName = $"{(int)card.Point}_{(int)card.Suit}";
        foreach (var img in cardImages) {
            if (img.name == cardName) {
                pointImage.sprite = img;
                break;
            }
        }
    }
}