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
        
        // 显示UI
        Show();
        characterEl.gameObject.SetActive(true);
    }

    /// <summary>
    /// 设置卡牌剩余数量
    /// </summary>
    /// <param name="num">数量</param>
    public void SetCardStack(int num) {
        cardStackTextEl.text = num.ToString();
        // 显示卡牌剩余数量
        if (!cardStackImageEl.gameObject.activeSelf) {
            cardStackImageEl.gameObject.SetActive(true);
        }
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
            cardStackTextEl.text = "20";
            landlordIconEl.gameObject.SetActive(true);
            
            // 地主图片从屏幕中间移动到昵称上面
            var component = GetOrAddComponent<RectPosTween>(landlordIconEl.gameObject);
            component.MoveLocalPosTime(1.6f, new Vector3(-605, 0, 0));
        } else {
            landlordIconEl.gameObject.SetActive(false);
        }
    }
}