using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 全局提示组件
/// </summary>
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
        Destroy(gameObject, 2.5f);
    }
}