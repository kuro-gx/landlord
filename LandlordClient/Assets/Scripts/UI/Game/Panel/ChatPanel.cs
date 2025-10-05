using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : UIBase {
    [SerializeField, Header("快捷聊天列表容器")] private RectTransform chatItemBox;

    #region 聊天语音

    private readonly List<string> _characterSound0 = new() {
        "大家好！很高兴见到各位！",
        "快点啊！我等得花儿都谢了！",
        "你的牌打得也太好啦！",
        "解释什么？彪悍的人生不需要解释！",
        "你今天晚上必须给我上线，否则我就把你的名字写到碑上去！",
        "交个朋友怎么样？",
        "这钻石可能是假的，把你的验钞机拿过来！",
        "吵架总是不好的，不如…干脆决斗？",
        "看着我的签名干嘛？难道…你想暗算我？",
        "大家好！我是非非非常美丽的巨兔12138！"
    };

    private readonly List<string> _characterSound1 = new() {
        "大家好！我是最强侦探福尔摩汪！",
        "快点啊！我等得花儿都谢了！",
        "你的牌打得也太好啦！",
        "不要走！和我一起决战到天亮！",
        "你愿意和天底下最帅的汪汪交个朋友吗？",
        "惊不惊喜，意不意外？",
        "还有这种操作？",
        "真相只有一个，这局我赢定了！"
    };

    private readonly List<string> _characterSound2 = new() {
        "闻君之名，如雷贯耳！",
        "速兮！我等得花儿都谢了！",
        "打得不错哦，哼哼！",
        "不要争执不休，专心游戏吧！",
        "嘤嘤嘤…断线了，网络真差！",
        "失礼了，和妾身再来一局吧！",
        "你也是闭月羞花的MM吗？",
        "花有再开日，人有重逢时！"
    };

    #endregion

    protected override void Init() {
    }

    /// <summary>
    /// 初始化快捷聊天项
    /// </summary>
    /// <param name="pos">坐位</param>
    public void InitChatItems(int pos) {
        var chatItem = Resources.Load<ChatItem>("Prefabs/ChatItem");
        if (chatItem == null) {
            return;
        }

        switch (pos) {
            case 0: {
                for (var i = 0; i < _characterSound0.Count; i++) {
                    if (chatItemBox.transform.childCount >= 10) return;
                    var item = Instantiate(chatItem, chatItemBox);
                    item.SetText(_characterSound0[i]);
                    item.name = i.ToString();
                    var button = item.GetComponent<Button>();
                    int chatId = i;
                    button.onClick.AddListener(() => ChatRequest(pos, chatId));
                }

                break;
            }
            case 1: {
                for (var i = 0; i < _characterSound1.Count; i++) {
                    if (chatItemBox.transform.childCount >= 8) return;
                    var item = Instantiate(chatItem, chatItemBox);
                    item.SetText(_characterSound1[i]);
                    item.name = i.ToString();
                    var button = item.GetComponent<Button>();
                    int chatId = i;
                    button.onClick.AddListener(() => ChatRequest(pos, chatId));
                }

                break;
            }
            case 2: {
                for (var i = 0; i < _characterSound2.Count; i++) {
                    if (chatItemBox.transform.childCount >= 8) return;
                    var item = Instantiate(chatItem, chatItemBox);
                    item.SetText(_characterSound2[i]);
                    item.name = i.ToString();
                    var button = item.GetComponent<Button>();
                    int chatId = i;
                    button.onClick.AddListener(() => ChatRequest(pos, chatId));
                }

                break;
            }
        }
    }

    /// <summary>
    /// 发送聊天项
    /// </summary>
    /// <param name="pos">玩家坐位</param>
    /// <param name="chatId">聊天项索引</param>
    private void ChatRequest(int pos, int chatId) {
        var form = new ChatForm {
            Pos = pos,
            ChatId = chatId
        };

        NetSocketMgr.Client.SendData(NetDefine.CMD_ChatCode, form.ToByteString());
        Show(false);
    }
}