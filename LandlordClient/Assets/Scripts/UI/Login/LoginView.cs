using System.Collections.Generic;
using UnityEngine;

public class LoginView : UIBase {
    public static LoginView Instance;
    
    [SerializeField, Header("登录窗口")] private LoginPanel _loginPanel;
    [SerializeField, Header("登录窗口")] private RegisterPanel _registerPanel;

    private Dictionary<PanelType, UIBase> _panelDict;

    protected override void Awake() {
        Instance = this;
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    public UIBase GetPanel(PanelType type) {
        return _panelDict[type];
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public void ShowPanel(PanelType type) {
        _panelDict[type].Show();
    }

    public override void Init() {
        _panelDict = new Dictionary<PanelType, UIBase> {
            { PanelType.LoginPanel, _loginPanel },
            { PanelType.RegisterPanel, _registerPanel }
        };
    }
}