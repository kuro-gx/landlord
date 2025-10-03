using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 左侧玩家面板
/// </summary>
public class LeftPlayerPanel : UIBase {
    [SerializeField, Header("昵称")] private Text usernameEl;
    [SerializeField, Header("豆子")] private Text moneyEl;
    [SerializeField, Header("手牌背景")] private Image cardStackImageEl;
    [SerializeField, Header("剩余卡牌")] private Text cardStackTextEl;
    [SerializeField, Header("地主图片标识")] private Image landlordIconEl;
    [SerializeField, Header("人物形象")] private Image characterEl;
    [SerializeField, Header("聊天气泡")] private Image chatContainerEl;
    [SerializeField, Header("提示文字")] private Image tipTextEl;
    [SerializeField, Header("倒计时图片")] private Image clockImageEl;
    [SerializeField, Header("倒计时文字")] private Text clockTextEl;
    [SerializeField, Header("出牌区域")] private RectTransform playHandArea;

    // 初始卡牌数量
    private int _baseCardNum = 17;
    // 打出的牌的预制体列表
    private readonly List<CardDisplay> _displayCards = new(20);
    // 定时器
    private TimerUtil _timer;

    protected override void Init() {
    }
    
    /// <summary>
    /// 更新UI信息
    /// </summary>
    /// <param name="playerInfo">玩家信息</param>
    public void RefreshPanel(Player playerInfo) {
        if (playerInfo == null) {
            return;
        }
        
        usernameEl.text = playerInfo.Username;
        moneyEl.text = playerInfo.Money.ToString();
        characterEl.sprite = Resources.Load<Sprite>("Character/Tex_1" + playerInfo.Pos);
        _baseCardNum = 17;
        
        // 显示UI
        Show();
        characterEl.gameObject.SetActive(true);
    }

    /// <summary>
    /// 设置卡牌剩余数量
    /// </summary>
    /// <param name="num">在17张的基础上相加的值</param>
    /// <returns>剩余卡牌</returns>
    public int SetCardStack(int num) {
        _baseCardNum += num;
        cardStackTextEl.text = _baseCardNum.ToString();
        // 显示卡牌剩余数量
        if (!cardStackImageEl.gameObject.activeSelf) {
            cardStackImageEl.gameObject.SetActive(true);
        }
        
        return _baseCardNum;
    }
    
    
    /// <summary>
    /// 显示or隐藏提示消息
    /// </summary>
    public void ChangeTipVisibility(string tipImageName, bool visibility = true) {
        if (tipImageName != null) {
            tipTextEl.sprite = Resources.Load<Sprite>("TipText/" + tipImageName);
            tipTextEl.SetNativeSize();
        }
        tipTextEl.gameObject.SetActive(visibility);
    }
    
    /// <summary>
    /// 显示 or 隐藏地主图标
    /// </summary>
    public void ChangeLordIconVisibility(bool visibility = true) {
        // 缩放显示，普通隐藏
        if (visibility) {
            // 如果是显示地主图标，说明玩家成为了地主，设置手牌剩余数量
            SetCardStack(3);
            landlordIconEl.gameObject.SetActive(true);
            
            // 地主图片从屏幕中间移动到昵称上面
            var component = GetOrAddComponent<RectPosTween>(landlordIconEl.gameObject);
            component.MoveLocalPosInTime(1.6f, new Vector3(-605, 0, 0));
        } else {
            landlordIconEl.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 显示打出的牌
    /// </summary>
    /// <param name="cards">打出的牌</param>
    public void ShowCardDisplay(List<Card> cards) {
        _displayCards.Clear();

        for (var i = 0; i < cards.Count; i++) {
            var cardDisplay = Resources.Load<CardDisplay>("Prefabs/CardDisplay");
            if (cardDisplay == null) {
                return;
            }
            
            var panel = Instantiate(cardDisplay, playHandArea);
            panel.SetCardInfo(cards[i]);
            // 起始坐标：(-170,65)，第11张开始坐标变为左:(-170,0)
            if (i < 10) {
                panel.transform.localPosition = new Vector2(0 + i * 30, -15);
            } else {
                panel.transform.localPosition = new Vector2(0 + (i - 10) * 30, -80);
            }
            _displayCards.Add(panel);
        }
    }
    
    /// <summary>
    /// 删除打出的卡牌的预制体
    /// </summary>
    public void DestroyCardDisplay() {
        if (_displayCards.Count == 0) {
            return;
        }

        foreach (var display in _displayCards) {
            Destroy(display.gameObject);
        }

        _displayCards.Clear();
    }
    
    /// <summary>
    /// 倒计时处理
    /// </summary>
    /// <param name="time">倒计时秒数</param>
    public void SetClockHandle(int time) {
        // 显示定时器
        clockImageEl.gameObject.SetActive(true);
        clockTextEl.text = time.ToString();

        // 每隔1秒修改闹钟内文字
        int tmpCountDown = time;
        var timerTask = new TimerTask {
            RateTime = 1,
            RateCallback = () => {
                tmpCountDown -= 1;
                clockTextEl.text = tmpCountDown.ToString();
            },
            EndTime = tmpCountDown
        };

        _timer = GetOrAddComponent<TimerUtil>(clockTextEl.gameObject);
        timerTask.EndCallback = HideClock;
        if (_timer) _timer.AddTimerTask(timerTask);
    }

    /// <summary>
    /// 隐藏定时器
    /// </summary>
    public void HideClock() {
        clockImageEl.gameObject.SetActive(false);
        if (_timer) {
            _timer.RemoveTimerTask();
        }
    }
}