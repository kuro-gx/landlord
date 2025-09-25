using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 左上角用户信息面板
/// </summary>
public class InfoPanel : UIBase {
    [SerializeField, Header("退出登录按钮")] private Button logoutBtn;
    [SerializeField, Header("用户头像")] private Button avatarEl;
    [SerializeField, Header("用户昵称")] private Text usernameEl;
    [SerializeField, Header("设置信息面板")] private SetInfoPanel setInfoPanel;

    public override void Init() {
        // 退出登录
        logoutBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            Global.LoginUser = null;
            // 返回登录界面
            SceneManager.LoadScene("LoginScene");
        });
        
        // 点击用户头像，显示设置用户信息面板
        avatarEl.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            setInfoPanel.Show();
        });
    }

    /// <summary>
    /// 设置用户名
    /// </summary>
    /// <param name="username">用户名</param>
    public void SetUsername(string username) {
        usernameEl.text = username;
    }

    private void OnDestroy() {
        logoutBtn.onClick.RemoveAllListeners();
        avatarEl.onClick.RemoveAllListeners();
    }
}