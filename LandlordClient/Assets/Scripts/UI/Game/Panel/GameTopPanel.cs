using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 顶部面板
/// </summary>
public class GameTopPanel : UIBase {
    [SerializeField, Header("退出按钮")] private Button exitBtnEl;
    [SerializeField, Header("时间文本")] private Text timeTextEl;
    [SerializeField, Header("换桌按钮")] private Button changeTableBtnEl;
    [SerializeField, Header("托管按钮")] private Button hostedBtnEl;

    public override void Init() {
        // 退出按钮点击事件
        exitBtnEl.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            SceneManager.LoadScene("MainScene");
        });
    }

    private void OnDestroy() {
        exitBtnEl.onClick.RemoveAllListeners();
    }
}