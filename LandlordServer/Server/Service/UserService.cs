using System;
using System.Security.Cryptography;
using System.Text;
using SqlSugar;

public class UserService {
    private readonly SqlSugarClient _db;

    public UserService(SqlSugarClient db) {
        _db = db;
    }

    /// <summary>
    /// 注册账号
    /// </summary>
    /// <param name="form">请求参数</param>
    public ResultCode Register(RegisterBo form) {
        int count = _db.Queryable<User>().Where(v => v.Mobile == form.Mobile).Count();
        // 账号已存在
        if (count > 0) {
            return ResultCode.AccountExist;
        }

        User user = new User {
            Username = "用户" + new Random().Next(1000, 10000),
            Mobile = form.Mobile,
            Password = MD5Encrypt64(form.Password),
            Gender = 0,
            State = 1,
            Money = 8000,
            Diamond = 500,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        };
        int res = _db.Insertable(user).ExecuteCommand();
        if (res <= 0) {
            return ResultCode.ServerError;
        }

        return ResultCode.Success;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="form">登录参数</param>
    public LoginResponse Login(LoginBo form) {
        LoginResponse res = new LoginResponse();

        User user = _db.Queryable<User>()
            .Where(v => v.Mobile == form.Mobile).Where(v => v.State != 3).First();
        if (user == null) {
            res.Code = ResultCode.AccountNotExist;
            return res;
        }

        if (!MD5Encrypt64(form.Password).Equals(user.Password)) {
            res.Code = ResultCode.PasswordError;
            return res;
        }

        // 账号被冻结
        if (user.State == 2) {
            res.Code = ResultCode.AccountTerminate;
            return res;
        }

        res.Code = ResultCode.Success;
        res.UserId = user.Id;
        res.Username = user.Username;
        res.Gender = user.Gender;
        res.Money = user.Money;
        res.Diamond = user.Diamond;
        res.WinCount = user.WinCount;
        res.LoseCount = user.LoseCount;
        return res;
    }

    /// <summary>
    /// 修改用户信息
    /// </summary>
    public ResultCode UpdateUserInfo(UpdateUserBo form) {
        User user = _db.Queryable<User>().Where(v => v.Id == form.UserId).First();
        if (user == null) {
            return ResultCode.AccountNotExist;
        }

        user.Username = form.Username;
        if (form.Gender == 1 || form.Gender == 2) {
            user.Gender = (byte)form.Gender;
        }

        int result = _db.Updateable(user).ExecuteCommand();
        if (result <= 0) {
            return ResultCode.ServerError;
        }

        return ResultCode.Success;
    }

    private string MD5Encrypt64(string password) {
        MD5 md5 = MD5.Create();
        // 加密后是一个字节类型的数组
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(s);
    }
}