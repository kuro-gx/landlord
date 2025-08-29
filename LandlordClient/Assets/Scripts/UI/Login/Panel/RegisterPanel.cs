using System.Text.RegularExpressions;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : UIBase {
    [SerializeField, Header("手机号输入框")] private InputField _mobileInput;
    [SerializeField, Header("短信验证码")] private InputField _smsInput;
    [SerializeField, Header("密码输入框")] private InputField _pwdInput;
    [SerializeField, Header("重复密码输入框")] private InputField _repwdInput;

    [SerializeField, Header("发送验证码按钮")] private Button _sendCodeBtn;
    [SerializeField, Header("注册按钮")] private Button _registerBtn;
    [SerializeField, Header("关闭按钮")] private Button _closeBtn;

    public override void Init() {
        _sendCodeBtn.onClick.AddListener(() => { ShowSystemTips("验证码已发送", Color.green); });
        _registerBtn.onClick.AddListener(OnRegisterBtnClicked);
        _closeBtn.onClick.AddListener(() => { Show(false); });
    }

    private void OnDestroy() {
        _registerBtn.onClick.RemoveListener(OnRegisterBtnClicked);
        _sendCodeBtn.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 注册按钮点击事件
    /// </summary>
    private void OnRegisterBtnClicked() {
        if (string.IsNullOrEmpty(_mobileInput.text.Trim())) {
            ShowSystemTips("手机号不能为空!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(_smsInput.text.Trim())) {
            ShowSystemTips("短信验证码不能为空!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(_pwdInput.text.Trim())) {
            ShowSystemTips("密码不能为空!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(_repwdInput.text.Trim())) {
            ShowSystemTips("确认密码不能为空!", Color.red);
            return;
        }

        if (_pwdInput.text.Length > 16 || _pwdInput.text.Length < 6) {
            ShowSystemTips("密码长度须为6-16位!", Color.red);
            return;
        }

        if (!_pwdInput.text.Equals(_repwdInput.text)) {
            ShowSystemTips("两次密码不一致!", Color.red);
            return;
        }

        string pattern = @"^1[3-9]\d{9}$";
        if (!Regex.IsMatch(_mobileInput.text.Trim(), pattern)) {
            ShowSystemTips("手机号不合法!", Color.red);
            return;
        }

        RegisterBo form = new RegisterBo() {
            Mobile = _mobileInput.text.Trim(),
            SmsCode = _smsInput.text.Trim(),
            Password = _pwdInput.text.Trim()
        };
        // 发送注册请求
        NetSocketMgr.Client.SendData(NetDefine.CMD_RegisterCode, form.ToByteString());
        // 显示Loading
        LoginView.Instance.ShowLoading();
    }
}