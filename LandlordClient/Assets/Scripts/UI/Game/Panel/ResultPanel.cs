using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 结算面板
/// </summary>
public class ResultPanel : UIBase {
    [SerializeField, Header("人物形象")] private Image characterEl;
    
    [SerializeField, Header("胜利图标")] private RectTransform winIcon;
    [SerializeField, Header("失败图标")] private RectTransform lostIcon;

    [SerializeField, Header("地图图标1")] private Image row1LandIcon;
    [SerializeField, Header("昵称1")] private Text row1Nickname;
    [SerializeField, Header("底分1")] private Text row1Score;
    [SerializeField, Header("倍数1")] private Text row1Multiple;
    [SerializeField, Header("欢乐豆1")] private Text row1Money;

    [SerializeField, Header("地图图标2")] private Image row2LandIcon;
    [SerializeField, Header("昵称2")] private Text row2Nickname;
    [SerializeField, Header("底分2")] private Text row2Score;
    [SerializeField, Header("倍数2")] private Text row2Multiple;
    [SerializeField, Header("欢乐豆2")] private Text row2Money;

    [SerializeField, Header("地图图标3")] private Image row3LandIcon;
    [SerializeField, Header("昵称3")] private Text row3Nickname;
    [SerializeField, Header("底分3")] private Text row3Score;
    [SerializeField, Header("倍数3")] private Text row3Multiple;
    [SerializeField, Header("欢乐豆3")] private Text row3Money;

    [SerializeField, Header("继续游戏")] private Button startBtn;
    [SerializeField, Header("返回大厅")] private Button backBtn;

    protected override void Init() {
        backBtn.onClick.AddListener(() => {
            AudioService.Instance.PlayUIAudio(Constant.NormalClick);
            SceneManager.LoadScene("MainScene");
        });
    }

    /// <summary>
    /// 显示or隐藏面板
    /// </summary>
    public void ShowPanel(GameEndResponse response, int selfPos, bool isShow = true) {
        gameObject.transform.DOScale(isShow ? Vector3.one : Vector3.zero, 0.4f);

        if (!isShow) {
            return;
        }
        
        // 人物形象
        characterEl.sprite = Resources.Load<Sprite>("Character/Tex_1" + selfPos);

        // 设置底分和倍数
        row1Score.text = response.BaseScore.ToString();
        row2Score.text = response.BaseScore.ToString();
        row3Score.text = response.BaseScore.ToString();
        row1Multiple.text = response.Multiple.ToString();
        row2Multiple.text = response.Multiple.ToString();
        row3Multiple.text = response.Multiple.ToString();

        // 除自己外的两个玩家
        var list = new List<PlayerResult>(2);
        // 自己的数据
        PlayerResult self = null;
        foreach (var p in response.Players) {
            if (p.Pos == selfPos) {
                self = p;
            } else {
                list.Add(p);
            }
        }

        // 自己的数据
        if (self!.IsLord) {
            row1LandIcon.gameObject.SetActive(true);
        }
        row1Nickname.text = self.Nickname;
        if (self.Money < 0) {
            row1Money.text = self.Money.ToString();
        } else {
            row1Money.text = "+" + self.Money;
        }
        
        // 另外玩家的数据
        if (list[0].IsLord) {
            row2LandIcon.gameObject.SetActive(true);
        }
        row2Nickname.text = list[0].Nickname;
        if (list[0].Money < 0) {
            row2Money.text = list[0].Money.ToString();
        } else {
            row2Money.text = "+" + list[0].Money;
        }
        
        if (list[1].IsLord) {
            row3LandIcon.gameObject.SetActive(true);
        }
        row3Nickname.text = list[1].Nickname;
        if (list[1].Money < 0) {
            row3Money.text = list[1].Money.ToString();
        } else {
            row3Money.text = "+" + list[1].Money;
        }
    }

    private void OnDestroy() {
        backBtn.onClick.RemoveAllListeners();
    }
}