using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家面板
/// </summary>
public class SelfPlayerPanel : UIBase {
    [SerializeField, Header("昵称")] private Text _usernameEl;
    [SerializeField, Header("豆子")] private Text _moneyEl;
    [SerializeField, Header("地主图片标识")] private Image _landlordIconEl;
    [SerializeField, Header("人物形象")] private Image _characterEl;
    [SerializeField, Header("聊天气泡")] private Image _chatContainerEl;
    [SerializeField, Header("提示文字")] private Image _readyTextEl;

    public override void Init() {
        // 设置用户信息
        if (Global.LoginUser != null) {
            _usernameEl.text = Global.LoginUser.Username;
            _moneyEl.text = Global.LoginUser.Money.ToString();
        }
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
        _characterEl.sprite = Resources.Load<Sprite>("Character/Tex_0" + playerInfo.Pos);
    }
}