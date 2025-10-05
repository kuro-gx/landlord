using Spine.Unity;
using UnityEngine;

/// <summary>
/// 特效面板
/// </summary>
public class EffectPanel : UIBase {
    [SerializeField, Header("春天特效")] private SkeletonAnimation springEffect;
    [SerializeField, Header("飞机特效")] private SkeletonAnimation planeEffect;
    [SerializeField, Header("连对特效")] private SkeletonAnimation seqPairEffect;
    [SerializeField, Header("顺子特效")] private SkeletonAnimation seqSingleEffect;
    [SerializeField, Header("炸弹特效")] private SkeletonAnimation bombEffect;
    
    public static EffectPanel Instance;
    
    private void Awake() {
        Instance = this;
    }

    protected override void Init() {
    }

    /// <summary>
    /// 播放春天特效
    /// </summary>
    public void PlaySpringEffect() {
        springEffect.gameObject.SetActive(true);
        springEffect.AnimationState.SetAnimation(0, "Spring", false);
        // 动画播放完成隐藏特效
        springEffect.AnimationState.Complete += _ => {
            springEffect.gameObject.SetActive(false);
        };
    }
    
    /// <summary>
    /// 播放飞机特效: Feiji_L、Feiji_M、Feiji_R
    /// </summary>
    public void PlayPlaneEffect(string effectName) {
        planeEffect.gameObject.SetActive(true);
        planeEffect.AnimationState.SetAnimation(0, effectName, false);
        // 动画播放完成隐藏特效
        planeEffect.AnimationState.Complete += _ => {
            planeEffect.gameObject.SetActive(false);
        };
    }
    
    /// <summary>
    /// 播放连对特效
    /// </summary>
    public void PlaySeqPairEffect() {
        seqPairEffect.gameObject.SetActive(true);
        seqPairEffect.AnimationState.SetAnimation(0, "liandui", false);
        // 动画播放完成隐藏特效
        seqPairEffect.AnimationState.Complete += _ => {
            seqPairEffect.gameObject.SetActive(false);
        };
    }
    
    /// <summary>
    /// 播放顺子特效
    /// </summary>
    public void PlaySeqSingleEffect() {
        seqSingleEffect.gameObject.SetActive(true);
        seqSingleEffect.AnimationState.SetAnimation(0, "ShunZi_0412_clean", false);
        // 动画播放完成隐藏特效
        seqSingleEffect.AnimationState.Complete += _ => {
            seqSingleEffect.gameObject.SetActive(false);
        };
    }
    
    /// <summary>
    /// 播放炸弹特效: lujingyou、lujingzhu、lujingzuo
    /// </summary>
    public void PlayBombEffect(string effectName) {
        bombEffect.gameObject.SetActive(true);
        bombEffect.AnimationState.SetAnimation(0, effectName, false);
        bombEffect.AnimationState.AddAnimation(0, "zha", false, 0);
        // 动画播放完成隐藏特效
        bombEffect.AnimationState.Complete += _ => {
            bombEffect.gameObject.SetActive(false);
        };
    }
}