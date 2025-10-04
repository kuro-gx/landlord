using System.Linq;
using DG.Tweening;
using Google.Protobuf;
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
    [SerializeField, Header("不出按钮")] private Button passBtn;
    [SerializeField, Header("提示按钮")] private Button hintBtn;
    [SerializeField, Header("出牌按钮")] private Button playHandBtn;

    [SerializeField, Header("叫地主按钮页")] private GameObject callLordBtnPage;
    [SerializeField, Header("叫地主按钮")] private Button callLordBtnEl;
    [SerializeField, Header("叫地主按钮文本")] private Text callLordTextEl;
    [SerializeField, Header("叫地主倒计时文本")] private Text callLordCountDownEl;
    [SerializeField, Header("不叫按钮")] private Button notCallLordBtnEl;
    [SerializeField, Header("不叫按钮文本")] private Text notCallLordTextEl;

    [SerializeField, Header("加倍按钮页")] private GameObject raiseBtnPage;
    [SerializeField, Header("加倍按钮")] private Button raiseBtn;
    [SerializeField, Header("不加倍按钮")] private Button notRaiseBtn;
    [SerializeField, Header("加倍倒计时文本")] private Text raiseCountDownEl;

    private readonly string _matchText = "正在速配玩伴"; // 匹配文字
    private readonly float _duration = 1.5f; // 点点出现的时间间隔
    private const string Dots = "...";
    private int _dotIndex = 0; // 当前点点的索引
    private bool _isAnimating;
    private bool _isStartGame;
    private TimerUtil _timer;
    private PlayHandBtnEnum _playHandBtnPageState; // 出牌按钮页状态

    protected override void Init() {
        // 开始匹配
        startBtnEl.onClick.AddListener(StartMatchClicked);
        // 叫地主 or 抢地主
        callLordBtnEl.onClick.AddListener(CallLordClicked);
        // 不叫 or 不抢
        notCallLordBtnEl.onClick.AddListener(NotCallLordClicked);
        // 加倍
        raiseBtn.onClick.AddListener(RaiseClicked);
        // 不加倍
        notRaiseBtn.onClick.AddListener(NotRaiseClicked);
        // 出牌
        playHandBtn.onClick.AddListener(PlayHandClicked);
        // 不出
        passBtn.onClick.AddListener(PassClicked);
        // 提示
        hintBtn.onClick.AddListener(() => { });
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
    /// <param name="isGrab">是否为抢地主环节</param>
    /// <param name="isShow">是否显示</param>
    public void ShowCallLordBtnPage(bool isGrab, bool isShow = true) {
        callLordBtnPage.SetActive(isShow);
        if (isGrab) {
            callLordTextEl.text = "抢地主";
            notCallLordTextEl.text = "不抢";
            callLordBtnEl.tag = "grab";
            notCallLordBtnEl.tag = "grab";
        } else {
            callLordTextEl.text = "叫地主";
            notCallLordTextEl.text = "不叫";
            callLordBtnEl.tag = "call";
            notCallLordBtnEl.tag = "call";
        }
    }

    /// <summary>
    /// 倒计时结束的处理
    /// </summary>
    /// <param name="time">倒计时秒数</param>
    /// <param name="state">游戏状态</param>
    public void SetClockCallback(int time, GameState state) {
        SetClockText(time, state);

        // 每隔1秒修改闹钟内文字
        var timerTask = new TimerTask {
            RateTime = 1,
            RateCallback = () => {
                time -= 1;
                SetClockText(time, state);
            },
            EndTime = time
        };

        // 根据游戏状态设置倒计时结束的回调
        switch (state) {
            case GameState.CallLord: {
                // 挂载组件
                _timer = GetOrAddComponent<TimerUtil>(callLordCountDownEl.gameObject);
                timerTask.EndCallback = () => {
                    CallOrGrabLordApi(false, false);
                    callLordBtnPage.SetActive(false);
                    Destroy(_timer);
                    _timer = null;
                };
                break;
            }
            case GameState.GrabLord: {
                _timer = GetOrAddComponent<TimerUtil>(callLordCountDownEl.gameObject);
                timerTask.EndCallback = () => {
                    CallOrGrabLordApi(true, false);
                    callLordBtnPage.SetActive(false);
                    Destroy(_timer);
                    _timer = null;
                };
                break;
            }
            case GameState.Raise: {
                _timer = GetOrAddComponent<TimerUtil>(raiseCountDownEl.gameObject);
                timerTask.EndCallback = NotRaiseClicked;
                break;
            }
            case GameState.PlayingHand: {
                _timer = GetOrAddComponent<TimerUtil>(playHandCountDownEl.gameObject);
                timerTask.EndCallback = () => {
                    // 倒计时结束，自动选择不出 或者 打出一张最小的牌
                    if (_playHandBtnPageState is PlayHandBtnEnum.OnlyPass or PlayHandBtnEnum.ShowAll) {
                        // 放弃出牌
                        PassClicked();
                    } else if (_playHandBtnPageState == PlayHandBtnEnum.OnlyPlayHand) {
                        // 还原卡牌选中状态
                        RestoreCardSelect();
                        SelfPlayerPanel.Instance.SelfCardPanelList.Last().isSelected = true;
                        PlayHandClicked();
                    }

                    _timer.RemoveTimerTask();
                    ShowPlayHandBtnPage(PlayHandBtnEnum.None, false);
                };
                break;
            }
        }

        if (_timer) _timer.AddTimerTask(timerTask);
    }

    /// <summary>
    /// 设置倒计时闹钟倒计时文字
    /// </summary>
    /// <param name="time">剩余秒数</param>
    /// <param name="state">游戏状态</param>
    private void SetClockText(int time, GameState state) {
        switch (state) {
            case GameState.CallLord:
            case GameState.GrabLord:
                callLordCountDownEl.text = time.ToString();
                break;
            case GameState.Raise:
                raiseCountDownEl.text = time.ToString();
                break;
            case GameState.PlayingHand:
                playHandCountDownEl.text = time.ToString();
                break;
        }
    }

    /// <summary>
    /// 叫地主按钮点击事件
    /// </summary>
    private void CallLordClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        if (callLordBtnEl.CompareTag("call")) {
            CallOrGrabLordApi(false, true);
        }

        if (callLordBtnEl.CompareTag("grab")) {
            CallOrGrabLordApi(true, true);
        }

        // 隐藏按钮组
        callLordBtnPage.SetActive(false);
        // 销毁倒计时的组件
        Destroy(_timer);
        _timer = null;
    }

    /// <summary>
    /// 不叫地主点击事件
    /// </summary>
    private void NotCallLordClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        if (callLordBtnEl.CompareTag("call")) {
            CallOrGrabLordApi(false, false);
        }

        if (callLordBtnEl.CompareTag("grab")) {
            CallOrGrabLordApi(true, false);
        }

        // 隐藏按钮组
        callLordBtnPage.SetActive(false);
        // 销毁倒计时的组件
        if (_timer) {
            Destroy(_timer);
            _timer = null;
        }
    }

    /// <summary>
    /// 叫地主 or 抢地主请求
    /// </summary>
    private void CallOrGrabLordApi(bool isGrab, bool param) {
        if (isGrab) {
            var form = new GrabLordBo { Pos = Global.CurPos, IsGrab = param };
            // 发送抢地主请求
            NetSocketMgr.Client.SendData(NetDefine.CMD_GrabLordCode, form.ToByteString());
        } else {
            var form = new CallLordBo { Pos = Global.CurPos, IsCall = param };
            // 发送叫地主请求
            NetSocketMgr.Client.SendData(NetDefine.CMD_CallLordCode, form.ToByteString());
        }
    }

    /// <summary>
    /// 显示or隐藏加倍按钮页面
    /// </summary>
    /// <param name="isShow">是否显示</param>
    public void ShowRaiseBtnPage(bool isShow = true) {
        raiseBtnPage.SetActive(isShow);
    }

    /// <summary>
    /// 显示or隐藏出牌按钮页面
    /// </summary>
    /// <param name="state">显示</param>
    /// <param name="isShow">是否显示</param>
    public void ShowPlayHandBtnPage(PlayHandBtnEnum state, bool isShow = true) {
        playHandBtnPage.SetActive(isShow);
        if (!isShow) return;
        // 隐藏3个按钮
        passBtn.gameObject.SetActive(false);
        hintBtn.gameObject.SetActive(false);
        playHandBtn.gameObject.SetActive(false);
        
        _playHandBtnPageState = state;
        switch (state) {
            case PlayHandBtnEnum.OnlyPass:
                passBtn.gameObject.SetActive(true);
                break;
            case PlayHandBtnEnum.OnlyPlayHand:
                playHandBtn.gameObject.SetActive(true);
                break;
            case PlayHandBtnEnum.ShowAll:
                passBtn.gameObject.SetActive(true);
                hintBtn.gameObject.SetActive(true);
                playHandBtn.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 加倍请求
    /// </summary>
    private void RaiseClicked() {
        var form = new RaiseBo { Pos = Global.CurPos, IsRaise = true };
        NetSocketMgr.Client.SendData(NetDefine.CMD_RaiseCode, form.ToByteString());
        raiseBtnPage.SetActive(false);
        if (_timer) {
            Destroy(_timer);
            _timer = null;
        }
    }

    /// <summary>
    /// 不加倍请求
    /// </summary>
    private void NotRaiseClicked() {
        var form = new RaiseBo { Pos = Global.CurPos, IsRaise = false };
        NetSocketMgr.Client.SendData(NetDefine.CMD_RaiseCode, form.ToByteString());
        raiseBtnPage.SetActive(false);
        if (_timer) {
            Destroy(_timer);
            _timer = null;
        }
    }
    
    /// <summary>
    /// 出牌请求
    /// </summary>
    private void PlayHandClicked() {
        var list = SelfPlayerPanel.Instance.GetSelectedCards();
        if (list.Count == 0) {
            // 显示提示，没有选择任何牌
            SelfPlayerPanel.Instance.ShowTipMessage("5", "Sprites");
            return;
        }

        // 获取牌型
        var playHand = CardMgr.Instance.GetCardType(list);
        if (playHand.Type == CardType.HandUnknown) {
            // 选择的牌不符合规则
            SelfPlayerPanel.Instance.ShowTipMessage("3", "Sprites");
            return;
        }
        
        // 打不过别人的牌
        if (!CardMgr.Instance.CanBeat(SelfPlayerPanel.Instance.OtherPendCards, list)) {
            SelfPlayerPanel.Instance.ShowTipMessage("0", "Sprites");
            return;
        }

        // 隐藏出牌按钮页
        playHandBtnPage.SetActive(false);
        // 隐藏提示
        SelfPlayerPanel.Instance.ShowTipMessage(null, "Sprites", false);
        
        // 发送出牌请求，收到响应后再做剔除手牌等相关操作
        PlayHandRequest(false);
        // 出牌音效
        AudioService.Instance.PlayEffectAudio(Constant.PlayHand);
        if (_timer) {
            _timer.RemoveTimerTask();
        }
    }
    
    /// <summary>
    /// 不出请求
    /// </summary>
    private void PassClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        playHandBtnPage.SetActive(false);
        // 还原卡牌选中状态
        RestoreCardSelect();

        PlayHandRequest(true);
        if (_timer) {
            _timer.RemoveTimerTask();
        }
    }

    /// <summary>
    /// 发送出牌请求
    /// </summary>
    /// <param name="isPass">是否不出</param>
    private void PlayHandRequest(bool isPass) {
        var form = new PlayHandBo {
            Pos = Global.CurPos,
            IsPass = isPass
        };
        if (!isPass) {
            form.CardList.AddRange(SelfPlayerPanel.Instance.GetSelectedCards());
        }
        NetSocketMgr.Client.SendData(NetDefine.CMD_PlayHandCode, form.ToByteString());
    }

    /// <summary>
    /// 让选中的牌往下滑，还原其选中状态
    /// </summary>
    private void RestoreCardSelect() {
        var list = SelfPlayerPanel.Instance.GetSelectedCards();
        var panelList = SelfPlayerPanel.Instance.SelfCardPanelList;
        foreach (var c in list) {
            foreach (var p in panelList) {
                if ((int)c.Point * 10 + (int)c.Suit == p.cardValue) {
                    p.MoveTargetPosInTime(Constant.CardMoveTime, new Vector3(0, Constant.CardVDistance, 0));
                    p.isSelected = false;
                    break;
                }
            }
        }
    }
}