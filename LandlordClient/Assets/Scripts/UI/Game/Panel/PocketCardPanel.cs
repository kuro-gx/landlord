using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 底牌面板
/// </summary>
public class PocketCardPanel : UIBase {
    [SerializeField, Header("底牌左")] private Image cardLeftImage;
    [SerializeField, Header("底牌中")] private Image cardCenterImage;
    [SerializeField, Header("底牌右")] private Image cardRightImage;

    protected override void Init() {
    }

    /// <summary>
    /// 设置底牌
    /// </summary>
    /// <param name="holeCards">底牌</param>
    public void SetPocketCard(List<Card> holeCards) {
        Sprite[] cardImages = Resources.LoadAll<Sprite>("Sprites/card_small");
        string leftName = $"{(int)holeCards[0].Point}_{(int)holeCards[0].Suit}";
        string centerName = $"{(int)holeCards[1].Point}_{(int)holeCards[1].Suit}";
        string rightName = $"{(int)holeCards[2].Point}_{(int)holeCards[2].Suit}";
        
        foreach (var img in cardImages) {
            if (img.name == leftName) {
                cardLeftImage.sprite = img;
            } else if (img.name == centerName) {
                cardCenterImage.sprite = img;
            } else if (img.name == rightName) {
                cardRightImage.sprite = img;
            }
        }

        // 设置缩放动画
        gameObject.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.5f).OnComplete(() => {
            gameObject.transform.DOScale(Vector3.one, 0.2f);
        });
    }

    /// <summary>
    /// 将底牌设置为背面
    /// </summary>
    public void SetPocketCardBack() {
        Sprite back = Resources.Load<Sprite>("Icon/CardBack");
        cardLeftImage.sprite = back;
        cardCenterImage.sprite = back;
        cardRightImage.sprite = back;
    }
}