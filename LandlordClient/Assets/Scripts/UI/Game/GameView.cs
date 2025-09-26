using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using UnityEngine;

public class GameView : UIBase {
    [SerializeField, Header("底牌面板")] private PocketCardPanel pocketCardPanel;
    [SerializeField, Header("游戏界面顶部面板")] private GameTopPanel gameTopPanel;
    [SerializeField, Header("游戏界面底部面板")] private GameBottomPanel gameBottomPanel;
    [SerializeField, Header("按钮组面板")] private ButtonsPanel buttonsPanel;
    [SerializeField, Header("自己的信息面板")] private SelfPlayerPanel selfPlayerPanel;
    [SerializeField, Header("左侧玩家信息面板")] private LeftPlayerPanel leftPlayerPanel;
    [SerializeField, Header("右侧玩家信息面板")] private RightPlayerPanel rightPlayerPanel;
    [SerializeField, Header("手牌区域")] private RectTransform selfCardArea;

    private int _leftPosIndex = -1; // 左边玩家坐位索引
    private int _rightPosIndex = -1; // 右边玩家的坐位索引
    private int _selfPosIndex = -1; // 自己的坐位索引
    private Player _leftPlayerInfo; // 左边玩家信息
    private Player _selfPlayerInfo; // 自己的信息
    private Player _rightPlayerInfo; // 右边玩家信息
    private readonly List<CardPanel> _selfCardPanelList = new(); // 自己的手牌预制体列表
    // private int _robTimes = 0; // 抢地主次数

    public override void Init() {
        // 监听服务器返回的匹配结果
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_MatchCode, OnMatchHandle);
        // 监听服务器响应的发牌消息
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_DispatchCardCode, OnDispatchCardHandle);
        // 监听服务器响应的叫地主消息
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_CallLordCode, OnCallLordHandle);
        // 监听服务器响应的抢地主消息
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_GrabLordCode, OnGrabLordHandle);
    }

    /// <summary>
    /// 处理服务器响应的匹配结果
    /// </summary>
    private void OnMatchHandle(ByteString data) {
        MatchResponse response = MatchResponse.Parser.ParseFrom(data);
        // 根据返回的坐位索引号确认左右玩家
        switch (response.SelfPos) {
            case 0:
                _leftPosIndex = 2;
                _rightPosIndex = 1;
                break;
            case 1:
                _leftPosIndex = 0;
                _rightPosIndex = 2;
                break;
            case 2:
                _leftPosIndex = 1;
                _rightPosIndex = 0;
                break;
        }

        _selfPosIndex = response.SelfPos;
        Global.CurPos = response.SelfPos;
        _selfPlayerInfo = response.Player.ElementAtOrDefault(response.SelfPos);
        _leftPlayerInfo = response.Player.ElementAtOrDefault(_leftPosIndex);
        _rightPlayerInfo = response.Player.ElementAtOrDefault(_rightPosIndex);

        // 更新UI
        selfPlayerPanel.RefreshPanel(_selfPlayerInfo);
        leftPlayerPanel.RefreshPanel(_leftPlayerInfo);
        rightPlayerPanel.RefreshPanel(_rightPlayerInfo);
    }

    /// <summary>
    /// 服务器发牌响应的处理
    /// </summary>
    private void OnDispatchCardHandle(ByteString data) {
        var response = DispatchCardResponse.Parser.ParseFrom(data);
        // 隐藏匹配提示
        buttonsPanel.HideMatchTips();
        // 设置倍数
        gameBottomPanel.SetMultipleText(response.Multiple);
        // 显示底牌背景图
        pocketCardPanel.Show();
        // 销毁旧的卡牌预制体
        foreach (var child in _selfCardPanelList) {
            Destroy(child.gameObject);
        }
        _selfCardPanelList.Clear();
        // 隐藏玩家的提示文字
        leftPlayerPanel.ChangeTipVisibility(null, false);
        rightPlayerPanel.ChangeTipVisibility(null, false);
        selfPlayerPanel.ChangeTipVisibility(null, false);
        // 对手牌进行排序
        var list = response.CardList.ToList();
        list.Sort((a, b) => ((int)b.Point * 10 + (int)b.Suit).CompareTo((int)a.Point * 10 + (int)a.Suit));
        // 手牌距离左侧屏幕的宽度：(屏幕宽度 - ((卡牌数量 - 1) * 卡牌间距 + 卡牌宽度)) / 2
        float cardDistanceWidth = (selfCardArea.rect.width - (Constant.CardDistance * (list.Count - 1) + 135)) / 2;
        // 最左边第一张牌的起始X坐标：出牌区域起始X坐标 + 手牌距离左侧屏幕的宽度
        float firstCardX = selfCardArea.rect.width * -1 / 2 + cardDistanceWidth;
        // 创建卡牌预制体
        for (var i = 0; i < list.Count; i++) {
            CardPanel cardPanel = Resources.Load<CardPanel>("Prefabs/CardPanel");
            if (cardPanel == null) return;
            CardPanel panel = Instantiate(cardPanel, selfCardArea);
            panel.SetCardInfo(list[i]);
            _selfCardPanelList.Add(panel);
            // 设置卡牌之间的间距
            panel.transform.localPosition = new Vector2(firstCardX + i * Constant.CardDistance, -20);
        }

        // 显示左右玩家手牌数量
        leftPlayerPanel.SetCardStack(17);
        rightPlayerPanel.SetCardStack(17);

        // 动画显示手牌
        StartCoroutine(DelayMoveCard());
    }
    
    /// <summary>
    /// 处理服务器响应的叫地主消息
    /// </summary>
    private void OnCallLordHandle(ByteString data) {
        var response = CallLordResponse.Parser.ParseFrom(data);
        var tipAndAudioName = response.IsCall ? "CallLord" : "NotCall";
        // 播放音频
        AudioService.Instance.PlayOperateAudio(response.LastPos, tipAndAudioName);

        // 显示上一个叫地主玩家的操作提示
        if (response.LastPos == _leftPosIndex) {
            leftPlayerPanel.ChangeTipVisibility(tipAndAudioName);
            // 上个叫地主的玩家是左侧玩家，且左侧玩家选择“不叫”，则轮到自己叫地主
            if (!response.IsCall) {
                buttonsPanel.ChangeCallLordBtnPage(false);
            } else {
                // 左侧玩家叫地主，进入抢地主环节
                GameStateChangedHandle(GameState.GrabLord);
            }
        } else if (response.LastPos == _selfPosIndex) {
            selfPlayerPanel.ChangeTipVisibility(tipAndAudioName);
        } else {
            rightPlayerPanel.ChangeTipVisibility(tipAndAudioName);
        }
    }
    
    /// <summary>
    /// 处理服务器响应的抢地主消息
    /// </summary>
    private void OnGrabLordHandle(ByteString data) {
        var response = GrabLordResponse.Parser.ParseFrom(data);
        var tipImageName = response.IsGrab ? "Grab" : "NotGrab";
        // 播放音频
        AudioService.Instance.PlayOperateAudio(response.LastPos, response.IsGrab ? $"Grab{response.GrabTimes}" : "NotGrab");

        // 显示上一个叫地主玩家的操作提示
        if (response.LastPos == _leftPosIndex) {
            leftPlayerPanel.ChangeTipVisibility(tipImageName);
        } else if (response.LastPos == _selfPosIndex) {
            selfPlayerPanel.ChangeTipVisibility(tipImageName);
        } else {
            rightPlayerPanel.ChangeTipVisibility(tipImageName);
        }
        
        // 重新设置倍数
        gameBottomPanel.SetMultipleText(response.Multiple);
        // 如果还能抢地主的坐位是自己，则显示抢地主按钮页
        if (response.CanGrabPos == _selfPosIndex) {
            buttonsPanel.ChangeCallLordBtnPage(true);
            // 倒计时结束，发送“不抢”请求并隐藏按钮页
            buttonsPanel.SetClockCallback(Constant.CallLordCountDown, GameState.GrabLord, () => {
                buttonsPanel.CallOrGrabLordApi(true, false);
                buttonsPanel.ChangeCallLordBtnPage(true, false);
            });
        }
    }

    /// <summary>
    /// 移动卡牌的动画
    /// </summary>
    IEnumerator DelayMoveCard() {
        AudioService.Instance.PlayEffectAudio(Constant.CardDispatch);
        foreach (var panel in _selfCardPanelList) {
            panel.gameObject.SetActive(true);
            panel.MovePosInTime(Constant.CardMoveDelay, new Vector3(Constant.CardDistance, 0, 0));

            yield return new WaitForSeconds(Constant.CardMoveDelay);
        }

        // 动画结束后切换游戏状态为'叫地主'，0号位置的玩家先叫地主
        if (_selfPosIndex == 0) {
            GameStateChangedHandle(GameState.CallLord);
        }
    }

    /// <summary>
    /// 游戏状态改变处理
    /// </summary>
    /// <param name="state">状态</param>
    private void GameStateChangedHandle(GameState state) {
        switch (state) {
            case GameState.CallLord:
                buttonsPanel.ChangeCallLordBtnPage(false);
                buttonsPanel.SetClockCallback(Constant.CallLordCountDown, GameState.CallLord, () => {});
                break;
            case GameState.GrabLord:
                buttonsPanel.ChangeCallLordBtnPage(true);
                buttonsPanel.SetClockCallback(Constant.CallLordCountDown, GameState.GrabLord, () => {});
                break;
            case GameState.Raise:
                break;
            case GameState.PlayingHand:
                break;
        }
    }
}