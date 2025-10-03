using System;
using System.Collections.Generic;

public class PlayHand {
    // 牌型
    public CardType Type { get; set; }

    // 牌型比较的关键点数
    public CardPoint Point { get; set; }
}

public class CardMgr : Singleton<CardMgr> {
    /// <summary>
    /// 初始化54张扑克牌，并进行洗牌
    /// </summary>
    public List<Card> InitAllCards() {
        List<Card> cards = new List<Card>(54);

        for (int i = (int)CardSuit.Diamond; i <= (int)CardSuit.Spade; i++) {
            for (int j = (int)CardPoint.Three; j < (int)CardPoint.JokerSmall; j++) {
                cards.Add(new Card { Suit = (CardSuit)i, Point = (CardPoint)j });
            }
        }

        cards.Add(new Card { Suit = CardSuit.SuitNone, Point = CardPoint.JokerSmall });
        cards.Add(new Card { Suit = CardSuit.SuitNone, Point = CardPoint.JokerBig });

        Random random = new Random((int)DateTime.Now.Ticks);
        for (int i = cards.Count - 1; i >= 0; i--) {
            int j = random.Next(0, i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }

        return cards;
    }

    /// <summary>
    /// 获取出牌的类型
    /// </summary>
    public PlayHand GetCardType(List<Card> cards) {
        var playHand = new PlayHand { Type = CardType.HandUnknown, Point = CardPoint.PointNone };
        if (cards == null || cards.Count == 0) {
            playHand.Type = CardType.HandPass;
        } else if (IsSingle(cards)) { // 单牌
            playHand.Type = CardType.HandSingle;
            playHand.Point = cards[0].Point;
        } else if (IsPair(cards)) { // 对
            playHand.Type = CardType.HandPair;
            playHand.Point = cards[0].Point;
        } else if (IsTriple(cards)) { // 3张
            playHand.Type = CardType.HandTriple;
            playHand.Point = cards[0].Point;
        } else if (IsTripleSingle(cards)) { // 三带一
            playHand.Type = CardType.HandTripleSingle;
            playHand.Point = FindPointFromCards(cards, CardType.HandTripleSingle);
        } else if (IsTriplePair(cards)) { // 三带对
            playHand.Type = CardType.HandTriplePair;
            playHand.Point = FindPointFromCards(cards, CardType.HandTriplePair);
        } else if (IsPlane(cards, playHand)) { // 飞机不带
            playHand.Type = CardType.HandPlane;
        } else if (IsPlaneSingle(cards, playHand)) { // 飞机带单
            playHand.Type = CardType.HandPlaneSingle;
        } else if (IsPlanePair(cards, playHand)) { // 飞机带对
            playHand.Type = CardType.HandPlanePair;
        } else if (IsSeqPair(cards, playHand)) { // 连对
            playHand.Type = CardType.HandSeqPair;
        } else if (IsSeqSingle(cards, playHand)) { // 顺子
            playHand.Type = CardType.HandSeqSingle;
        } else if (IsBomb(cards)) { // 炸弹
            playHand.Type = CardType.HandBomb;
            playHand.Point = cards[0].Point;
        } else if (IsBombPair(cards)) { // 炸弹带一对
            playHand.Type = CardType.HandBombPair;
            playHand.Point = FindPointFromCards(cards, CardType.HandBombPair);
        } else if (IsBombTwoSingle(cards)) { // 炸弹带两单
            playHand.Type = CardType.HandBombTwoSingle;
            playHand.Point = FindPointFromCards(cards, CardType.HandBombTwoSingle);
        } else if (IsBombTwoPair(cards, playHand)) { // 炸弹带两对
            playHand.Type = CardType.HandBombTwoPair;
        } else if (IsBombJokers(cards)) { // 王炸
            playHand.Type = CardType.HandBombJokers;
        } else if (IsBombJokersPair(cards)) { // 王炸带一对
            playHand.Type = CardType.HandBombJokersPair;
        } else if (IsBombJokersTwoSingle(cards)) { // 王炸带两单
            playHand.Type = CardType.HandBombJokersTwoSingle;
        } else if (IsBombJokersTwoPair(cards)) { // 王炸带两对
            playHand.Type = CardType.HandBombJokersTwoPair;
        }

        return playHand;
    }

    /// <summary>
    /// 出牌大小比较
    /// </summary>
    /// <param name="cards">上一轮对手出的牌</param>
    /// <param name="myCards">我打出的牌</param>
    /// <returns>我能否压住上一轮的牌</returns>
    public bool CanBeat(List<Card> cards, List<Card> myCards) {
        // 我的牌型
        var playHand = GetCardType(myCards);
        if (playHand.Type == CardType.HandUnknown) {
            return false;
        }

        // 对手的牌型
        var other = GetCardType(cards);
        // 对手放弃出牌
        if (other.Type == CardType.HandPass) {
            return true;
        }

        // 我的牌是王炸
        if (playHand.Type == CardType.HandBombJokers) {
            return true;
        }

        // 我的牌是炸弹，并且对方的牌型枚举值在HandSingle(2)-HandBombJokersTwoPair(19)之间
        if (playHand.Type == CardType.HandBomb && other.Type >= CardType.HandSingle &&
            other.Type <= CardType.HandBombJokersTwoPair) {
            return true;
        }

        // 双方牌型一致
        if (playHand.Type == other.Type) {
            // 双方的牌数量必须一致
            if (cards.Count != myCards.Count) {
                return false;
            }

            // 如果是连对、顺子，将所有牌的点数累加再进行判断
            if (playHand.Type == CardType.HandSeqSingle || playHand.Type == CardType.HandSeqPair) {
                int myTotalPoint = 0, otherTotalPoint = 0;
                for (var i = 0; i < cards.Count; i++) {
                    myTotalPoint += (int)myCards[i].Point;
                    otherTotalPoint += (int)cards[i].Point;
                }

                return myTotalPoint > otherTotalPoint;
            }

            return playHand.Point > other.Point;
        }

        return false;
    }

    /// <summary>
    /// 单张牌
    /// </summary>
    public bool IsSingle(List<Card> cards) {
        return cards.Count == 1;
    }

    /// <summary>
    /// 对子
    /// </summary>
    public bool IsPair(List<Card> cards) {
        return cards.Count == 2 && cards[0].Point == cards[1].Point;
    }

    /// <summary>
    /// 3张相同的牌，3不带
    /// </summary>
    public bool IsTriple(List<Card> cards) {
        return cards.Count == 3 && IsWith(cards, 3);
    }

    /// <summary>
    /// 三带一
    /// </summary>
    public bool IsTripleSingle(List<Card> cards) {
        return cards.Count == 4 && IsWith(cards, 3);
    }

    /// <summary>
    /// 三带对
    /// </summary>
    public bool IsTriplePair(List<Card> cards) {
        bool res = cards.Count == 5 && IsWith(cards, 4);
        if (res) {
            int code, min = 99, max = 0, sum = 0;
            foreach (var card in cards) {
                code = 3 + (int)card.Point / 4;
                sum += code;
                if (code < min) {
                    min = code;
                }

                if (code > max) {
                    max = code;
                }
            }

            res = min * 3 + max * 2 == sum || min * 2 + max * 3 == sum;
        }

        return res;
    }

    /// <summary>
    /// 飞机不带
    /// </summary>
    public bool IsPlane(List<Card> cards, PlayHand playHand) {
        var classify = Classify(cards);
        var list = classify["3"];
        if (classify["1"].Count == 0 && classify["2"].Count == 0 && list.Count >= 2 && classify["4"].Count == 0) {
            // 两种牌之间点数相差为1
            for (int i = 1; i < list.Count; i++) {
                if (list[i] - list[i - 1] != 1) {
                    return false;
                }
            }

            // 最大的点数小于 2
            if (list[list.Count - 1] < (int)CardPoint.Two) {
                playHand.Point = (CardPoint)list[0];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 飞机带单
    /// </summary>
    public bool IsPlaneSingle(List<Card> cards, PlayHand playHand) {
        var classify = Classify(cards);
        var list = classify["3"];
        if (classify["1"].Count >= 2 && classify["2"].Count == 0 && list.Count >= 2 && classify["4"].Count == 0) {
            // 单牌的数量和3张牌的数量不等
            if (classify["1"].Count != list.Count) {
                return false;
            }
            
            // 两种牌之间点数相差不为1
            for (int i = 1; i < list.Count; i++) {
                if (list[i] - list[i - 1] != 1) {
                    return false;
                }
            }
            
            // 最大的点数小于2，2张单牌不做限制，可以是大小王
            if (list[list.Count - 1] < (int)CardPoint.Two) {
                playHand.Point = (CardPoint)list[0];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 飞机带对
    /// </summary>
    public bool IsPlanePair(List<Card> cards, PlayHand playHand) {
        var classify = Classify(cards);
        var list = classify["3"];
        if (classify["1"].Count == 0 && classify["2"].Count >= 2 && list.Count >= 2 && classify["4"].Count == 0) {
            // 单牌的数量和3张牌的数量不等
            if (classify["2"].Count != list.Count) {
                return false;
            }
            
            // 两种牌之间点数相差不为1
            for (int i = 1; i < list.Count; i++) {
                if (list[i] - list[i - 1] != 1) {
                    return false;
                }
            }
            
            // 最大的点数小于2
            if (list[list.Count - 1] < (int)CardPoint.Two) {
                playHand.Point = (CardPoint)list[0];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 连对
    /// </summary>
    public bool IsSeqPair(List<Card> cards, PlayHand playHand) {
        if (cards.Count < 6) return false;
        
        var classify = Classify(cards);
        var list = classify["2"];
        if (classify["1"].Count == 0 && list.Count >= 3 && classify["3"].Count == 0 && classify["4"].Count == 0) {
            // 连对最后1张点数 - 第1张点数 = 连对数量 - 1 ：334455 -> 5-3 = 3-1
            if (list[list.Count - 1] - list[0] == list.Count - 1 && 
                list[0] >= (int)CardPoint.Three && list[list.Count - 1] < (int)CardPoint.Two) {
                playHand.Point = (CardPoint)list[0];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 顺子
    /// </summary>
    public bool IsSeqSingle(List<Card> cards, PlayHand playHand) {
        if (cards.Count < 5) return false;
        
        var classify = Classify(cards);
        var list = classify["1"];
        if (list.Count >= 5 && classify["2"].Count == 0 && classify["3"].Count == 0 && classify["4"].Count == 0) {
            // 顺子最后1张点数 - 第1张点数 = 顺子数量 - 1 ：34567 -> 7-3 = 5-1
            if (list[list.Count - 1] - list[0] == list.Count - 1 && 
                list[0] >= (int)CardPoint.Three && list[list.Count - 1] < (int)CardPoint.Two) {
                playHand.Point = (CardPoint)list[0];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 炸弹
    /// </summary>
    public bool IsBomb(List<Card> cards) {
        return cards.Count == 4 && IsWith(cards, 4);
    }

    /// <summary>
    /// 炸弹带一对
    /// </summary>
    public bool IsBombPair(List<Card> cards) {
        return cards.Count == 6 && IsWith(cards, 5);
    }

    /// <summary>
    /// 炸弹带两对
    /// </summary>
    public bool IsBombTwoPair(List<Card> cards, PlayHand playHand) {
        if (cards.Count != 8) return false;
        
        var classify = Classify(cards);
        if (classify["1"].Count == 0 && classify["2"].Count == 2 && classify["3"].Count == 0 && classify["4"].Count == 1) {
            playHand.Point = (CardPoint)classify["4"][0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// 炸弹带两单
    /// </summary>
    public bool IsBombTwoSingle(List<Card> cards) {
        return cards.Count == 6 && IsWith(cards, 4);
    }

    /// <summary>
    /// 王炸
    /// </summary>
    public bool IsBombJokers(List<Card> cards) {
        if (cards.Count != 2) return false;
        // 小王：16 + 大王17 == 33
        return (int)cards[0].Point + (int)cards[1].Point == 33;
    }

    /// <summary>
    /// 王炸带一对
    /// </summary>
    public bool IsBombJokersPair(List<Card> cards) {
        if (cards.Count != 4) return false;

        var classify = Classify(cards);
        if (classify["1"].Count == 2 && classify["2"].Count == 1 && classify["3"].Count == 0 && classify["4"].Count == 0) {
            if (classify["1"][0] + classify["1"][1] == 33) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 王炸带两张单
    /// </summary>
    public bool IsBombJokersTwoSingle(List<Card> cards) {
        if (cards.Count != 4) return false;
        var classify = Classify(cards);
        if (classify["1"].Count == 4 && classify["2"].Count == 0 && classify["3"].Count == 0 && classify["4"].Count == 0) {
            if (classify["1"][2] + classify["1"][3] == 33) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 王炸带两对
    /// </summary>
    public bool IsBombJokersTwoPair(List<Card> cards) {
        if (cards.Count != 6) return false;

        var classify = Classify(cards);
        if (classify["1"].Count == 2 && classify["2"].Count == 2 && classify["3"].Count == 0 && classify["4"].Count == 0) {
            if (classify["1"][0] + classify["1"][1] == 33) {
                return true;
            }
        }

        return false;
    }

    #region 牌型判断辅助函数

    /// <summary>
    /// <para>判断是否为三带、四带或什么都不带</para>
    /// 4个相同的牌可以统计出的次数为4; 
    /// 3个相同的牌可以统计出的次数为3; 
    /// 2个相同的牌可以统计出的次数为1;
    /// 其它为0
    /// 即：x带y对，那么count=x+y（x>=3）；x带y，那么count=x；如果x=4同时y=0那么这就是炸弹。
    /// </summary>
    private bool IsWith(List<Card> cards, int count) {
        int r = 0, n = 0, p, k, tmp, code;
        for (int i = 0; i < cards.Count; i++) {
            // code: 牌码(牌的实际点数)
            code = (int)cards[i].Point;
            tmp = 1 << code;
            p = (r & tmp) >> code; // 判断第code位上是否为1
            k = (n & tmp) >> code;
            count -= p + k;
            if (p == 1) {
                n |= tmp;
            }

            if (k == 1) {
                n &= ~tmp;
            }

            r |= tmp;
        }

        return count == 0;
    }

    /// <summary>
    /// 将卡牌进行分类，dict的value里面的值存储的是 CardPoint
    /// dict[1]: 存放单张牌的集合
    /// dict[2]: 存放对子的集合
    /// dict[3]: 存放3张相同牌的集合
    /// dict[4]: 存放普通炸弹的集合
    /// </summary>
    private Dictionary<string, List<int>> Classify(List<Card> cards) {
        int maxPoint = Convert.ToInt32(CardPoint.JokerBig + 1);
        // 创建空数组，以牌的点数为下标，牌的张数为值进行存储
        int[] cardRecord = new int[maxPoint];
        foreach (var card in cards) {
            cardRecord[(int)card.Point]++;
        }

        var dict = new Dictionary<string, List<int>> {
            { "1", new List<int>(20) },
            { "2", new List<int>(10) },
            { "3", new List<int>(10) },
            { "4", new List<int>(5) }
        };
        for (int i = (int)CardPoint.Three; i < maxPoint; ++i) {
            if (cardRecord[i] == 1) {
                dict["1"].Add(i);
            } else if (cardRecord[i] == 2) {
                dict["2"].Add(i);
            } else if (cardRecord[i] == 3) {
                dict["3"].Add(i);
            } else if (cardRecord[i] == 4) {
                dict["4"].Add(i);
            }
        }

        return dict;
    }

    /// <summary>
    /// 根据牌型取得该牌型比较大小的关键点数
    /// </summary>
    /// <param name="cards">卡牌列表</param>
    /// <param name="type">卡牌所属的牌型</param>
    /// <returns>关键点数</returns>
    private CardPoint FindPointFromCards(List<Card> cards, CardType type) {
        var dict = Classify(cards);
        if (type == CardType.HandBombPair || type == CardType.HandBombTwoSingle) {
            return (CardPoint)dict["4"][0];
        }

        if (type == CardType.HandTripleSingle || type == CardType.HandTriplePair) {
            return (CardPoint)dict["3"][0];
        }

        return CardPoint.PointNone;
    }

    #endregion
}