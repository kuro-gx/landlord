using UnityEngine;
using UnityEngine.UI;

public class PocketCardPanel : UIBase {
    [SerializeField, Header("底牌左")] private Image _cardLeft;
    [SerializeField, Header("底牌中")] private Image _cardCenter;
    [SerializeField, Header("底牌右")] private Image _cardRight;

    public override void Init() {
        // Sprite[] cardImages = Resources.LoadAll<Sprite>("Sprites/card_small");
        // _cardLeft.sprite = cardImages[0];
        // _cardCenter.sprite = cardImages[1];
        // _cardRight.sprite = cardImages[2];
    }
}