using DG.Tweening;
using UnityEngine;

/// <summary>
/// 人物立绘面板
/// </summary>
public class CharacterAnimation : MonoBehaviour {
    private Sequence _characterSequence;

    private void Start() {
        StartAnimation();
    }

    /// <summary>
    /// 起始动画
    /// </summary>
    private void StartAnimation() {
        // 起点
        transform.localPosition = new Vector3(-1100.0f, -30.0f, 0.0f); 
        if (_characterSequence != null) {
            _characterSequence.Kill();
        }

        _characterSequence = DOTween.Sequence();
        _characterSequence
            .Append(transform.DOLocalMove(new Vector3(-668.0f, -30.0f, 0.0f), .3f)).SetEase(Ease.InOutSine) // 经过点
            .Join(transform.DOScale(new Vector3(1.1f, 1.0f, 1.0f), .3f)).SetDelay(.2f)
            .Append(transform.DOLocalMove(new Vector3(-768.0f, -30.0f, 0.0f), .3f)) // 终点
            .Join(transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), .3f));

        _characterSequence.Play();
    }
}