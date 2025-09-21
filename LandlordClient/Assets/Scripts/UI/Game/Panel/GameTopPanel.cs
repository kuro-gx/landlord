using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 顶部面板
/// </summary>
public class GameTopPanel : UIBase {
    [SerializeField, Header("退出按钮")] private Button _exitBtn;
    [SerializeField, Header("时间文本")] private Text _timeText;
    [SerializeField, Header("换桌按钮")] private Button _changeTableBtn;
    [SerializeField, Header("托管按钮")] private Button _hostedBtn;

    public override void Init() {
        // 退出按钮点击事件
        _exitBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            SceneManager.LoadScene("MainScene");
        });
    }

    private void OnDestroy() {
        _exitBtn.onClick.RemoveAllListeners();
    }
}