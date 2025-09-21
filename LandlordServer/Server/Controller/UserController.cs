using Google.Protobuf;

/// <summary>
/// 登录控制器
/// </summary>
public class UserController : IContainer {
    private UserService _userService;

    public UserController(UserService service) {
        _userService = service;
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
            case NetDefine.CMD_UpdateUserInfoCode:
                OnUpdateHandle(package);
                break;
        }
    }

    /// <summary>
    /// 登录请求处理
    /// </summary>
    /// <param name="package">请求参数</param>
    private void OnLoginHandle(BasePackage package) {
        LoginBo form = LoginBo.Parser.ParseFrom(package.Data);
        Session session = SessionMgr.Instance.GetSession(package.SessionId);

        LoginResponse res = new LoginResponse();
        if (string.IsNullOrEmpty(form.Password)) {
            res.Code = ResultCode.PasswordNotBlank;
            session.SendData(package, package.Code, res.ToByteString());
            return;
        }

        if (string.IsNullOrEmpty(form.Mobile)) {
            res.Code = ResultCode.MobileNotBlank;
            session.SendData(package, package.Code, res.ToByteString());
            return;
        }

        res = _userService.Login(form);
        session.SendData(package, package.Code, res.ToByteString());
        // 登录成功，保存用户的ID
        if (res.Code == ResultCode.Success) {
            session.UserId = res.UserId;
            // 缓存Player类和连接对象
            User loginUser = new User(res.UserId, res.Username, res.Money);
            Cache.Instance.UserDict.Add(res.UserId, loginUser);
            Cache.Instance.SessionDict.Add(res.UserId, session);
        }
    }

    /// <summary>
    /// 处理注册事件
    /// </summary>
    private void OnRegisterHandle(BasePackage package) {
        Result res = new Result();
        RegisterBo form = RegisterBo.Parser.ParseFrom(package.Data);
        Session session = SessionMgr.Instance.GetSession(package.SessionId);

        if (!form.SmsCode.Equals("6666")) {
            res.Code = ResultCode.SmsCodeError;
            session.SendData(package, package.Code, res.ToByteString());
            return;
        }

        if (string.IsNullOrEmpty(form.Mobile)) {
            res.Code = ResultCode.MobileNotBlank;
            session.SendData(package, package.Code, res.ToByteString());
            return;
        }

        if (string.IsNullOrEmpty(form.Password)) {
            res.Code = ResultCode.PasswordNotBlank;
            session.SendData(package, package.Code, res.ToByteString());
            return;
        }

        res.Code = _userService.Register(form);
        // 将结果返回给Unity
        session.SendData(package, package.Code, res.ToByteString());
    }

    /// <summary>
    /// 修改用户信息
    /// </summary>
    private void OnUpdateHandle(BasePackage package) {
        UpdateUserBo form = UpdateUserBo.Parser.ParseFrom(package.Data);
        Session session = SessionMgr.Instance.GetSession(package.SessionId);

        ResultCode code = _userService.UpdateUserInfo(form);
        Result result = new Result { Code = code };
        session.SendData(package, package.Code, result.ToByteString());
    }
}