using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIBase {
    
    [SerializeField, Header("关闭按钮")] private Button closeBtn;
    [SerializeField, Header("音量复选框")] private Toggle toggleAudio;
    [SerializeField, Header("音量进度条")] private Slider sliderVolume;

    protected override void Init() {
        closeBtn.onClick.AddListener(() => {
            Show(false);
        });

        toggleAudio.onValueChanged.AddListener(result => {
            
        });
        
        sliderVolume.onValueChanged.AddListener(speed => {
            
        });
    }
    
    private void OnDestroy() {
        closeBtn.onClick.RemoveAllListeners();
        toggleAudio.onValueChanged.RemoveAllListeners();
        sliderVolume.onValueChanged.RemoveAllListeners();
    }
}