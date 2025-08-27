using System.Text.RegularExpressions;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : UIBase {
    [SerializeField, Header("手机号输入框")] private InputField _mobileInput;
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
            if (string.IsNullOrEmpty(_mobileInput.text.Trim())) {
                ShowSystemTips("手机号不能为空!", Color.red);
                return;
            }

            if (string.IsNullOrEmpty(_pwdInput.text.Trim())) {
                ShowSystemTips("密码不能为空!", Color.red);
                return;
            }
            
            string pattern = @"^1[3-9]\d{9}$";
            if (!Regex.IsMatch(_mobileInput.text.Trim(), pattern)) {
                ShowSystemTips("手机号不合法!", Color.red);
                return;
            }

            LoginBo form = new LoginBo {
                Mobile = _mobileInput.text.Trim(),
                Password = _pwdInput.text.Trim()
            };
  
            // 发送登录请求
            NetSocketMgr.Client.SendData(NetDefine.CMD_LoginCode, form.ToByteString());
            // 显示Loading
            LoginView.Instance.ShowPanel(PanelType.LoadingPanel);
        });
    }

    private void OnDestroy() {
        _registerBtn.onClick.RemoveAllListeners();
        _loginBtn.onClick.RemoveAllListeners();
    }
}