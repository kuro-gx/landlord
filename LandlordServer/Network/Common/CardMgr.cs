using System;
using System.Collections.Generic;

public class CardMgr : Singleton<CardMgr> {
    /// <summary>
    /// 初始化54张扑克牌，并进行洗牌
    /// </summary>
    /// <returns></returns>
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
    public CardType GetCardType(List<Card> cards) {
        CardType type = CardType.HandUnknown;
        if (cards == null || cards.Count == 0) {
            type = CardType.HandPass;
        } else if (IsSingle(cards)) {
            type = CardType.HandSingle;
        } else if (IsPair(cards)) {
            type = CardType.HandPair;
        } else if (IsTriple(cards)) {
            type = CardType.HandTriple;
        } else if (IsTripleSingle(cards)) {
            type = CardType.HandTripleSingle;
        } else if (IsTriplePair(cards)) {
            type = CardType.HandTriplePair;
        } else if (IsPlane(cards)) {
            type = CardType.HandPlane;
        } else if (IsPlaneTwoSingle(cards)) {
            type = CardType.HandPlaneTwoSingle;
        } else if (IsPlaneTwoPair(cards)) {
            type = CardType.HandPlaneTwoPair;
        } else if (IsSeqPair(cards)) {
            type = CardType.HandSeqPair;
        } else if (IsSeqSingle(cards)) {
            type = CardType.HandSeqSingle;
        } else if (IsBomb(cards)) {
            type = CardType.HandBomb;
        } else if (IsBombPair(cards)) {
            type = CardType.HandBombPair;
        } else if (IsBombTwoSingle(cards)) {
            type = CardType.HandBombTwoSingle;
        } else if (IsBombTwoPair(cards)) {
            type = CardType.HandBombTwoPair;
        } else if (IsBombJokers(cards)) {
            type = CardType.HandBombJokers;
        } else if (IsBombJokersPair(cards)) {
            type = CardType.HandBombJokersPair;
        } else if (IsBombJokersTwoSingle(cards)) {
            type = CardType.HandBombJokersTwoSingle;
        } else if (IsBombJokersTwoPair(cards)) {
            type = CardType.HandBombJokersTwoPair;
        }

        return type;
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
    public bool IsPlane(List<Card> cards) {
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
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 飞机带两张单
    /// </summary>
    public bool IsPlaneTwoSingle(List<Card> cards) {
        var classify = Classify(cards);
        var list = classify["3"];
        if (classify["1"].Count == 2 && classify["2"].Count == 0 && list.Count == 2 && classify["4"].Count == 0) {
            // 飞机牌之间点数相差为1，且最大的点数小于2；2张单牌不做限制，可以是大小王
            if (list[1] - list[0] == 1 && list[1] < (int)CardPoint.Two) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 飞机带两对
    /// </summary>
    public bool IsPlaneTwoPair(List<Card> cards) {
        var classify = Classify(cards);
        var list = classify["3"];
        if (classify["1"].Count == 0 && classify["2"].Count == 2 && list.Count == 2 && classify["4"].Count == 0) {
            // 飞机牌之间点数相差为1，且最大的点数小于2
            if (list[1] - list[0] == 1 && list[1] < (int)CardPoint.Two) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 连对
    /// </summary>
    public bool IsSeqPair(List<Card> cards) {
        int count = cards.Count;
        int seqPair0 = 0, seqPair1 = 0, min = 99;
        if (count >= 6 && count % 2 == 0) {
            foreach (var t in cards) {
                int code = 3 + ((int)t.Point) / 4;
                if (code >= 14) return false;
                if (code < min) {
                    min = code;
                }

                int temp = 1 << code;
                if ((seqPair0 & temp) == 0) {
                    seqPair0 |= temp;
                } else if ((seqPair1 & temp) == 0) {
                    seqPair1 |= temp;
                } else {
                    return false;
                }
            }

            return seqPair0 == seqPair1 && IsSuccessiveOne(seqPair0, min, min + cards.Count / 2 - 1);
        }

        return false;
    }

    /// <summary>
    /// 顺子
    /// </summary>
    public bool IsSeqSingle(List<Card> cards) {
        if (cards.Count >= 5) {
            int seqSingle = 0, temp, code, min = 99;
            foreach (var card in cards) {
                code = 3 + (int)card.Point / 4;
                if (code < min) {
                    min = code;
                }

                temp = 1 << code;
                // 顺子不能包含王和2
                if ((seqSingle & temp) != 0 || code > 14) {
                    return false;
                }

                seqSingle |= temp;
            }

            return IsSuccessiveOne(seqSingle, min, min + cards.Count - 1);
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
    public bool IsBombTwoPair(List<Card> cards) {
        bool res = cards.Count == 8 && IsWith(cards, 6);
        // 由于四带两对和飞机带两张的条件一致，需要再进一步杨筛选
        if (res) {
            int tmp = 0, code, count = 0;
            for (int i = 0; i < cards.Count; i++) {
                code = 3 + (int)cards[i].Point / 4;
                tmp |= 1 << code;
            }

            while (tmp != 0) {
                count += (tmp & 1);
                tmp >>= 1;
            }

            res = count == 3;
        }

        return res;
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
    /// 判断一个数的二进制形式是否全是相连的1组成，即0011111100这种形式
    /// </summary>
    private bool IsSuccessiveOne(long num, int min, int max) {
        // 等比数列求和
        return num == (1 << (max + 1)) - (1 << min);
    }

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

    #endregion
}