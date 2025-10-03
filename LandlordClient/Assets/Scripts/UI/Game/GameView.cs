using System.Collections;
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

    private int _leftPosIndex = -1; // 左边玩家坐位索引
    private int _rightPosIndex = -1; // 右边玩家的坐位索引
    private int _selfPosIndex = -1; // 自己的坐位索引
    private Player _leftPlayerInfo; // 左边玩家信息
    private Player _selfPlayerInfo; // 自己的信息
    private Player _rightPlayerInfo; // 右边玩家信息

    protected override void Init() {
        // 监听服务器返回的匹配结果
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_MatchCode, OnMatchHandle);
        // 监听服务器响应的发牌消息
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_DispatchCardCode, OnDispatchCardHandle);
        // 监听服务器响应的叫地主消息
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_CallLordCode, OnCallLordHandle);
        // 监听服务器响应的抢地主消息
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_GrabLordCode, OnGrabLordHandle);
        // 监听服务器响应的玩家成为地主消息
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_BecomeLordCode, OnBecomeLordHandle);
        // 监听服务器响应的加倍消息
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_RaiseCode, OnRaiseHandle);
        // 监听服务器响应的出牌消息
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_PlayHandCode, OnPlayHandHandle);
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
        selfPlayerPanel.DestroyCardPanelList();
        // 隐藏玩家的提示文字
        leftPlayerPanel.ChangeTipVisibility(null, false);
        rightPlayerPanel.ChangeTipVisibility(null, false);
        selfPlayerPanel.ChangeTipVisibility(null, "TipText", false);
        // 创建手牌预制体列表
        selfPlayerPanel.InitCardPanelList(response.CardList.ToList());

        // 显示左右玩家手牌数量
        leftPlayerPanel.SetCardStack(0);
        rightPlayerPanel.SetCardStack(0);

        // 动画显示手牌
        StartCoroutine(DelayMoveCard());
    }

    /// <summary>
    /// 处理服务器响应的叫地主消息
    /// </summary>
    private void OnCallLordHandle(ByteString data) {
        var response = CallLordResponse.Parser.ParseFrom(data);
        var tipAndAudioName = response.IsCall ? Constant.CallLord : Constant.NotCall;
        // 播放音频
        AudioService.Instance.PlayOperateAudio(response.LastPos, tipAndAudioName);

        // 显示上一个叫地主玩家的操作提示
        if (response.LastPos == _leftPosIndex) {
            // 显示其选择并关闭倒计时
            leftPlayerPanel.ChangeTipVisibility(tipAndAudioName);
            leftPlayerPanel.HideClock();
            // 上个叫地主的玩家是左侧玩家，且左侧玩家选择“不叫”，则轮到自己叫地主
            if (!response.IsCall) {
                buttonsPanel.ShowCallLordBtnPage(false);
            } else {
                // 左侧玩家叫地主，进入抢地主环节
                GameStateChangedHandle(GameState.GrabLord);
            }
        } else if (response.LastPos == _selfPosIndex) {
            selfPlayerPanel.ChangeTipVisibility(tipAndAudioName);
            // 如果上一个叫地主的玩家是自己，则现在轮到右侧玩家操作，显示右侧玩家的倒计时
            rightPlayerPanel.SetClockHandle(Constant.CallLordCountDown);
        } else {
            // 显示其选择并关闭倒计时
            rightPlayerPanel.ChangeTipVisibility(tipAndAudioName);
            rightPlayerPanel.HideClock();
            // 如果上一个叫地主的玩家是右侧玩家，则选择轮到左侧玩家操作，显示左侧玩家的倒计时
            leftPlayerPanel.SetClockHandle(Constant.CallLordCountDown);
        }
    }

    /// <summary>
    /// 处理服务器响应的抢地主消息
    /// </summary>
    private void OnGrabLordHandle(ByteString data) {
        var response = GrabLordResponse.Parser.ParseFrom(data);
        var tipImageName = response.IsGrab ? "Grab" : Constant.NotGrab;
        // 播放音频
        AudioService.Instance.PlayOperateAudio(response.LastPos,
            response.IsGrab ? $"Grab{response.GrabTimes}" : Constant.NotGrab);

        // 显示上一个抢地主玩家的操作提示
        if (response.LastPos == _leftPosIndex) {
            leftPlayerPanel.ChangeTipVisibility(tipImageName);
            leftPlayerPanel.HideClock();
        } else if (response.LastPos == _selfPosIndex) {
            selfPlayerPanel.ChangeTipVisibility(tipImageName);
            // 下一个抢地主的玩家是右侧玩家
            if (response.CanGrabPos == _rightPosIndex) {
                rightPlayerPanel.SetClockHandle(Constant.CallLordCountDown);
            }
        } else {
            rightPlayerPanel.ChangeTipVisibility(tipImageName);
            rightPlayerPanel.HideClock();
            // 下一个抢地主的玩家是左侧玩家
            if (response.CanGrabPos == _leftPosIndex) {
                leftPlayerPanel.SetClockHandle(Constant.CallLordCountDown);
            }
        }

        // 设置倍数
        gameBottomPanel.SetMultipleText(response.Multiple);

        // 如果没人抢地主
        if (response.CanGrabPos == -1) {
            // 隐藏抢地主按钮组
            buttonsPanel.ShowCallLordBtnPage(true, false);
        }

        // 如果还能抢地主的玩家是自己，则显示抢地主按钮页
        if (response.CanGrabPos == _selfPosIndex) {
            buttonsPanel.ShowCallLordBtnPage(true);
            // 倒计时结束，发送“不抢”请求并隐藏按钮页
            buttonsPanel.SetClockCallback(Constant.CallLordCountDown, GameState.GrabLord);
        }
    }

    /// <summary>
    /// 处理服务器响应的玩家成为地主消息
    /// </summary>
    private void OnBecomeLordHandle(ByteString data) {
        var response = BecomeLordResponse.Parser.ParseFrom(data);
        var list = response.HoleCards.ToList();
        // 显示底牌
        pocketCardPanel.SetPocketCard(list);
        // 隐藏叫/抢地主的提示文字
        leftPlayerPanel.ChangeTipVisibility(null, false);
        rightPlayerPanel.ChangeTipVisibility(null, false);
        selfPlayerPanel.ChangeTipVisibility(null, "TipText", false);

        // 显示地主图标
        if (response.LordPos == _leftPosIndex) {
            leftPlayerPanel.ChangeLordIconVisibility();
        } else if (response.LordPos == _rightPosIndex) {
            rightPlayerPanel.ChangeLordIconVisibility();
        } else {
            selfPlayerPanel.ChangeLordIconVisibility();
            // 自己成为地主
            selfPlayerPanel.BecomeLord(list);
        }

        // 游戏进入加倍状态
        GameStateChangedHandle(GameState.Raise);
    }

    /// <summary>
    /// 处理服务器响应的加倍消息
    /// </summary>
    private void OnRaiseHandle(ByteString data) {
        var response = RaiseResponse.Parser.ParseFrom(data);
        var tipAndAudioName = response.IsRaise ? Constant.Raise : Constant.NotRaise;
        AudioService.Instance.PlayOperateAudio(response.LastPos, tipAndAudioName);

        if (response.LastPos == _leftPosIndex) {
            leftPlayerPanel.ChangeTipVisibility(tipAndAudioName);
        } else if (response.LastPos == _selfPosIndex) {
            selfPlayerPanel.ChangeTipVisibility(tipAndAudioName);
        } else {
            rightPlayerPanel.ChangeTipVisibility(tipAndAudioName);
        }

        // 设置新的倍数
        gameBottomPanel.SetMultipleText(response.Multiple);

        // 所有人都选择过后，隐藏文字提示
        if (response.CanRaise) return;
        leftPlayerPanel.ChangeTipVisibility(null, false);
        rightPlayerPanel.ChangeTipVisibility(null, false);
        selfPlayerPanel.ChangeTipVisibility(null, "TipText", false);
        // 游戏进入出牌阶段
        GameStateChangedHandle(GameState.PlayingHand);
        // 如果自己是地主，则显示出牌按钮页面
        if (response.LordPos == _selfPosIndex) {
            buttonsPanel.ShowPlayHandBtnPage(PlayHandBtnEnum.OnlyPlayHand);
        }
    }

    /// <summary>
    /// 处理服务器响应的出牌消息
    /// </summary>
    private void OnPlayHandHandle(ByteString data) {
        var response = PlayHandResponse.Parser.ParseFrom(data);
        if (response.LastPos == _leftPosIndex) {
            OtherPlayHandHandle(true, response);
            // 出牌人是左侧玩家，处理完其逻辑后，轮到自己出牌
            buttonsPanel.ShowPlayHandBtnPage(PlayHandBtnEnum.ShowAll);
            buttonsPanel.SetClockCallback(Constant.PlayHandCountDown, GameState.PlayingHand);
        } else if (response.LastPos == _rightPosIndex) {
            OtherPlayHandHandle(false, response);
            // 显示左侧玩家的倒计时
            rightPlayerPanel.HideClock();
            if (!response.IsEnd) leftPlayerPanel.SetClockHandle(Constant.PlayHandCountDown);
        } else {
            // 出牌人是自己，销毁左侧玩家打出的牌，显示自己打出的牌
            leftPlayerPanel.DestroyCardDisplay();
            selfPlayerPanel.ShowCardDisplay();
            // 清空选中的卡牌
            selfPlayerPanel.PrepareCardList.Clear();
            // todo 播放音频

            // 删除打出的牌
            selfPlayerPanel.SelfCardList.RemoveAll(c => response.PendCards.Contains(c));
            // 删除打出的牌的预制体
            for (var i = selfPlayerPanel.SelfCardPanelList.Count - 1; i >= 0; i--) {
                foreach (var c in response.PendCards) {
                    if ((int)c.Point * 10 + (int)c.Suit == selfPlayerPanel.SelfCardPanelList[i].cardValue) {
                        Destroy(selfPlayerPanel.SelfCardPanelList[i].gameObject);
                        selfPlayerPanel.SelfCardPanelList.RemoveAt(i);
                        break;
                    }
                }
            }

            // 重新设置手牌的位置
            selfPlayerPanel.ResetCardPosition();
            // 显示右侧玩家的倒计时
            leftPlayerPanel.HideClock();
            if (!response.IsEnd) rightPlayerPanel.SetClockHandle(Constant.PlayHandCountDown);
        }

        // 更新倍数
        gameBottomPanel.SetMultipleText(response.Multiple);

        // todo 如果游戏结束
        if (response.IsEnd) {
            // todo 春天了，显示动画
            if (response.IsSpring) {
            } else {
                // todo 正常结算
            }
        }
    }

    /// <summary>
    /// 移动卡牌的动画
    /// </summary>
    IEnumerator DelayMoveCard() {
        AudioService.Instance.PlayEffectAudio(Constant.CardDispatch);
        foreach (var panel in selfPlayerPanel.SelfCardPanelList) {
            panel.Show();
            panel.MoveLocalPosInTime(Constant.CardMoveDelay, new Vector3(Constant.CardHDistance, 0, 0));

            yield return new WaitForSeconds(Constant.CardMoveDelay);
        }

        // 动画结束后切换游戏状态为'叫地主'，0号位置的玩家先叫地主
        if (_selfPosIndex == 0) {
            GameStateChangedHandle(GameState.CallLord);
        } else if (_leftPosIndex == 0) {
            leftPlayerPanel.SetClockHandle(Constant.CallLordCountDown);
        } else if (_rightPosIndex == 0) {
            rightPlayerPanel.SetClockHandle(Constant.CallLordCountDown);
        }
    }

    /// <summary>
    /// 游戏状态改变处理
    /// </summary>
    /// <param name="state">状态</param>
    private void GameStateChangedHandle(GameState state) {
        switch (state) {
            case GameState.CallLord:
                buttonsPanel.ShowCallLordBtnPage(false);
                buttonsPanel.SetClockCallback(Constant.CallLordCountDown, GameState.CallLord);
                break;
            case GameState.GrabLord:
                buttonsPanel.ShowCallLordBtnPage(true);
                buttonsPanel.SetClockCallback(Constant.CallLordCountDown, GameState.GrabLord);
                break;
            case GameState.Raise:
                buttonsPanel.ShowRaiseBtnPage();
                buttonsPanel.SetClockCallback(Constant.RaiseCountDown, GameState.Raise);
                break;
            case GameState.PlayingHand:
                // 设置selfPlayerPanel的游戏状态为PlayingHand，此状态可以选择卡牌
                selfPlayerPanel.GameState = GameState.PlayingHand;
                buttonsPanel.SetClockCallback(Constant.PlayHandCountDown, GameState.PlayingHand);
                break;
        }
    }

    /// <summary>
    /// 左右玩家出牌的处理
    /// </summary>
    /// <param name="isLeft">是否为左侧玩家</param>
    /// <param name="response">出牌响应</param>
    private void OtherPlayHandHandle(bool isLeft, PlayHandResponse response) {
        // 播放不出牌的音效
        if (response.IsPass) {
            AudioService.Instance.PlayOperateAudio(
                isLeft ? _leftPosIndex : _rightPosIndex, Constant.PassArr[Random.Range(0, 4)]);
        }

        // 设置剩余卡牌，显示左右玩家打出的牌
        int remainingNum;
        if (isLeft) {
            // 销毁右侧玩家打出的牌
            rightPlayerPanel.DestroyCardDisplay();
            leftPlayerPanel.ShowCardDisplay(response.PendCards.ToList());
            remainingNum = leftPlayerPanel.SetCardStack(response.PendCards.Count * -1);
        } else {
            rightPlayerPanel.ShowCardDisplay(response.PendCards.ToList());
            remainingNum = rightPlayerPanel.SetCardStack(response.PendCards.Count * -1);;
        }
        // 剩余牌数量报警或者播放打出的牌
        if (remainingNum is > 0 and < 3) {
            AudioService.Instance.PlayOperateAudio(isLeft ? _leftPosIndex : _rightPosIndex,
                Constant.Alarm + remainingNum);
        } else {
            // todo 播放音频
        }

        // 记录对手打出的牌，客户端出牌比较大小使用
        selfPlayerPanel.PendCardList.Clear();
        selfPlayerPanel.PendCardList.AddRange(response.PendCards);
        // 销毁自己打出的牌
        selfPlayerPanel.DestroyCardDisplay();
    }
}