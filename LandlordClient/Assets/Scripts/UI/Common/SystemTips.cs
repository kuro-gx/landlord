using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SystemTips : MonoBehaviour {
    [SerializeField, Header("提示信息")] private Text tipMessageText;

    public void RefreshUI(string msg, Color color) {
        tipMessageText.text = msg;
        tipMessageText.color = color;

        RectTransform rectTrans = transform as RectTransform;
        if (rectTrans != null) {
            rectTrans.DOAnchorPosY(rectTrans.anchoredPosition.y - 130, 1);
        }
        
        // 定时销毁当前对象
        Observable.Timer(TimeSpan.FromSeconds(2.5)).Subscribe(v => {
            Destroy(gameObject);
        });
    }
}