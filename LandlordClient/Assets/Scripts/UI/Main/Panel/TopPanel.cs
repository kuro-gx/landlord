using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TopPanel : UIBase {
    [SerializeField, Header("设置按钮")] private Button _settingBtn;
    [SerializeField, Header("欢乐豆数量")] private Text _beanText;
    private RectTransform _transform;

    public override void Init() {
        // 面板从右往左出现
        _transform = GetComponent<RectTransform>();
        _transform.DOAnchorPos(new Vector2(0.0f, 0.0f), 0.4f).From(new Vector2(550.0f, 0.0f));

        _settingBtn.onClick.AddListener(() => { });
    }

    private void OnDestroy() {
        _settingBtn.onClick.RemoveAllListeners();
    }
}