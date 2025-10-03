using System.Collections.Generic;
using Google.Protobuf;

public class FightService {

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="room">房间</param>
    public void StartDispatchCardHandle(Room room) {
        // 清空玩家手牌
        foreach (var player in room.Players) {
            player.CardList.Clear();
        }
        // 清空底牌
        room.HoleCards.Clear();

        // 初始化54张牌
        List<Card> allCards = CardMgr.Instance.InitAllCards();
        // 设置3张底牌
        room.HoleCards.Add(allCards[0]);
        room.HoleCards.Add(allCards[1]);
        room.HoleCards.Add(allCards[2]);
        // 分发3副初始牌，每副17张
        for (int i = 3; i < allCards.Count; i++) {
            room.Players[i % 3].CardList.Add(allCards[i]);
        }

        // 将卡牌分发给用户
        var dispatchCardRes = new DispatchCardResponse { BaseScore = 3, Multiple = 1 };
        foreach (var p in room.Players) {
            dispatchCardRes.CardList.Clear();
            dispatchCardRes.CardList.AddRange(p.CardList);
            Cache.Instance.SessionDict[p.Id].SendData(NetDefine.CMD_DispatchCardCode, dispatchCardRes.ToByteString());
        }
    }

    /// <summary>
    /// 成为地主
    /// </summary>
    /// <param name="room">房间信息</param>
    public void BecomeLord(Room room) {
        // 设置出牌人坐位
        room.PendPos = room.CurLordPos;
        // 分发底牌
        var holeCards = room.HoleCards;
        room.Players[room.CurLordPos].CardList.AddRange(holeCards);
        // 修改房间状态为加倍
        room.RoomState = RoomState.Raise;

        var response = new BecomeLordResponse { LordPos = room.CurLordPos };
        response.HoleCards.AddRange(room.HoleCards);
        foreach (var p in room.Players) {
            Cache.Instance.SessionDict[p.Id].SendData(NetDefine.CMD_BecomeLordCode, response.ToByteString());
        }
    }

    /// <summary>
    /// 移除玩家手牌
    /// </summary>
    /// <param name="player">玩家</param>
    /// <param name="pendCards">打出的牌</param>
    /// <returns>true: 没牌了, false: 还有牌</returns>
    public bool RemovePlayerCards(Player player, List<Card> pendCards) {
        foreach (var card in pendCards) {
            for (int j = 0; j < player.CardList.Count; j++) {
                if (player.CardList[j].Point == card.Point && player.CardList[j].Suit == card.Suit) {
                    player.CardList.RemoveAt(j);
                    break;
                }
            }
        }

        return player.CardList.Count == 0;
    }
}