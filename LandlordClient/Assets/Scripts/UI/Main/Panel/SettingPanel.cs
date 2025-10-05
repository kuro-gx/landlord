using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIBase {

    [SerializeField, Header("关闭按钮")] private Button closeBtn;
    [SerializeField, Header("确定按钮")] private Button confirmBtn;
    [SerializeField, Header("音乐复选框")] private Toggle toggleMusic;
    [SerializeField, Header("音效复选框")] private Toggle toggleSound;
    [SerializeField, Header("音量进度条")] private Slider sliderVolume;

    private bool _bgmOn = false;
    private bool _effectOn = false;
    private float _volumeTemp = 0f;

    protected override void Init() {
        RefreshPanel();

        toggleMusic.onValueChanged.AddListener(OnMusicChanged);
        toggleSound.onValueChanged.AddListener(OnSoundChanged);
        sliderVolume.onValueChanged.AddListener(OnSliderChanged);
        
        confirmBtn.onClick.AddListener(OnConfirmBtnClicked);
        closeBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            Show(false);
        });
    }
    
    /// <summary>
    /// 初始化音乐、音效和音量
    /// </summary>
    public void RefreshPanel() {
        AudioService.Instance.InitMusic(ref _bgmOn, ref _effectOn, ref _volumeTemp);

        toggleMusic.isOn = _bgmOn;
        toggleSound.isOn = _effectOn;
        sliderVolume.value = _volumeTemp;
    }
    
    /// <summary>
    /// 音乐复选框改变触发
    /// </summary>
    private void OnMusicChanged(bool result) {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        
        _bgmOn = result;
        toggleMusic.isOn = _bgmOn;
        AudioService.Instance.SetMusic(_bgmOn);
    }
    
    /// <summary>
    /// 音效复选框改变触发
    /// </summary>
    private void OnSoundChanged(bool result) {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        
        _effectOn = result;
        toggleSound.isOn = _effectOn;
        AudioService.Instance.SetSound(_effectOn);
    }

    /// <summary>
    /// 音量滑动条改变触发
    /// </summary>
    private void OnSliderChanged(float result) {
        _volumeTemp = result;
        sliderVolume.value = _volumeTemp;
        AudioService.Instance.SetVolume(_volumeTemp);
    }

    /// <summary>
    /// 确定按钮点击事件
    /// </summary>
    private void OnConfirmBtnClicked() {
        AudioService.Instance.PlayUIAudio(Constant.NormalClick);
        
        PlayerPrefs.SetString("BGM", _bgmOn ? "ON" : "OFF");
        PlayerPrefs.SetString("Effect", _effectOn ? "ON" : "OFF");
        PlayerPrefs.SetFloat("Volume", _volumeTemp + 1);
        
        Show(false);
    }
    
    private void OnDestroy() {
        closeBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.RemoveAllListeners();
        toggleMusic.onValueChanged.RemoveAllListeners();
        toggleSound.onValueChanged.RemoveAllListeners();
        sliderVolume.onValueChanged.RemoveAllListeners();
    }
}