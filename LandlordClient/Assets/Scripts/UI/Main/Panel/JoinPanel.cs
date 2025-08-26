using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoinPanel : UIBase {
    [SerializeField, Header("关闭按钮")] private Button _closeBtn;
    [SerializeField, Header("输入的房间号")] private Text _roomNumText;

    private List<string> _roomNoList = new();

    public override void Init() {
        // 点击关闭面板按钮，隐藏面板，清除输入的房号
        _closeBtn.onClick.AddListener(() => {
            Show(false);
            OnClearRoomNo();
        });

        // 给键盘按钮绑定点击事件
        Button[] buttons = transform.GetChild(0).GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++) {
            // 数字按钮
            if (buttons[i].name.StartsWith("numBtn")) {
                buttons[i].onClick.AddListener(OnNumBtnClicked);
            }

            // 清除按钮
            if (buttons[i].name == "clearBtn") {
                buttons[i].onClick.AddListener(OnClearRoomNo);
            }

            // todo 确定按钮
            if (buttons[i].name == "okBtn") {
                buttons[i].onClick.AddListener(() => { });
            }
        }
    }

    private void OnDestroy() {
        _closeBtn.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 数字键盘按钮点击事件
    /// </summary>
    private void OnNumBtnClicked() {
        // 房间号输入完毕
        if (_roomNoList.Count >= 6) {
            return;
        }

        // 获取按钮名称
        var buttonName = EventSystem.current.currentSelectedGameObject.name;
        var arr = buttonName.Split('_');
        _roomNoList.Add(arr[1]);

        // 显示已输入的房间号
        RefreshRoomNo();
    }

    /// <summary>
    /// 重新渲染用户输入的房间号
    /// </summary>
    private void RefreshRoomNo() {
        if (_roomNoList.Count == 0) {
            _roomNumText.text = "";
            return;
        }

        string tmp = "";
        for (var i = 0; i < _roomNoList.Count; i++) {
            if (i != _roomNoList.Count - 1) {
                tmp += _roomNoList[i] + "     ";
            } else {
                tmp += _roomNoList[i];
            }
        }

        _roomNumText.text = tmp;
    }

    /// <summary>
    /// 清除已经输入的房间号
    /// </summary>
    private void OnClearRoomNo() {
        _roomNoList.Clear();
        RefreshRoomNo();
    }
}