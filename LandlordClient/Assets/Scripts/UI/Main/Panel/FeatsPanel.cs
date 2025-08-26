using UnityEngine;
using UnityEngine.UI;

public class FeatsPanel : UIBase {
    [SerializeField, Header("关闭按钮")] private Button _closeBtn;
    
    public override void Init() {
        _closeBtn.onClick.AddListener(() => { Show(false); });
    }
    
    private void OnDestroy() {
        _closeBtn.onClick.RemoveAllListeners();
    }
}