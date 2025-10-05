using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 快捷聊天项
/// </summary>
public class ChatItem : UIBase {
    
    [SerializeField, Header("快捷聊天内容")] private Text chatText;
    
    protected override void Init() {
    }

    /// <summary>
    /// 设置聊天快捷项内容
    /// </summary>
    public void SetText(string chat) {
        chatText.text = chat;
    }
}