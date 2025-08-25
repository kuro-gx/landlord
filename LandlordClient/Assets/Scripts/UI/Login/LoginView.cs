using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;

public class LoginView : UIBase {
    public static LoginView Instance;

    [SerializeField, Header("登录窗口")] private LoginPanel _loginPanel;
    [SerializeField, Header("登录窗口")] private RegisterPanel _registerPanel;
    [SerializeField, Header("Loading窗口")] private LoadingPanel _loadingPanel;

    private Dictionary<PanelType, UIBase> _panelDict;

    protected override void Awake() {
        Instance = this;
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public void ShowPanel(PanelType type) {
        _panelDict[type].Show();
    }

    public override void Init() {
        _panelDict = new Dictionary<PanelType, UIBase> {
            { PanelType.LoginPanel, _loginPanel },
            { PanelType.RegisterPanel, _registerPanel },
            { PanelType.LoadingPanel, _loadingPanel }
        };

        // 监听服务器返回的注册 & 登录结果
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_RegisterCode, OnRegisterHandle);
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_LoginCode, OnLoginHandle);
    }

    private void OnLoginHandle(ByteString data) {
        _loadingPanel.Show(false);
        LoginRes res = LoginRes.Parser.ParseFrom(data);
        if (res == null) {
            return;
        }

        switch (res.Code) {
            case CmdCode.Success:
                ShowSystemTips("登录成功!", Color.green);
                Debug.Log("用户ID：" + res.UserId);
                break;
            case CmdCode.PasswordNotBlank:
                ShowSystemTips("密码不能为空!", Color.red);
                break;
            case CmdCode.MobileNotBlank:
                ShowSystemTips("手机号不能为空!", Color.red);
                break;
            case CmdCode.AccountNotExist:
                ShowSystemTips("该手机号尚未注册!", Color.red);
                break;
            case CmdCode.PasswordError:
                ShowSystemTips("账号或密码错误!", Color.red);
                break;
            default:
                ShowSystemTips("未知错误!", Color.red);
                break;
        }
    }

    private void OnRegisterHandle(ByteString data) {
        _loadingPanel.Show(false);
        R res = R.Parser.ParseFrom(data);
        switch (res.Code) {
            case CmdCode.Success: {
                ShowSystemTips("注册成功!", Color.green);
                _registerPanel.Show(false);
                _loginPanel.Show();
                break;
            }
            case CmdCode.MobileNotBlank:
                ShowSystemTips("手机号不能为空!", Color.red);
                break;
            case CmdCode.PasswordNotBlank:
                ShowSystemTips("密码不能为空!", Color.red);
                break;
            case CmdCode.SmsCodeError:
                ShowSystemTips("短信验证码错误!", Color.red);
                break;
            case CmdCode.AccountExist:
                ShowSystemTips("该手机号已被注册!", Color.red);
                break;
            case CmdCode.ServerError:
                ShowSystemTips("服务器异常!", Color.red);
                break;
        }
    }
}