using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 顶部面板
/// </summary>
public class GameTopPanel : UIBase {
    [SerializeField, Header("左边")] private GameObject leftArea;
    [SerializeField, Header("右边")] private GameObject rightArea;
    [SerializeField, Header("退出按钮")] private Button exitBtnEl;

    protected override void Init() {
        // 退出按钮点击事件
        exitBtnEl.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            SceneManager.LoadScene("MainScene");
        });

        // 左右两边的缩放动画
        leftArea.transform.DOScale(Vector3.one, 0.4f);
        rightArea.transform.DOScale(new Vector3(-1f, 1f, 1f), 0.4f);
    }

    private void OnDestroy() {
        exitBtnEl.onClick.RemoveAllListeners();
    }
}