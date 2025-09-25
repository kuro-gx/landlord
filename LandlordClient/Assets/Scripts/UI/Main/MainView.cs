using Google.Protobuf;
using UnityEngine;

public class MainView : UIBase {
    [SerializeField, Header("用户信息面板")] private InfoPanel infoPanel;
    [SerializeField, Header("修改昵称面板")] private SetInfoPanel setInfoPanel;
    [SerializeField, Header("右上角区域面板")] private TopPanel topPanel;

    // 临时保存修改用户信息参数
    private UpdateUserBo _userParam;

    public override void Init() {
        if (Global.LoginUser != null) {
            // 设置登录用户的昵称和欢乐豆
            infoPanel.SetUsername(Global.LoginUser.Username);
            topPanel.SetMoney(Global.LoginUser.Money, Global.LoginUser.Diamond);
        }

        setInfoPanel.UpdateUserInfoAction = UpdateUserInfoHandle;

        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_UpdateUserInfoCode, OnUpdateInfoHandle);
    }

    /// <summary>
    /// 修改用户信息结果回调
    /// </summary>
    private void OnUpdateInfoHandle(ByteString data) {
        Result res = Result.Parser.ParseFrom(data);
        if (res.Code == ResultCode.Success) {
            ShowSystemTips("修改成功!", Color.green);
            infoPanel.SetUsername(_userParam.Username);
            Global.LoginUser.Username = _userParam.Username;
            setInfoPanel.Show(false);
        } else {
            ShowSystemTips("服务器异常!", Color.red);
        }
    }

    /// <summary>
    /// 修改用户信息请求
    /// </summary>
    private void UpdateUserInfoHandle(UpdateUserBo form) {
        _userParam = form;
        NetSocketMgr.Client.SendData(NetDefine.CMD_UpdateUserInfoCode, form.ToByteString());
    }
}