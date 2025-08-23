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
            if (string.IsNullOrEmpty(_mobileInput.text)) {
                return;
            }

            if (string.IsNullOrEmpty(_smsInput.text)) {
                return;
            }

            if (string.IsNullOrEmpty(_pwdInput.text)) {
                return;
            }

            if (string.IsNullOrEmpty(_repwdInput.text)) {
                return;
            }

            if (!_pwdInput.text.Equals(_repwdInput.text)) {
                return;
            }
        });

        _backBtn.onClick.AddListener(() => {
            Show(false);
            LoginView.Instance.ShowPanel(PanelType.LoginPanel);
        });
    }
}