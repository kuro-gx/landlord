using System.Text.RegularExpressions;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : UIBase {
    [SerializeField, Header("手机号输入框")] private InputField _mobileInput;
    [SerializeField, Header("密码输入框")] private InputField _pwdInput;

    [SerializeField, Header("登录按钮")] private Button _loginBtn;
    [SerializeField, Header("关闭按钮")] private Button _closeBtn;

    public override void Init() {
        _loginBtn.onClick.AddListener(OnLoginBtnClicked);
        _closeBtn.onClick.AddListener(() => { Show(false); });
    }

    private void OnDestroy() {
        _loginBtn.onClick.RemoveListener(OnLoginBtnClicked);
        _closeBtn.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 登录按钮点击事件
    /// </summary>
    private void OnLoginBtnClicked() {
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
        LoginView.Instance.ShowLoading();
    }
}