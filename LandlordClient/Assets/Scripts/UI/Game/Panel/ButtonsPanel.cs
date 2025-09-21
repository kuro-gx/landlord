using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 按钮组面板
/// </summary>
public class ButtonsPanel : UIBase {
    [SerializeField, Header("开始游戏or匹配按钮")] private Button _startBtn;
    [SerializeField, Header("取消匹配按钮")] private Button _cancelBtn;
    [SerializeField, Header("匹配提示文字")] private Text _matchTipText;
    [SerializeField, Header("出牌按钮页")] private GameObject _playHandBtnPage;
    [SerializeField, Header("出牌or不出按钮页")] private GameObject _playOrPassBtnPage;
    [SerializeField, Header("叫地主按钮页")] private GameObject _callLordBtnPage;

    private readonly string _matchText = "正在速配玩伴"; // 匹配文字
    private readonly float _duration = 1.5f; // 点点出现的时间间隔
    private const string Dots = "...";
    private int _dotIndex = 0; // 当前点点的索引
    private bool _isAnimating = false;

    public override void Init() {
        // 开始匹配
        _startBtn.onClick.AddListener(StartMatchClicked);
    }

    /// <summary>
    /// 用户点击“快速开始按钮”，进行匹配
    /// </summary>
    private void StartMatchClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        // 隐藏按钮，显示匹配提示文字
        _startBtn.gameObject.SetActive(false);
        _matchTipText.gameObject.SetActive(true);
        AnimateDots();
        
        // 发送匹配请求
        NetSocketMgr.Client.SendData(NetDefine.CMD_MatchCode, null);
    }

    /// <summary>
    /// 播放匹配提示文字动画
    /// </summary>
    private void StartMatchAnimate() {
        _matchTipText.text = _matchText;
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
            .OnUpdate(() => _matchTipText.text = $"{_matchText}{Dots.Substring(0, _dotIndex)}")
            .OnComplete(StartMatchAnimate); // 动画完成后重新开始
    }
}