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
    [SerializeField, Header("战绩面板")] private FeatsPanel _featsPanel;

    public override void Init() {
        // 设置按钮点击事件
        _settingButton.onClick.AddListener(() => { _settingPanel.Show(); });

        // 战绩按钮点击事件
        _featsButton.onClick.AddListener(() => { _featsPanel.Show(); });

        // 创建房间按钮点击事件
        _createRoomBtn.onClick.AddListener(() => { });

        // 加入房间按钮点击事件
        _joinRoomBtn.onClick.AddListener(() => { _joinPanel.Show(); });
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