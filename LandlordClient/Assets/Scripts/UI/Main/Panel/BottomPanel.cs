using DG.Tweening;
using UnityEngine;

/// <summary>
/// 底部按钮组面板
/// </summary>
public class BottomPanel : MonoBehaviour {
    private RectTransform _transform;

    void Start() {
        _transform = GetComponent<RectTransform>();
        _transform.DOAnchorPos(new Vector2(0.0f, 50.0f), 0.4f).From(new Vector2(0.0f, -50.0f));
    }
}