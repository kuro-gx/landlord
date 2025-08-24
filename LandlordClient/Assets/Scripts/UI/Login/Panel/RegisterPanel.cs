using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : UIBase {
    [SerializeField, Header("账号输入框")] private InputField _mobileInput;
    [SerializeField, Header("短信验证码")] private InputField _smsInput;
    [SerializeField, Header("密码输入框")] private InputField _pwdInput;
    [SerializeField, Header("重复密码输入框")] private InputField _repwdInput;

    [SerializeField, Header("发送验证码按钮")] private Button _sendCodeBtn;
    [SerializeField, Header("注册按钮")] private Button _registerBtn;
    [SerializeField, Header("返回登录按钮")] private Button _backBtn;

    public override void Init() {
        _sendCodeBtn.onClick.AddListener(() => { });

        _registerBtn.onClick.AddListener(() => {
            if (string.IsNullOrEmpty(_mobileInput.text.Trim())) {
                ShowSystemTips("账号不能为空!", Color.red);
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

            if (!_pwdInput.text.Equals(_repwdInput.text)) {
                ShowSystemTips("两次密码不一致!", Color.red);
                return;
            }

            RegisterReq param = new RegisterReq() {
                Mobile = _mobileInput.text.Trim(),
                SmsCode = _smsInput.text.Trim(),
                Password = _pwdInput.text.Trim()
            };
            NetSocketMgr.Client.SendData(NetDefine.CMD_RegisterCode, param.ToByteString());
        });

        _backBtn.onClick.AddListener(() => {
            Show(false);
            LoginView.Instance.ShowPanel(PanelType.LoginPanel);
        });
    }

    private void OnDestroy() {
        _registerBtn.onClick.RemoveAllListeners();
        _sendCodeBtn.onClick.RemoveAllListeners();
    }
}