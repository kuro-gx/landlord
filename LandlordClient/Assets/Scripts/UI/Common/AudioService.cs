using UnityEngine;

/// <summary>
/// 音频服务
/// </summary>
public class AudioService : MonoBehaviour {
    public static AudioService Instance;
    
    [SerializeField, Header("BGM")] private AudioSource bgmAudio;
    [SerializeField, Header("特效音效")] private AudioSource effectAudio;
    [SerializeField, Header("点击UI音效")] private AudioSource uiAudio;
    [SerializeField, Header("操作音效")] private AudioSource operateAudio;

    private void Awake() {
        Instance = this;
        // 场景切换时保留本组件
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="audioName">音频文件名</param>
    public void PlayUIAudio(string audioName) {
        AudioClip source = Resources.Load<AudioClip>("Audio/UI/" + audioName);
        uiAudio.clip = source;
        uiAudio.Play();
    }

    public void PlayEffectAudio(string audioName) {
        AudioClip source = Resources.Load<AudioClip>("Audio/Effect/" + audioName);
        effectAudio.clip = source;
        effectAudio.Play();
    }

    public void PlayOperateAudio(int pos, string audioName) {
        AudioClip source = Resources.Load<AudioClip>($"Audio/Operate/Character{pos}/{audioName}");
        operateAudio.clip = source;
        operateAudio.Play();
    }
}