using System.Linq;
using Google.Protobuf;
using UnityEngine;

public class GameView : UIBase {
    [SerializeField, Header("底牌面板")] private PocketCardPanel _pocketCardPanel;
    [SerializeField, Header("游戏界面顶部面板")] private GameTopPanel _gameTopPanel;
    [SerializeField, Header("按钮组面板")] private ButtonsPanel _buttonsPanel;
    [SerializeField, Header("自己的信息面板")] private SelfPlayerPanel _selfPlayerPanel;
    [SerializeField, Header("左侧玩家信息面板")] private LeftPlayerPanel _leftPlayerPanel;
    [SerializeField, Header("右侧玩家信息面板")] private RightPlayerPanel _rightPlayerPanel;

    private int _leftPosIndex = -1; // 左边玩家坐位索引
    private int _selfPosIndex = -1; // 自己的坐位索引
    private int _rightPosIndex = -1; // 右边玩家的坐位索引
    private Player _leftPlayerInfo = null; // 左边玩家信息
    private Player _selfPlayerInfo = null; // 自己的信息
    private Player _rightPlayerInfo = null; // 右边玩家信息
    private int _baseScore = 3; // 底分
    private int _multipleNum = 1; // 倍数
    private int _robTimes = 0; // 抢地主次数

    public override void Init() {
        // 监听服务器返回的匹配结果
        SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_MatchCode, OnMatchHandle);
    }

    /// <summary>
    /// 处理服务器响应的匹配结果
    /// </summary>
    private void OnMatchHandle(ByteString data) {
        MatchResponse response = MatchResponse.Parser.ParseFrom(data);
        // 根据返回的坐位索引号确认左右玩家
        switch (response.SelfPos) {
            case 0:
                _leftPosIndex = 2;
                _rightPosIndex = 1;
                break;
            case 1:
                _leftPosIndex = 0;
                _rightPosIndex = 2;
                break;
            case 2:
                _leftPosIndex = 1;
                _rightPosIndex = 0;
                break;
        }
        
        _selfPlayerInfo = response.Player.ElementAtOrDefault(response.SelfPos);
        _leftPlayerInfo = response.Player.ElementAtOrDefault(_leftPosIndex);
        _rightPlayerInfo = response.Player.ElementAtOrDefault(_rightPosIndex);
        
        // 更新UI
        _selfPlayerPanel.RefreshPanel(_selfPlayerInfo);
        _leftPlayerPanel.RefreshPanel(_leftPlayerInfo);
        _rightPlayerPanel.RefreshPanel(_rightPlayerInfo);
    }
}