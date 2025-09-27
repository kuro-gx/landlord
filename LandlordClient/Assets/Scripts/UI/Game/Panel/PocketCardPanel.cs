using System.Collections.Generic;
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
        Show();
        
        Sprite[] cardImages = Resources.LoadAll<Sprite>("Sprites/card_small");
        string leftName = $"{(int)holeCards[0].Point}_{(int)holeCards[0].Suit}";
        string centerName = $"{(int)holeCards[1].Point}_{(int)holeCards[1].Suit}";
        string rightName = $"{(int)holeCards[2].Point}_{(int)holeCards[2].Suit}";

        // 设置缩放动画
        RectScaleTween scaleTween = GetOrAddComponent<RectScaleTween>(gameObject);
        scaleTween.ScaleTo(0.15f, Vector3.zero * 1.2f, () => {
            // 底牌背景放大到1.2倍后，显示底牌的正面
            foreach (var img in cardImages) {
                if (img.name == leftName) {
                    cardLeftImage.sprite = img;
                } else if (img.name == centerName) {
                    cardCenterImage.sprite = img;
                } else if (img.name == rightName) {
                    cardRightImage.sprite = img;
                }
            }
            
            scaleTween.ScaleTo(0.5f, Vector3.zero * 1.25f, () => {
                scaleTween.ScaleTo(0.2f, Vector3.zero, () => {
                    var timer = GetOrAddComponent<TimerUtil>(gameObject);
                    timer.AddTimerTask(new TimerTask {
                        EndTime = 5,
                        EndCallback = () => {
                            Show(false);
                            Destroy(timer);
                        }
                    });
                    
                    Destroy(scaleTween);
                });
            });
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