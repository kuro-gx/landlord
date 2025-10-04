using System.Collections.Generic;
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

    /// <summary>
    /// 播放打出卡牌的具体音效，如“对3”、“王炸”
    /// </summary>
    public void PlayCardAudio(int pos, List<Card> cards, bool isCover) {
        var playHand = CardMgr.Instance.GetCardType(cards);
        AudioClip source = null;
        string basePath = $"Audio/Operate/Character{pos}/";
        string cover = $"Cover{Random.Range(0, 2)}";
        switch (playHand.Type) {
            case CardType.HandSingle:
                source = Resources.Load<AudioClip>(basePath + (int)playHand.Point);
                break;
            case CardType.HandPair:
                source = Resources.Load<AudioClip>(basePath + $"Pair{(int)playHand.Point}");
                break;
            case CardType.HandTriple:
                source = Resources.Load<AudioClip>(basePath + $"Tuple{(int)playHand.Point}");
                break;
            case CardType.HandTripleSingle:
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "TripleSingle"));
                break;
            case CardType.HandTriplePair:
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "TriplePair"));
                break;
            case CardType.HandPlane:
            case CardType.HandPlaneSingle:
            case CardType.HandPlanePair:
                PlayEffectAudio(Constant.HandPlane);
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "Plane"));
                break;
            case CardType.HandSeqPair:
                PlayEffectAudio(Constant.HandSeq);
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "SeqPair"));
                break;
            case CardType.HandSeqSingle:
                PlayEffectAudio(Constant.HandSeq);
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "SeqSingle"));
                break;
            case CardType.HandBomb:
                PlayEffectAudio(Constant.HandBomb);
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "Bomb"));
                break;
            case CardType.HandBombPair:
            case CardType.HandBombTwoSingle:
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "BombTwoSingle"));
                break;
            case CardType.HandBombTwoPair:
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "BombTwoPair"));
                break;
            case CardType.HandBombJokers:
                PlayEffectAudio(Constant.HandBombJoker);
                source = Resources.Load<AudioClip>($"Audio/Operate/Character{pos}/BombJokers");
                break;
        }

        if (source == null) {
            return;
        }

        operateAudio.clip = source;
        operateAudio.Play();
    }
}