using System;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : UIBase {
    [SerializeField, Header("关闭按钮")] private Button _closeBtn;
    [SerializeField, Header("确定按钮")] private Button _okBtn;
    [SerializeField, Header("用户名输入框")] private InputField _usernameInput;
    [SerializeField, Header("胜场数")] private Text _winText;
    [SerializeField, Header("败场数")] private Text _lostText;
    [SerializeField, Header("欢乐豆")] private Text _moneyText;
    [SerializeField, Header("性别男")] private Toggle _genderMan;
    [SerializeField, Header("性别女")] private Toggle _genderWoman;

    // 将参数传递给父组件让其发送网络请求
    public Action<UpdateUserBo> UpdateUserInfoAction;

    public override void Init() {
        _closeBtn.onClick.AddListener(() => { Show(false); });

        _okBtn.onClick.AddListener(() => {
            if (string.IsNullOrEmpty(_usernameInput.text.Trim())) {
                ShowSystemTips("昵称不能为空！", Color.red);
                return;
            }

            UpdateUserBo form = new UpdateUserBo {
                UserId = Global.LoginUser.UserId,
                Username = _usernameInput.text.Trim()
            };
            if (_genderMan.isOn) {
                form.Gender = 1;
            }

            if (_genderWoman.isOn) {
                form.Gender = 2;
            }

            // 将参数传给父组件，让其发起网络请求
            UpdateUserInfoAction?.Invoke(form);
        });
    }

    private void Awake() {
        if (Global.LoginUser != null) {
            // 数据填充
            _usernameInput.text = Global.LoginUser.Username;
            _moneyText.text = Global.LoginUser.Money.ToString();
            _winText.text = "胜场数：" + Global.LoginUser.WinCount;
            _lostText.text = "负场数：" + Global.LoginUser.LoseCount;
            if (Global.LoginUser.Gender == 1) {
                _genderMan.isOn = true;
            } else if (Global.LoginUser.Gender == 2) {
                _genderWoman.isOn = true;
            }
        }
    }

    private void OnDestroy() {
        _closeBtn.onClick.RemoveAllListeners();
        _okBtn.onClick.RemoveAllListeners();
    }
}