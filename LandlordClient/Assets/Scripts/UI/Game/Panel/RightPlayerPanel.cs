using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 右侧玩家面板
/// </summary>
public class RightPlayerPanel : UIBase {
    [SerializeField, Header("昵称")] private Text _usernameEl;
    [SerializeField, Header("豆子")] private Text _moneyEl;
    [SerializeField, Header("剩余卡牌")] private Image _cardStackEl;
    [SerializeField, Header("地主图片标识")] private Image _landlordIconEl;
    [SerializeField, Header("人物形象")] private Image _characterEl;
    [SerializeField, Header("聊天气泡")] private Image _chatContainerEl;
    [SerializeField, Header("提示文字")] private Image _readyTextEl;

    public override void Init() {
    }
    
    /// <summary>
    /// 更新UI信息
    /// </summary>
    /// <param name="playerInfo">玩家信息</param>
    public void RefreshPanel(Player playerInfo) {
        if (playerInfo == null) {
            return;
        }
        
        _usernameEl.text = playerInfo.Username;
        _moneyEl.text = playerInfo.Money.ToString();
        _characterEl.sprite = Resources.Load<Sprite>("Character/Tex_1" + playerInfo.Pos);
        
        // 显示UI
        _usernameEl.gameObject.SetActive(true);
        _moneyEl.gameObject.SetActive(true);
        _characterEl.gameObject.SetActive(true);
    }
}