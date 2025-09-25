using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 底牌面板
/// </summary>
public class PocketCardPanel : UIBase {
    [SerializeField, Header("底牌左")] private Image cardLeftImage;
    [SerializeField, Header("底牌中")] private Image cardCenterImage;
    [SerializeField, Header("底牌右")] private Image cardRightImage;

    public override void Init() {
        // Sprite[] cardImages = Resources.LoadAll<Sprite>("Sprites/card_small");
        // _cardLeft.sprite = cardImages[0];
        // _cardCenter.sprite = cardImages[1];
        // _cardRight.sprite = cardImages[2];
    }
}