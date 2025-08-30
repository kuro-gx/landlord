using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class MainView : UIBase {
    [SerializeField, Header("用户昵称")] private Text _username;
    [SerializeField, Header("豆子")] private Text _money;

    [SerializeField, Header("设置按钮")] private Button _settingButton;
    [SerializeField, Header("战绩按钮")] private Button _featsButton;
    [SerializeField, Header("创建房间按钮")] private Button _createRoomBtn;
    [SerializeField, Header("加入房间按钮")] private Button _joinRoomBtn;

    [SerializeField, Header("用户信息面板")] private InfoPanel _infoPanel;
    [SerializeField, Header("设置面板")] private SettingPanel _settingPanel;
    [SerializeField, Header("加入房间面板")] private JoinPanel _joinPanel;

    // 临时保存修改用户信息参数
    private UpdateUserBo _userParam;

    public override void Init() {
        // 设置按钮点击事件
        _settingButton.onClick.AddListener(() => { _settingPanel.Show(); });

        // 创建房间按钮点击事件
        _createRoomBtn.onClick.AddListener(() => { });

        // 加入房间按钮点击事件
        _joinRoomBtn.onClick.AddListener(() => { _joinPanel.Show(); });

        _infoPanel.UpdateUserInfoAction = UpdateUserInfoHandle;
        
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
            Debug.Log(_username.text);
            Global.LoginUser.Username = _userParam.Username;
            Global.LoginUser.Gender = _userParam.Gender;
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

    private void Awake() {
        if (Global.LoginUser != null) {
            // 设置登录用户的昵称和欢乐豆
            _username.text = Global.LoginUser.Username;
            _money.text = Global.LoginUser.Money.ToString();
        }
    }

    private void OnDestroy() {
        _settingButton.onClick.RemoveAllListeners();
        _featsButton.onClick.RemoveAllListeners();
        _createRoomBtn.onClick.RemoveAllListeners();
        _joinRoomBtn.onClick.RemoveAllListeners();
    }

    // 点击头像显示用户设置面板
    public void OnAvatarClicked() {
        _infoPanel.Show();
    }
}