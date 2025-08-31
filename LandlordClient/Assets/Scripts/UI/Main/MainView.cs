using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class MainView : UIBase {
    [SerializeField, Header("用户昵称")] private Text _username;
    [SerializeField, Header("豆子")] private Text _money;

    [SerializeField, Header("设置按钮")] private Button _settingButton;
    [SerializeField, Header("修改昵称面板")] private SetInfoPanel _setInfoPanel;

    // 临时保存修改用户信息参数
    private UpdateUserBo _userParam;

    public override void Init() {
        if (Global.LoginUser != null) {
            // 设置登录用户的昵称和欢乐豆
            _username.text = Global.LoginUser.Username;
            _money.text = Global.LoginUser.Money.ToString();
        }
        
        // 设置按钮点击事件
        _settingButton.onClick.AddListener(() => { });

        _setInfoPanel.UpdateUserInfoAction = UpdateUserInfoHandle;

        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_UpdateUserInfoCode, OnUpdateInfoHandle);
    }

    /// <summary>
    /// 修改用户信息结果回调
    /// </summary>
    private void OnUpdateInfoHandle(ByteString data) {
        R res = R.Parser.ParseFrom(data);
        if (res.Code == CmdCode.Success) {
            ShowSystemTips("修改成功!", Color.green);
            _username.text = _userParam.Username;
            Global.LoginUser.Username = _userParam.Username;
            _setInfoPanel.Show(false);
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

    private void OnDestroy() {
        _settingButton.onClick.RemoveAllListeners();
    }
}