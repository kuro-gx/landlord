using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TopPanel : UIBase {
    [SerializeField, Header("设置按钮")] private Button settingBtn;
    [SerializeField, Header("欢乐豆数量")] private Text beanText;
    [SerializeField, Header("钻石")] private Text diamondText;
    private RectTransform _transform;

    protected override void Init() {
        // 面板从右往左出现
        _transform = GetComponent<RectTransform>();
        _transform.DOAnchorPos(new Vector2(0.0f, 0.0f), 0.4f).From(new Vector2(550.0f, 0.0f));

        settingBtn.onClick.AddListener(() => { });
    }

    private void OnDestroy() {
        settingBtn.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 设置豆子和钻石数量
    /// </summary>
    /// <param name="bean">豆子</param>
    /// <param name="diamond">钻石</param>
    public void SetMoney(int bean, int diamond) {
        beanText.text = bean.ToString();
        diamondText.text = diamond.ToString();
    }
}