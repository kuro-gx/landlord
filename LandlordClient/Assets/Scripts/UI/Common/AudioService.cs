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

    private bool _bgmOn = true; // BGM是否打开
    private bool _effectOn = true; // 音效是否打开
    private float _volume = 0; // 音量

    private void Awake() {
        Instance = this;
        // 场景切换时保留本组件
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 初始化音乐、音效和音量
    /// </summary>
    public void InitMusic(ref bool bmg, ref bool effect, ref float volume) {
        string bgmStr = PlayerPrefs.GetString("BGM");
        string effectStr = PlayerPrefs.GetString("Effect");
        float volumeTmp = PlayerPrefs.GetFloat("Volume");

        if (string.IsNullOrEmpty(bgmStr) || bgmStr == "ON") {
            _bgmOn = true;
            bmg = true;
        } else {
            _bgmOn = false;
            bmg = false;
        }

        if (string.IsNullOrEmpty(effectStr) || effectStr == "ON") {
            _effectOn = true;
            effect = true;
        } else {
            _effectOn = false;
            effect = false;
        }

        if (volumeTmp == 0.0f) {
            _volume = 1f;
            volume = 1f;
        } else {
            _volume -= 1;
            volume -= 1;
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBGMAudio(string audioName, bool isLoop = true) {
        bgmAudio.loop = isLoop;
        AudioClip source = Resources.Load<AudioClip>("Audio/BGM/" + audioName);
        bgmAudio.clip = source;
        if (_bgmOn) {
            bgmAudio.Play();
        }
    }

    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="audioName">音频文件名</param>
    public void PlayUIAudio(string audioName) {
        AudioClip source = Resources.Load<AudioClip>("Audio/UI/" + audioName);
        uiAudio.clip = source;
        if (_effectOn) {
            uiAudio.Play();
        }
    }

    public void PlayEffectAudio(string audioName) {
        AudioClip source = Resources.Load<AudioClip>("Audio/Effect/" + audioName);
        effectAudio.clip = source;
        if (_effectOn) {
            effectAudio.Play();
        }
    }

    public void PlayOperateAudio(int pos, string audioName) {
        AudioClip source = Resources.Load<AudioClip>($"Audio/Operate/Character{pos}/{audioName}");
        operateAudio.clip = source;
        if (_effectOn) {
            operateAudio.Play();
        }
    }

    /// <summary>
    /// 播放打出卡牌的具体音效，如“对3”、“王炸”
    /// </summary>
    public void PlayCardAudio(int pos, List<Card> cards, bool isCover, PlayerDirection direction) {
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
            case CardType.HandPlanePair: {
                if (direction == PlayerDirection.Left) {
                    EffectPanel.Instance.PlayPlaneEffect("Feiji_L");
                } else if (direction == PlayerDirection.Center) {
                    EffectPanel.Instance.PlayPlaneEffect("Feiji_M");
                } else if (direction == PlayerDirection.Right) {
                    EffectPanel.Instance.PlayPlaneEffect("Feiji_R");
                }

                PlayEffectAudio(Constant.HandPlane);
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "Plane"));
                break;
            }
            case CardType.HandSeqPair:
                EffectPanel.Instance.PlaySeqPairEffect();
                PlayEffectAudio(Constant.HandSeq);
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "SeqPair"));
                break;
            case CardType.HandSeqSingle:
                EffectPanel.Instance.PlaySeqSingleEffect();
                PlayEffectAudio(Constant.HandSeq);
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "SeqSingle"));
                break;
            case CardType.HandBomb: {
                if (direction == PlayerDirection.Left) {
                    EffectPanel.Instance.PlayBombEffect("lujingzuo");
                } else if (direction == PlayerDirection.Center) {
                    EffectPanel.Instance.PlayBombEffect("lujingzhu");
                } else if (direction == PlayerDirection.Right) {
                    EffectPanel.Instance.PlayBombEffect("lujingyou");
                }

                PlayEffectAudio(Constant.HandBomb);
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "Bomb"));
                break;
            }
            case CardType.HandBombPair:
            case CardType.HandBombTwoSingle:
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "BombTwoSingle"));
                break;
            case CardType.HandBombTwoPair:
                source = Resources.Load<AudioClip>(basePath + (isCover ? cover : "BombTwoPair"));
                break;
            case CardType.HandBombJokers: {
                if (direction == PlayerDirection.Left) {
                    EffectPanel.Instance.PlayBombEffect("lujingzuo");
                } else if (direction == PlayerDirection.Center) {
                    EffectPanel.Instance.PlayBombEffect("lujingzhu");
                } else if (direction == PlayerDirection.Right) {
                    EffectPanel.Instance.PlayBombEffect("lujingyou");
                }

                PlayEffectAudio(Constant.HandBombJoker);
                source = Resources.Load<AudioClip>($"Audio/Operate/Character{pos}/BombJokers");
                break;
            }
        }

        if (source == null) {
            return;
        }

        operateAudio.clip = source;
        operateAudio.Play();
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="volume">音量</param>
    public void SetVolume(float volume) {
        _volume = volume;

        bgmAudio.volume = volume;
        uiAudio.volume = volume;
        effectAudio.volume = volume;
        operateAudio.volume = volume;
    }

    /// <summary>
    /// 设置BGM状态
    /// </summary>
    /// <param name="isPlay">是否静音</param>
    public void SetMusic(bool isPlay) {
        _bgmOn = isPlay;

        if (isPlay) {
            if (bgmAudio != null && bgmAudio.clip != null && !bgmAudio.isPlaying) {
                bgmAudio.Play();
            }
        } else {
            if (bgmAudio != null && bgmAudio.clip != null && bgmAudio.isPlaying) {
                bgmAudio.Stop();
            }
        }
    }

    /// <summary>
    /// 设置音效状态
    /// </summary>
    /// <param name="isPlay">是否静音</param>
    public void SetSound(bool isPlay) {
        _effectOn = isPlay;
    }
}