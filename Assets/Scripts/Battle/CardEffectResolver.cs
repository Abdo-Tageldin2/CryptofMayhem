using UnityEngine;

// applies card effects for both player and enemy — one place instead of two
public class CardEffectResolver : MonoBehaviour
{
    private BattleManager bm;
    private ShieldSystem shields;
    private DeckManager deck;

    void Start()
    {
        bm      = GetComponent<BattleManager>();
        shields = GetComponent<ShieldSystem>();
        deck    = bm.deckManager;
    }

    // returns how many extra actions the card granted (from PlayAgain effects)
    public int ApplyPlayerCard(CardData card)
    {
        int extraActions = 0;

        foreach (var e in card.effects)
        {
            if (e == null) continue;

            switch (e.type)
            {
                case EffectType.Damage:    bm.enemyHp -= shields.AbsorbEnemyDamage(e.value);              break;
                case EffectType.Heal:      bm.playerHp = Mathf.Min(bm.playerMaxHp, bm.playerHp + e.value); break;
                case EffectType.Shield:    shields.AddPlayerShield(e.value);                                break;
                case EffectType.Draw:      deck.DrawCards(Owner.Player, e.value);                           break;
                case EffectType.PlayAgain: extraActions += e.value;                                         break;
            }
        }

        bm.enemyHp = Mathf.Max(0, bm.enemyHp);

        return extraActions;
    }

    // returns how many extra actions the card granted
    public int ApplyEnemyCard(CardData card)
    {
        int extraActions = 0;

        foreach (var e in card.effects)
        {
            if (e == null) continue;

            switch (e.type)
            {
                case EffectType.Heal:      bm.enemyHp = Mathf.Min(bm.enemyMaxHp, bm.enemyHp + e.value);                     break;
                case EffectType.Shield:    shields.AddEnemyShield(e.value);                                                    break;
                case EffectType.Damage:
                    int leftover = shields.AbsorbPlayerDamage(e.value);
                    bm.playerHp = Mathf.Max(0, bm.playerHp - leftover);
                    break;
                case EffectType.Draw:      deck.DrawCards(Owner.Enemy, e.value);                                                break;
                case EffectType.PlayAgain: extraActions += e.value;                                                             break;
            }
        }

        return extraActions;
    }
}
