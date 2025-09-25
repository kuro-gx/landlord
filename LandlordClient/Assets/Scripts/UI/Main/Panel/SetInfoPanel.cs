using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 修改用户昵称面板
/// </summary>
public class SetInfoPanel : UIBase {
    [SerializeField, Header("关闭按钮")] private Button closeBtn;
    [SerializeField, Header("提交按钮")] private Button submitBtn;
    [SerializeField, Header("用户名表单")] private InputField usernameInput;

    // 将参数传递给父组件让其发送网络请求
    public Action<UpdateUserBo> UpdateUserInfoAction;

    public override void Init() {
        closeBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            Show(false);
        });

        submitBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);

            if (string.IsNullOrEmpty(usernameInput.text.Trim())) {
                ShowSystemTips("昵称不能为空！", Color.red);
                return;
            }

            UpdateUserBo form = new UpdateUserBo {
                UserId = Global.LoginUser.UserId,
                Username = usernameInput.text.Trim()
            };

            // 将参数传给父组件，让其发起网络请求
            UpdateUserInfoAction?.Invoke(form);
        });
    }

    private void OnEnable() {
        if (Global.LoginUser != null) {
            usernameInput.text = Global.LoginUser.Username;
        }
    }

    private void OnDestroy() {
        closeBtn.onClick.RemoveAllListeners();
        submitBtn.onClick.RemoveAllListeners();
    }
}