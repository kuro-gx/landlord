using System;
using UnityEngine;

public class TimerTask {
    // 定时任务延迟执行的时间
    public float DelayTime;

    // 调用回调函数的间隔时间
    public float RateTime;

    // 定时任务结束时间
    public float EndTime;

    // 延迟结束回调
    public Action DelayCallback;

    // 间隔结束回调
    public Action RateCallback;

    // 定时结束回调
    public Action EndCallback;
}

public class TimerUtil : MonoBehaviour {
    // 计时状态
    enum TimerState {
        None,
        Delay,
        Normal
    }

    private bool _isRun;
    private float _delayCount;
    private float _rateCount;
    private float _endCount;
    private TimerTask _timerTask;
    private TimerState _timerState = TimerState.None;

    /// <summary>
    /// 添加定时任务
    /// </summary>
    public void AddTimerTask(TimerTask task) {
        _timerTask = task;
        _timerState = task.DelayTime > 0 ? TimerState.Delay : TimerState.Normal;

        _isRun = true;
    }

    private void Update() {
        if (_isRun) {
            float delta = Time.deltaTime;
            if (_timerState == TimerState.Delay) {
                DelayTimerHandler(delta);
            } else {
                NormalTimerHandler(delta);
            }
        }
    }

    private void DelayTimerHandler(float delta) {
        _delayCount += delta;
        float delayOffset = _delayCount - _timerTask.DelayTime;
        // 延时时间已结束
        if (delayOffset >= 0) {
            _rateCount = delayOffset;
            _endCount = delayOffset;
            // 调用回调
            if (_timerTask.DelayCallback != null) {
                _timerTask.DelayCallback();
                _timerTask.DelayCallback = null;
            }

            _timerState = TimerState.Normal;
            NormalTimerHandler(0);
        }
    }

    private void NormalTimerHandler(float delta) {
        if (_timerTask.RateTime > 0) {
            _rateCount += delta;
            float rateOffset = _rateCount - _timerTask.RateTime;
            if (rateOffset >= 0) {
                _rateCount = rateOffset;
                _timerTask.RateCallback?.Invoke();
            }
        }

        if (!Mathf.Approximately(_timerTask.EndTime, -1)) {
            _endCount += delta;
            float endOffset = _endCount - _timerTask.EndTime;
            if (endOffset >= 0) {
                _timerTask.EndCallback?.Invoke();
                OnDisable();
            }
        }
    }

    private void OnDisable() {
        _isRun = false;
        _timerTask = null;
        _timerState = TimerState.None;
        _delayCount = 0;
        _rateCount = 0;
        _endCount = 0;
    }
}