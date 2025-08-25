using Google.Protobuf;

/// <summary>
/// 登录控制器
/// </summary>
public class LoginController : IContainer {
    private LoginService _loginService;

    public LoginController(LoginService service) {
        _loginService = service;
    }

    public void OnInit() {
    }

    public void OnServerCommand(BasePackage package) {
        switch (package.Code) {
            case NetDefine.CMD_RegisterCode:
                OnRegisterHandle(package);
                break;
            case NetDefine.CMD_LoginCode:
                OnLoginHandle(package);
                break;
        }
    }

    /// <summary>
    /// 登录请求处理
    /// </summary>
    /// <param name="package">请求参数</param>
    private void OnLoginHandle(BasePackage package) {
        LoginReq form = LoginReq.Parser.ParseFrom(package.Data);
        Session session = SessionMgr.Instance.GetSession(package.SessionId);
        LoginRes res = _loginService.Login(form);
        session.SendData(package, package.Code, res.ToByteString());
    }

    /// <summary>
    /// 处理注册事件
    /// </summary>
    private void OnRegisterHandle(BasePackage package) {
        R res = new R();
        RegisterReq form = RegisterReq.Parser.ParseFrom(package.Data);
        Session session = SessionMgr.Instance.GetSession(package.SessionId);

        if (!form.SmsCode.Equals("6666")) {
            res.Code = CmdCode.SmsCodeError;
            session.SendData(package, package.Code, res.ToByteString());
            return;
        }

        if (string.IsNullOrEmpty(form.Mobile)) {
            res.Code = CmdCode.MobileNotBlank;
            session.SendData(package, package.Code, res.ToByteString());
            return;
        }
        
        if (string.IsNullOrEmpty(form.Password)) {
            res.Code = CmdCode.PasswordNotBlank;
            session.SendData(package, package.Code, res.ToByteString());
            return;
        }

        res.Code = _loginService.Register(form);
        // 将结果返回给Unity
        session.SendData(package, package.Code, res.ToByteString());
    }
}