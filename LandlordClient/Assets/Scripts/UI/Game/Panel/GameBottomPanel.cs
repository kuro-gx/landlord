using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 右下角面板
/// </summary>
public class GameBottomPanel : UIBase {
    [SerializeField, Header("倍数文本")] private Text _multipleText;
    [SerializeField, Header("聊天按钮")] private Button _chatBtn;
    [SerializeField, Header("聊天面板")] private ChatPanel _chatPanel;

    public override void Init() {
        _chatBtn.onClick.AddListener(ChatBtnClicked);
    }

    /// <summary>
    /// 聊天按钮点击事件
    /// </summary>
    private void ChatBtnClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        // 显示 or 隐藏聊天快捷面板
        _chatPanel.Show(!_chatPanel.gameObject.activeSelf);
    }
}