using System.Text.RegularExpressions;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : UIBase {
    [SerializeField, Header("手机号输入框")] private InputField mobileInput;
    [SerializeField, Header("密码输入框")] private InputField pwdInput;

    [SerializeField, Header("登录按钮")] private Button loginBtn;
    [SerializeField, Header("关闭按钮")] private Button closeBtn;

    public override void Init() {
        // 填充账号密码
        mobileInput.text = "13000000001";
        pwdInput.text = "123456";
        
        loginBtn.onClick.AddListener(OnLoginBtnClicked);
        closeBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            Show(false);
        });
    }

    private void OnDestroy() {
        loginBtn.onClick.RemoveListener(OnLoginBtnClicked);
        closeBtn.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 登录按钮点击事件
    /// </summary>
    private void OnLoginBtnClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);

        if (string.IsNullOrEmpty(mobileInput.text.Trim())) {
            ShowSystemTips("手机号不能为空!", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(pwdInput.text.Trim())) {
            ShowSystemTips("密码不能为空!", Color.red);
            return;
        }

        string pattern = @"^1[3-9]\d{9}$";
        if (!Regex.IsMatch(mobileInput.text.Trim(), pattern)) {
            ShowSystemTips("手机号不合法!", Color.red);
            return;
        }

        LoginBo form = new LoginBo {
            Mobile = mobileInput.text.Trim(),
            Password = pwdInput.text.Trim()
        };

        // 发送登录请求
        NetSocketMgr.Client.SendData(NetDefine.CMD_LoginCode, form.ToByteString());
        // 显示Loading
        LoginView.Instance.ShowLoading();
    }
}