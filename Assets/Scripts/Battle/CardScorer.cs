using UnityEngine;

// pure scoring logic for the enemy AI — no MonoBehaviour, no state
public static class CardScorer
{
    const int HealUrgentWeight  = 50;
    const int HealNormalWeight  = 3;
    const int ShieldWeight      = 20;
    const int ShieldSaturation  = 3;
    const int DmgUrgentWeight   = 50;
    const int DmgNormalWeight   = 30;
    const int LowHpThreshold    = 3;
    const int DrawWeight        = 8;
    const int PlayAgainWeight   = 35;

    public static int Score(CardData card, int enemyHp, int playerHp, int healThreshold, int enemyShieldValue)
    {
        if (card == null) return -999;

        int score = 0;
        int heal      = Sum(card, EffectType.Heal);
        int shield    = Sum(card, EffectType.Shield);
        int dmg       = Sum(card, EffectType.Damage);
        int draw      = Sum(card, EffectType.Draw);
        int playAgain = Sum(card, EffectType.PlayAgain);

        score += enemyHp <= healThreshold ? heal * HealUrgentWeight : heal * HealNormalWeight;
        if (enemyShieldValue < ShieldSaturation) score += shield * ShieldWeight;
        score += playerHp <= LowHpThreshold ? dmg * DmgUrgentWeight : dmg * DmgNormalWeight;
        score += draw * DrawWeight;
        score += playAgain * PlayAgainWeight;
        score += Random.Range(0, 2);

        return score;
    }

    static int Sum(CardData card, EffectType type)
    {
        if (card?.effects == null) return 0;
        int total = 0;
        foreach (var e in card.effects)
            if (e != null && e.type == type) total += e.value;
        return total;
    }
}
