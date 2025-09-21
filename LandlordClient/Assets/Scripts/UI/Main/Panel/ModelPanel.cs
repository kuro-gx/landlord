using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 中间按钮区域面板
/// </summary>
public class ModelPanel : UIBase {
    [SerializeField, Header("经典模式按钮")] private Button _classifyModelBtn;
    private Sequence _closeSequence;

    public override void Init() {
        StartAnimation();
        
        // 点击经典模式，跳转对局界面
        _classifyModelBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            SceneManager.LoadScene("GameScene");
        });
    }

    private void OnDestroy() {
        _classifyModelBtn.onClick.RemoveAllListeners();
        DOTween.KillAll();
    }

    /// <summary>
    /// 起始动画
    /// </summary>
    private void StartAnimation() {
        transform.localPosition = new Vector3(800.0f, 0.0f, 0.0f);
        if (_closeSequence != null) {
            _closeSequence.Kill();
        }

        _closeSequence = DOTween.Sequence();
        _closeSequence
            .Append(transform.DOLocalMove(new Vector3(-60.0f, 0.0f, 0.0f), .3f)).SetEase(Ease.InOutSine)
            .Join(transform.DOScale(new Vector3(1.1f, 1.0f, 1.0f), .3f)).SetDelay(.2f)
            .Append(transform.DOLocalMove(new Vector3(40.0f, 0.0f, 0.0f), .3f))
            .Join(transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), .3f));

        _closeSequence.Play();
    }
}