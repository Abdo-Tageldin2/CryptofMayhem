using UnityEngine;

// handles the enemy's turn — drawing, picking cards, applying effects
public class EnemyAI : MonoBehaviour
{
    [Range(1, 10)] public int healThreshold = 5;
    [Range(1, 10)] public int maxPlaysPerTurn = 4;

    [SerializeField, Range(0f, 2f)] private float firstPlayDelay = 0.35f;
    [SerializeField, Range(0f, 2f)] private float cardPlayDelay  = 0.55f;

    int actionsLeft;
    int playsThisTurn;

    BattleManager bm;
    ShieldSystem shields;
    CardEffectResolver resolver;
    DeckManager deck;

    void Awake()
    {
        bm       = GetComponent<BattleManager>();
        shields  = GetComponent<ShieldSystem>();
        resolver = GetComponent<CardEffectResolver>();
    }

    void Start()
    {
        deck = bm.deckManager;
    }

    public void StartTurn()
    {
        actionsLeft   = bm.actionsPerTurn;
        playsThisTurn = 0;

        deck.DrawCards(Owner.Enemy, bm.drawPerTurn);
        if (deck.HandCount(Owner.Enemy) == 0)
            deck.DrawCards(Owner.Enemy, bm.emptyHandDraw);

        Invoke(nameof(PlayLoop), firstPlayDelay);
    }

    CardData PickBestCard()
    {
        Transform hand = deck.enemyHandContainer;
        if (hand == null || hand.childCount == 0) return null;

        CardData best = null;
        int bestScore = int.MinValue;
        int shieldVal = shields.GetEnemyShieldValue();

        for (int i = 0; i < hand.childCount; i++)
        {
            CardView cv = hand.GetChild(i).GetComponent<CardView>();
            if (cv == null || cv.data == null) continue;

            int s = CardScorer.Score(cv.data, bm.enemyHp, bm.playerHp, healThreshold, shieldVal);
            if (s > bestScore)
            {
                bestScore = s;
                best = cv.data;
            }
        }

        return best;
    }

    void PlayLoop()
    {
        if (bm.playerHp <= 0 || bm.enemyHp <= 0) { bm.EndEnemyTurn(); return; }
        if (actionsLeft <= 0 || playsThisTurn >= maxPlaysPerTurn) { bm.EndEnemyTurn(); return; }

        CardData chosen = PickBestCard();

        if (chosen == null) { bm.EndEnemyTurn(); return; }

        if (PlayedCardFXController.I != null && deck.enemyHandContainer != null)
        {
            Vector3 pos = deck.enemyHandContainer.position;
            PlayedCardFXController.I.AddPlayedCardAnimatedFromWorldPos(pos, chosen.artwork);
        }

        actionsLeft += resolver.ApplyEnemyCard(chosen);
        deck.Discard(Owner.Enemy, chosen);
        deck.RemoveSpecificCardFromHandUI(Owner.Enemy, chosen);

        actionsLeft--;
        playsThisTurn++;

        bm.RefreshUI();
        Invoke(nameof(PlayLoop), cardPlayDelay);
    }

}
