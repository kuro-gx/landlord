using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIBase {
    
    [SerializeField, Header("关闭按钮")] private Button _closeBtn;
    [SerializeField, Header("音量复选框")] private Toggle _toggleAudio;
    [SerializeField, Header("音量进度条")] private Slider _sliderVolume;
    
    public override void Init() {
        _closeBtn.onClick.AddListener(() => {
            Show(false);
        });

        _toggleAudio.onValueChanged.AddListener(result => {
            
        });
        
        _sliderVolume.onValueChanged.AddListener(speed => {
            
        });
    }
    
    private void OnDestroy() {
        _closeBtn.onClick.RemoveAllListeners();
        _toggleAudio.onValueChanged.RemoveAllListeners();
        _sliderVolume.onValueChanged.RemoveAllListeners();
    }
}