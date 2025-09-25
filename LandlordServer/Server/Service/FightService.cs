using System.Collections.Generic;
using Google.Protobuf.Collections;

public class FightService {
    private List<Card> _last3Cards; // 3张底牌

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="players">房间内的玩家</param>
    public void StartDispatchCardHandle(RepeatedField<Player> players) {
        // 清空玩家手牌
        foreach (Player player in players) {
            player.CardList.Clear();
        }

        // 初始化54张牌
        List<Card> allCards = CardMgr.Instance.InitAllCards();
        // 设置3张底牌
        _last3Cards = new List<Card> { allCards[0], allCards[1], allCards[2] };
        // 分发3副初始牌，每副17张
        for (int i = 3; i < allCards.Count; i++) {
            players[i % 3].CardList.Add(allCards[i]);
        }
    }
}