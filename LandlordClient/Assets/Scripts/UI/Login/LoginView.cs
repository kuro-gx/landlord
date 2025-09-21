using Google.Protobuf;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginView : UIBase {
    public static LoginView Instance;

    [SerializeField, Header("登录窗口")] private LoginPanel _loginPanel;
    [SerializeField, Header("登录窗口")] private RegisterPanel _registerPanel;
    [SerializeField, Header("Loading窗口")] private LoadingPanel _loadingPanel;

    [SerializeField, Header("登录按钮")] private Button _loginBtn;
    [SerializeField, Header("注册按钮")] private Button _registerBtn;

    protected void Awake() {
        Instance = this;
    }

    public override void Init() {
        // 监听服务器返回的注册 & 登录结果
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_RegisterCode, OnRegisterHandle);
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_LoginCode, OnLoginHandle);
        
        _loginBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            _loginPanel.Show();
        });
        _registerBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            _registerPanel.Show();
        });
    }

    /// <summary>
    /// 登录结果回调
    /// </summary>
    private void OnLoginHandle(ByteString data) {
        _loadingPanel.Show(false);
        LoginResponse res = LoginResponse.Parser.ParseFrom(data);
        if (res == null) {
            return;
        }

        switch (res.Code) {
            case ResultCode.Success:
                // ShowSystemTips("登录成功!", Color.green);
                Global.LoginUser = res;
                SceneManager.LoadScene("MainScene");
                break;
            case ResultCode.PasswordNotBlank:
                ShowSystemTips("密码不能为空!", Color.red);
                break;
            case ResultCode.MobileNotBlank:
                ShowSystemTips("手机号不能为空!", Color.red);
                break;
            case ResultCode.AccountNotExist:
                ShowSystemTips("该手机号尚未注册!", Color.red);
                break;
            case ResultCode.PasswordError:
                ShowSystemTips("账号或密码错误!", Color.red);
                break;
            case ResultCode.AccountTerminate:
                ShowSystemTips("账号已被封禁!", Color.red);
                break;
            default:
                ShowSystemTips("未知错误!", Color.red);
                break;
        }
    }

    /// <summary>
    /// 注册结果回调
    /// </summary>
    private void OnRegisterHandle(ByteString data) {
        _loadingPanel.Show(false);
        Result res = Result.Parser.ParseFrom(data);
        switch (res.Code) {
            case ResultCode.Success: {
                ShowSystemTips("注册成功!", Color.green);
                _registerPanel.Show(false);
                _loginPanel.Show();
                break;
            }
            case ResultCode.MobileNotBlank:
                ShowSystemTips("手机号不能为空!", Color.red);
                break;
            case ResultCode.PasswordNotBlank:
                ShowSystemTips("密码不能为空!", Color.red);
                break;
            case ResultCode.SmsCodeError:
                ShowSystemTips("短信验证码错误!", Color.red);
                break;
            case ResultCode.AccountExist:
                ShowSystemTips("该手机号已被注册!", Color.red);
                break;
            case ResultCode.ServerError:
                ShowSystemTips("服务器异常!", Color.red);
                break;
        }
    }

    /// <summary>
    /// 显示加载面板
    /// </summary>
    public void ShowLoading() {
        _loadingPanel.Show();
    }
}