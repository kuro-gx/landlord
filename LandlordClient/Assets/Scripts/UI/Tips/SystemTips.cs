using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SystemTips : MonoBehaviour {
    [SerializeField, Header("提示信息")] private Text _tipMessage;

    public void RefreshUI(string msg, Color color) {
        _tipMessage.text = msg;
        _tipMessage.color = color;

        RectTransform rectTrans = transform as RectTransform;
        rectTrans.DOAnchorPosY(rectTrans.anchoredPosition.y - 130, 1);
        
        // 定时销毁当前对象
        Observable.Timer(TimeSpan.FromSeconds(2.5)).Subscribe(v => {
            Destroy(gameObject);
        });
    }
}