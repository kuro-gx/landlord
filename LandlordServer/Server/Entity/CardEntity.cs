using System;

/// <summary>
/// 卡牌对象
/// </summary>
public class CardEntity : IEquatable<CardEntity> {
    /// <summary>
    /// 花色
    /// </summary>
    public CardSuit CardSuit { get; set; }

    /// <summary>
    /// 点数
    /// </summary>
    public CardPoint CardPoint { get; set; }

    public CardEntity() {
    }

    public CardEntity(CardPoint point, CardSuit suit) {
        CardPoint = point;
        CardSuit = suit;
    }

    /// <summary>
    /// 比较两种卡牌是否相等
    /// </summary>
    public bool Equals(CardEntity other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return CardPoint == other.CardPoint && CardSuit == other.CardSuit;
    }

    public override int GetHashCode() {
        unchecked {
            return Convert.ToInt32(CardPoint) * 100 + Convert.ToInt32(CardSuit);
        }
    }

    public static bool operator ==(CardEntity left, CardEntity right) {
        return Equals(left, right);
    }

    public static bool operator !=(CardEntity left, CardEntity right) {
        return !Equals(left, right);
    }

    public override string ToString() {
        return $"[{Convert.ToInt32(CardPoint)}, {nameof(CardSuit)}: {CardSuit}]";
    }
}
