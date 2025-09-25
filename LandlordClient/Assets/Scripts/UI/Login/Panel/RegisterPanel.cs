using System.Text.RegularExpressions;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : UIBase {
    [SerializeField, Header("手机号输入框")] private InputField mobileInput;
    [SerializeField, Header("短信验证码")] private InputField smsInput;
    [SerializeField, Header("密码输入框")] private InputField pwdInput;
    [SerializeField, Header("重复密码输入框")] private InputField rePwdInput;

    [SerializeField, Header("发送验证码按钮")] private Button sendCodeBtn;
    [SerializeField, Header("注册按钮")] private Button registerBtn;
    [SerializeField, Header("关闭按钮")] private Button closeBtn;

    public override void Init() {
        sendCodeBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            ShowSystemTips("验证码已发送", Color.green);
        });
        registerBtn.onClick.AddListener(OnRegisterBtnClicked);
        closeBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            Show(false);
        });
    }

    private void OnDestroy() {
        registerBtn.onClick.RemoveListener(OnRegisterBtnClicked);
        sendCodeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 注册按钮点击事件
    /// </summary>
    private void OnRegisterBtnClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        
        if (string.IsNullOrEmpty(mobileInput.text.Trim())) {
            ShowSystemTips("手机号不能为空!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(smsInput.text.Trim())) {
            ShowSystemTips("短信验证码不能为空!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(pwdInput.text.Trim())) {
            ShowSystemTips("密码不能为空!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(rePwdInput.text.Trim())) {
            ShowSystemTips("确认密码不能为空!", Color.red);
            return;
        }

        if (pwdInput.text.Length > 16 || pwdInput.text.Length < 6) {
            ShowSystemTips("密码长度须为6-16位!", Color.red);
            return;
        }

        if (!pwdInput.text.Equals(rePwdInput.text)) {
            ShowSystemTips("两次密码不一致!", Color.red);
            return;
        }

        string pattern = @"^1[3-9]\d{9}$";
        if (!Regex.IsMatch(mobileInput.text.Trim(), pattern)) {
            ShowSystemTips("手机号不合法!", Color.red);
            return;
        }

        RegisterBo form = new RegisterBo() {
            Mobile = mobileInput.text.Trim(),
            SmsCode = smsInput.text.Trim(),
            Password = pwdInput.text.Trim()
        };
        // 发送注册请求
        NetSocketMgr.Client.SendData(NetDefine.CMD_RegisterCode, form.ToByteString());
        // 显示Loading
        LoginView.Instance.ShowLoading();
    }
}