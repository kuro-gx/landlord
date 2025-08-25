using System;
using System.Security.Cryptography;
using System.Text;
using SqlSugar;

public class LoginService {
    private readonly SqlSugarClient _db;

    public LoginService(SqlSugarClient db) {
        _db = db;
    }

    /// <summary>
    /// 注册账号
    /// </summary>
    /// <param name="req">请求参数</param>
    public CmdCode Register(RegisterReq req) {
        int count = _db.Queryable<User>().Where(v => v.Mobile == req.Mobile).Count();
        // 账号已存在
        if (count > 0) {
            return CmdCode.AccountExist;
        }

        User user = new User() {
            Username = "用户" + new Random().Next(1000, 10000),
            Mobile = req.Mobile,
            Password = MD5Encrypt64(req.Password),
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        };
        int res = _db.Insertable(user).ExecuteCommand();
        if (res <= 0) {
            return CmdCode.ServerError;
        }

        return CmdCode.Success;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="form">登录参数</param>
    public LoginRes Login(LoginReq form) {
        LoginRes res = new LoginRes();
        if (string.IsNullOrEmpty(form.Password)) {
            res.Code = CmdCode.PasswordNotBlank;
            return res;
        }

        if (string.IsNullOrEmpty(form.Mobile)) {
            res.Code = CmdCode.MobileNotBlank;
            return res;
        }

        User user = _db.Queryable<User>().Where(v => v.Mobile == form.Mobile).First();
        if (user == null) {
            res.Code = CmdCode.AccountNotExist;
            return res;
        }

        if (!MD5Encrypt64(form.Password).Equals(user.Password)) {
            res.Code = CmdCode.PasswordError;
            return res;
        }

        res.Code = CmdCode.Success;
        res.UserId = user.Id;
        return res;
    }

    private string MD5Encrypt64(string password) {
        MD5 md5 = MD5.Create();
        // 加密后是一个字节类型的数组
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(s);
    }
}