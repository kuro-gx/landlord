using System;
using UnityEngine;

/// <summary>
/// 缩放动画组件
/// </summary>
public class RectScaleTween : MonoBehaviour {
    private RectTransform _rectTransform;

    private bool _isRun;
    // 缩放时间
    private float _scaleTime;
    // 缩放总耗时
    private float _countTime = 0;
    // 目标缩放大小
    private Vector3 _targetScale = Vector3.zero;
    // 缩放速率
    private Vector3 _scaleSpeed = Vector3.zero;
    // 原缩放大小
    private Vector3 _startScale = Vector3.zero;
    // 回调
    private Action _callback;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 缩放动画
    /// </summary>
    /// <param name="time">动画持续时间</param>
    /// <param name="targetScale">目标缩放值</param>
    /// <param name="callback">回调</param>
    public void ScaleTo(float time, Vector3 targetScale, Action callback = null) {
        _startScale = _rectTransform.localScale;
        _scaleTime = time;
        _targetScale = targetScale;
        _scaleSpeed = (targetScale - _startScale) / _scaleTime;
        _callback = callback;
        _isRun = true;
    }

    private void Update() {
        if (_isRun) {
            float delta = Time.deltaTime;
            _rectTransform.localScale += _scaleSpeed * delta;
            _countTime += delta;
            if (_countTime >= _scaleTime) {
                _rectTransform.localScale = _targetScale;

                OnDisable();
                _callback?.Invoke();
            }
        }
    }

    private void OnDisable() {
        // 重置数据
        _isRun = false;
        _scaleTime = 0;
        _countTime = 0;
        _targetScale = Vector3.zero;
        _scaleSpeed = Vector3.zero;
        _startScale = Vector3.zero;
    }
}