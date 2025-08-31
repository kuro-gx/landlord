using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 左上角用户信息面板
/// </summary>
public class InfoPanel : UIBase {
    [SerializeField, Header("退出登录按钮")] private Button _logoutBtn;
    [SerializeField, Header("用户头像")] private Button _avatar;
    [SerializeField, Header("设置信息面板")] private SetInfoPanel _setInfoPanel;

    public override void Init() {
        // 退出登录
        _logoutBtn.onClick.AddListener(() => {
            Global.LoginUser = null;
            // 返回登录界面
            SceneManager.LoadScene("LoginScene");
        });
        
        // 点击用户头像，显示设置用户信息面板
        _avatar.onClick.AddListener(() => {
            _setInfoPanel.Show();
        });
    }


    private void OnDestroy() {
        _logoutBtn.onClick.RemoveAllListeners();
        _avatar.onClick.RemoveAllListeners();
    }
}