using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 右下角面板
/// </summary>
public class GameBottomPanel : UIBase {
    [SerializeField, Header("倍数文本")] private Text multipleTextEl;
    [SerializeField, Header("聊天按钮")] private Button chatBtnEl;
    [SerializeField, Header("聊天面板")] private ChatPanel chatPanel;

    public override void Init() {
        chatBtnEl.onClick.AddListener(ChatBtnClicked);
    }

    /// <summary>
    /// 聊天按钮点击事件
    /// </summary>
    private void ChatBtnClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        // 显示 or 隐藏聊天快捷面板
        chatPanel.Show(!chatPanel.gameObject.activeSelf);
    }

    /// <summary>
    /// 设置倍数
    /// </summary>
    public void SetMultipleText(int multiple) {
        multipleTextEl.text = multiple.ToString();
    }
}