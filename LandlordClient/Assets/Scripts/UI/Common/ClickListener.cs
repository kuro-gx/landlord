using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 监听组件的点击事件
/// </summary>
public class ClickListener : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerDownHandler {
    public Action<GameObject> OnClickDown;
    public Action<GameObject> OnClickUp;
    public Action<GameObject> OnEnter;

    public void OnPointerDown(PointerEventData eventData) {
        OnClickDown?.Invoke(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnEnter?.Invoke(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData) {
        OnClickUp?.Invoke(gameObject);
    }
}