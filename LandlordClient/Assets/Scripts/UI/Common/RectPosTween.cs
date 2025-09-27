using System;
using UnityEngine;

/// <summary>
/// 移动动画
/// </summary>
public class RectPosTween : MonoBehaviour {
    private RectTransform _rectTransform;
    private bool _isRun;
    private float _moveTime; // 移动耗时
    private Vector3 _startPos = Vector3.zero; // 起始位置
    private Vector3 _moveSpeed = Vector3.zero; // 移动速度
    private Vector3 _targetPos = Vector3.zero; // 目标位置
    private float _countTime; // 总耗时
    private Action _callback; // 回调函数

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 移动函数
    /// </summary>
    /// <param name="time">移动耗时</param>
    /// <param name="offset">移动距离</param>
    /// <param name="cb">回调函数</param>
    public void MoveLocalPosTime(float time, Vector3 offset, Action cb = null) {
        _startPos = _rectTransform.localPosition;
        _moveTime = time;
        _targetPos = _startPos + offset;
        _moveSpeed = (_targetPos - _startPos) / _moveTime;
        _countTime = 0;
        _callback = cb;
        _isRun = true;
    }

    private void Update() {
        if (_isRun) {
            // 获取当前帧的时间
            float delta = Time.deltaTime;
            _rectTransform.localPosition += _moveSpeed * delta;
            if (_countTime >= _moveTime) {
                _rectTransform.localPosition = _targetPos;
                _isRun = false;
                _moveTime = 0;
                _targetPos = Vector3.zero;
                _moveSpeed = Vector3.zero;
                _startPos = Vector3.zero;
                _countTime = 0;
                
                _callback?.Invoke();
            }

            _countTime += delta;
        }
    }

    private void OnDisable() {
        _isRun = false;
        _moveTime = 0;
        _targetPos = Vector3.zero;
        _moveSpeed = Vector3.zero;
        _startPos = Vector3.zero;
        _countTime = 0;
        _callback = null;
    }
}