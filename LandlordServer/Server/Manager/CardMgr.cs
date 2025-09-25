using System;
using System.Collections.Generic;
using System.Linq;

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

    #region 牌型判断辅助函数

    /// <summary>
    /// 判断一个数的二进制形式是否全是相连的1组成，即0011111100这种形式
    /// </summary>
    private bool IsSuccessiveOne(long num, int min, int max) {
        //等比数列求和
        return num == ((1 << (max + 1)) - (1 << min));
    }

    /// <summary>
    /// <para>判断是否为三带、四带或什么都不带</para>
    /// 4个相同的牌可以统计出的次数为4; 
    /// 3个相同的牌可以统计出的次数为3; 
    /// 2个相同的牌可以统计出的次数为1;
    /// 其它为0
    /// 即：x带y对，那么count=x+y（x>=3）；x带y，那么count=x；如果x=4同时y=0那么这就是炸弹。
    /// </summary>
    private bool IsWith(List<CardEntity> cards, int count) {
        int r = 0, n = 0, p, k, tmp, code;
        for (int i = 0; i < cards.Count; i++) {
            // code: 牌码 枚举值+2=牌的实际点数 
            code = (int)cards[i].CardPoint + 2;
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
    /// 将卡牌进行分类
    /// dict[1]: 存放单张牌的集合
    /// dict[2]: 存放对子的集合
    /// dict[3]: 存放3张相同牌的集合
    /// dict[4]: 存放普通炸弹的集合
    /// </summary>
    private Dictionary<string, List<int>> Classify(List<CardEntity> cards) {
        int maxPoint = Convert.ToInt32(CardPoint.JokerBig + 1);
        // 创建空数组，以牌的点数为下标，牌的张数为值进行存储
        int[] cardRecord = new int[maxPoint];
        foreach (CardEntity card in cards) {
            cardRecord[Convert.ToInt32(card.CardPoint)]++;
        }

        Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
        for (int i = 1; i < maxPoint; ++i) {
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


    /// <summary>
    /// 单张牌
    /// </summary>
    public bool IsSingle(List<CardEntity> cards) {
        return cards.Count == 1;
    }

    /// <summary>
    /// 对子
    /// </summary>
    public bool IsPair(List<CardEntity> cards) {
        return cards.Count == 2 && cards[0].CardPoint == cards[1].CardPoint;
    }

    /// <summary>
    /// 3张相同的牌，3不带
    /// </summary>
    public bool IsTriple(List<CardEntity> cards) {
        return cards.Count == 3 && IsWith(cards, 3);
    }

    /// <summary>
    /// 三带一
    /// </summary>
    public bool IsTripleSingle(List<CardEntity> cards) {
        return cards.Count == 4 && IsWith(cards, 3);
    }

    /// <summary>
    /// 三带对
    /// </summary>
    public bool IsTriplePair(List<CardEntity> cards) {
        bool f = cards.Count == 5 && IsWith(cards, 4);
        if (f) {
            int code, min = 99, max = 0, sum = 0;
            for (int i = 0; i < cards.Count; i++) {
                code = 3 + ((int)cards[i].CardPoint) / 4;
                sum += code;
                if (code < min) {
                    min = code;
                }

                if (code > max) {
                    max = code;
                }
            }

            f = min * 3 + max * 2 == sum || min * 2 + max * 3 == sum;
        }

        return f;
    }

    /// <summary>
    /// 飞机
    /// </summary>
    public bool IsPlane(List<CardEntity> cards) {
        Dictionary<string, List<int>> classify = Classify(cards);
        List<int> list = classify["3"];
        if (classify["1"].Count == 0 && classify["2"].Count == 0 && list.Count >= 2 && classify["4"].Count == 0) {
            list.Sort();
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
    public bool IsPlaneTwoSingle(List<CardEntity> cards) {
        Dictionary<string, List<int>> classify = Classify(cards);
        List<int> list = classify["3"];
        if (classify["1"].Count == 2 && classify["2"].Count == 0 && list.Count == 2 && classify["4"].Count == 0) {
            list.Sort();
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
    public bool IsPlaneTwoPair(List<CardEntity> cards) {
        Dictionary<string, List<int>> classify = Classify(cards);
        List<int> list = classify["3"];
        if (classify["1"].Count == 0 && classify["2"].Count == 2 && list.Count == 2 && classify["4"].Count == 0) {
            list.Sort();
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
    public bool IsSeqPair(List<CardEntity> cards) {
        int count = cards.Count;
        int seqSingle0 = 0, seqSingle1 = 0, min = 99;
        if (count >= 6 && count % 2 == 0) {
            for (int i = 0; i < cards.Count; i++) {
                int code = (int)cards[i].CardPoint + 2;
                if (code >= 14) return false;
                if (code < min) {
                    min = code;
                }

                int temp = 1 << code;
                if ((seqSingle0 & temp) == 0) {
                    seqSingle0 |= temp;
                } else if ((seqSingle1 & temp) == 0) {
                    seqSingle1 |= temp;
                } else {
                    return false;
                }
            }

            return seqSingle0 == seqSingle1 && IsSuccessiveOne(seqSingle0, min, min + cards.Count / 2 - 1);
        }

        return false;
    }

    /// <summary>
    /// 顺子
    /// </summary>
    public bool IsSeqSingle(List<CardEntity> cards) {
        if (cards.Count >= 5) {
            int tmp = 0, temp, code, min = 99;
            for (int i = 0; i < cards.Count; i++) {
                code = (int)cards[i].CardPoint + 2;
                if (code < min) {
                    min = code;
                }

                temp = 1 << code;
                if ((tmp & temp) != 0 || code > 14) {
                    return false;
                } // 顺子不能包含王和2

                tmp |= temp;
            }

            return (tmp >>= (min + cards.Count)) == 0;
        }

        return false;
    }

    /// <summary>
    /// 炸弹
    /// </summary>
    public bool IsBomb(List<CardEntity> cards) {
        return cards.Count == 4 && IsWith(cards, 4);
    }

    /// <summary>
    /// 炸弹带一对
    /// </summary>
    public bool IsBombPair(List<CardEntity> cards) {
        Dictionary<string, List<int>> classify = Classify(cards);
        return classify["1"].Count == 0 && classify["2"].Count == 1 && classify["3"].Count == 0 && classify["4"].Count == 1;
    }

    /// <summary>
    /// 炸弹带两单
    /// </summary>
    public bool IsBombTwoSingle(List<CardEntity> cards) {
        Dictionary<string, List<int>> classify = Classify(cards);

        if (classify["1"].Count == 2 && classify["2"].Count == 0 && classify["3"].Count == 0 &&
            classify["4"].Count == 1) {
            classify["1"].Sort();
            // 炸弹带的两张单牌不能是大王和小王
            if (classify["1"][0] != (int)CardPoint.JokerSmall && classify["1"][1] != (int)CardPoint.JokerBig) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 王炸
    /// </summary>
    public bool IsBombJokers(List<CardEntity> cards) {
        if (cards.Count != 2) return false;
        // 升序排序
        List<CardEntity> list = cards.OrderBy(c => c.CardPoint).ThenBy(c => c.CardSuit).ToList();
        return list[0].CardPoint == CardPoint.JokerSmall && list[1].CardPoint == CardPoint.JokerBig;
    }

    /// <summary>
    /// 王炸带两张
    /// </summary>
    public bool IsBombJokersWithTwo(List<CardEntity> cards) {
        if (cards.Count != 4) return false;
        // 升序排序
        List<CardEntity> list = cards.OrderBy(c => c.CardPoint).ThenBy(c => c.CardSuit).ToList();
        return list[2].CardPoint == CardPoint.JokerSmall && list[3].CardPoint == CardPoint.JokerBig;
    }
}