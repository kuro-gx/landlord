using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : UIBase {
    [SerializeField, Header("账号输入框")] private InputField _mobileInput;
    [SerializeField, Header("密码输入框")] private InputField _pwdInput;
    
    [SerializeField, Header("登录按钮")] private Button _loginBtn;
    [SerializeField, Header("注册按钮")] private Button _registerBtn;
    
    public override void Init() {
        // 点击‘去注册’按钮后，隐藏登录面板，显示注册面板
        _registerBtn.onClick.AddListener(() => {
            Show(false);
            LoginView.Instance.ShowPanel(PanelType.RegisterPanel);
        });
        
        _loginBtn.onClick.AddListener(() => {
            
        });
    }
}