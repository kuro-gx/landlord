using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 按钮组面板
/// </summary>
public class ButtonsPanel : UIBase {
    [SerializeField, Header("开始游戏or匹配按钮")] private Button startBtnEl;
    [SerializeField, Header("匹配提示文字")] private Text matchTipEl;
    [SerializeField, Header("出牌按钮页")] private GameObject playHandBtnPage;
    [SerializeField, Header("出牌倒计时文本")] private Text playHandCountDownEl;
    [SerializeField, Header("叫地主按钮页")] private GameObject callLordBtnPage;
    [SerializeField, Header("叫地主按钮")] private Button callLordBtnEl;
    [SerializeField, Header("叫地主按钮文本")] private Text callLordTextEl;
    [SerializeField, Header("叫地主倒计时文本")] private Text callLordCountDownEl;
    [SerializeField, Header("不叫按钮")] private Button notCallLordBtnEl;
    [SerializeField, Header("不叫按钮文本")] private Text notCallLordTextEl;
    [SerializeField, Header("加倍按钮页")] private GameObject raiseBtnPage;

    private readonly string _matchText = "正在速配玩伴"; // 匹配文字
    private readonly float _duration = 1.5f; // 点点出现的时间间隔
    private const string Dots = "...";
    private int _dotIndex = 0; // 当前点点的索引
    private bool _isAnimating;
    private bool _isStartGame;

    public override void Init() {
        // 开始匹配
        startBtnEl.onClick.AddListener(StartMatchClicked);
    }

    /// <summary>
    /// 用户点击“快速开始按钮”，进行匹配
    /// </summary>
    private void StartMatchClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        // 隐藏按钮，显示匹配提示文字
        startBtnEl.gameObject.SetActive(false);
        matchTipEl.gameObject.SetActive(true);
        AnimateDots();

        // 发送匹配请求
        NetSocketMgr.Client.SendData(NetDefine.CMD_MatchCode);
    }

    /// <summary>
    /// 播放匹配提示文字动画
    /// </summary>
    private void StartMatchAnimate() {
        if (_isStartGame) return;
        matchTipEl.text = _matchText;
        _isAnimating = false;
        _dotIndex = 0;
        AnimateDots();
    }

    /// <summary>
    /// 匹配提示文字动画
    /// </summary>
    private void AnimateDots() {
        if (_isAnimating) return; // 如果已经在动画中，则不重复启动
        _isAnimating = true;
        DOTween.To(() => _dotIndex, x => _dotIndex = x, Dots.Length, _duration)
            .SetEase(Ease.Linear) // 使用线性缓动函数
            .OnUpdate(() => matchTipEl.text = $"{_matchText}{Dots.Substring(0, _dotIndex)}")
            .OnComplete(StartMatchAnimate); // 动画完成后重新开始
    }

    /// <summary>
    /// 关闭匹配提示
    /// </summary>
    public void HideMatchTips() {
        _isStartGame = true;
        matchTipEl.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示or隐藏叫地主按钮页
    /// </summary>
    /// <param name="isFirstCall">是否是第一人叫地主</param>
    /// <param name="visibility">显示or隐藏</param>
    public void ChangeCallLordVisibility(bool isFirstCall, bool visibility = true) {
        callLordBtnPage.SetActive(visibility);
        if (isFirstCall) {
            callLordTextEl.text = "叫地主";
            notCallLordTextEl.text = "不叫";
        } else {
            callLordTextEl.text = "抢地主";
            notCallLordTextEl.text = "不抢";
        }
    }

    /// <summary>
    /// 设置倒计时结束的回调
    /// </summary>
    /// <param name="time">倒计时秒数</param>
    /// <param name="state">游戏状态</param>
    /// <param name="callback">回调函数</param>
    public void SetClockCallback(int time, GameState state, Action callback) {
        switch (state) {
            case GameState.CallLord:
                callLordCountDownEl.text = time.ToString();
                break;
            case GameState.Raise:
                break;
            case GameState.PlayingHand:
                playHandCountDownEl.text = time.ToString();
                break;
        }
    }
}