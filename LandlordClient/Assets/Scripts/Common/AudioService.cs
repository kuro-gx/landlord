using UnityEngine;

/// <summary>
/// 音频服务
/// </summary>
public class AudioService : MonoBehaviour {
    public static AudioService Instance;
    
    [SerializeField, Header("BGM")] private AudioSource _bgmAudio;
    [SerializeField, Header("特效音效")] private AudioSource _effectAudio;
    [SerializeField, Header("点击UI音效")] private AudioSource _uiAudio;
    [SerializeField, Header("操作音效")] private AudioSource _oparateAudio;

    private void Awake() {
        Instance = this;
        // 场景切换时保留本组件
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="name">音频文件名</param>
    public void PlayUIAudio(string name) {
        AudioClip audio = Resources.Load<AudioClip>("Audio/UI/" + name);
        _uiAudio.clip = audio;
        _uiAudio.Play();
    }

    public void PlayEffectAudio(string name) {
        AudioClip audio = Resources.Load<AudioClip>("Audio/Effect/" + name);
        _effectAudio.clip = audio;
        _effectAudio.Play();
    }
}