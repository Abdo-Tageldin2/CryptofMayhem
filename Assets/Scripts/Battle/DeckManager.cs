using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Deck Assets")]
    public DeckData playerDeckData;
    public DeckData enemyDeckData;

    [Header("UI References")]
    public Transform playerHandContainer;
    public Transform enemyHandContainer;
    public CardView cardPrefab;

    [Header("Enemy Card Back")]
    public Sprite enemyCardBackSprite;

    private CardPool cardPool;

    // Internal piles
    private readonly List<CardData> playerDeck = new List<CardData>();
    private readonly List<CardData> playerDiscard = new List<CardData>();

    private readonly List<CardData> enemyDeck = new List<CardData>();
    private readonly List<CardData> enemyDiscard = new List<CardData>();

    public void Setup()
    {
        cardPool = GetComponent<CardPool>();
        if (cardPool != null)
        {
            cardPool.cardPrefab = cardPrefab;
            cardPool.FillPool();
        }

        BuildAndShuffle(Owner.Player);
        BuildAndShuffle(Owner.Enemy);

        ClearHandUI(Owner.Player);
        ClearHandUI(Owner.Enemy);
    }

    public int HandCount(Owner owner)
    {
        Transform hand = GetHandContainer(owner);
        return hand != null ? hand.childCount : 0;
    }

    public void DrawCards(Owner owner, int count)
    {
        for (int i = 0; i < count; i++)
        {
            List<CardData> deck = GetDeck(owner);
            List<CardData> discard = GetDiscard(owner);

            if (deck.Count == 0)
            {
                if (discard.Count == 0) return;

                // reshuffle discard back into the deck
                deck.AddRange(discard);
                discard.Clear();
                Shuffle(deck);
            }

            CardData top = deck[0];
            deck.RemoveAt(0);

            Transform hand = GetHandContainer(owner);
            if (hand == null) return;

            CardView view = (cardPool != null) ? cardPool.Get(hand) : Instantiate(cardPrefab, hand);
            view.SetCard(top);
            view.SetOwner(owner);

            if (owner == Owner.Enemy)
            {
                view.SetFaceDown(true, enemyCardBackSprite);
            }
            else
            {
                view.SetFaceDown(false, null);
            }
        }
    }

    public void Discard(Owner owner, CardData card)
    {
        if (card == null) return;
        GetDiscard(owner).Add(card);
    }

    public void RemoveSpecificCardFromHandUI(Owner owner, CardData card)
    {
        Transform hand = GetHandContainer(owner);
        if (hand == null) return;

        for (int i = 0; i < hand.childCount; i++)
        {
            CardView cv = hand.GetChild(i).GetComponent<CardView>();
            if (cv != null && cv.data == card)
            {
                ReturnCard(cv);
                return;
            }
        }
    }

    void BuildAndShuffle(Owner owner)
    {
        List<CardData> deck = GetDeck(owner);
        List<CardData> discard = GetDiscard(owner);

        deck.Clear();
        discard.Clear();

        DeckData src = (owner == Owner.Player) ? playerDeckData : enemyDeckData;
        if (src == null || src.cards == null) return;

        deck.AddRange(src.cards);
        Shuffle(deck);
    }

    void ClearHandUI(Owner owner)
    {
        Transform hand = GetHandContainer(owner);
        if (hand == null) return;

        for (int i = hand.childCount - 1; i >= 0; i--)
        {
            CardView cv = hand.GetChild(i).GetComponent<CardView>();
            ReturnCard(cv);
        }
    }

    void ReturnCard(CardView cv)
    {
        if (cardPool != null)
            cardPool.Return(cv);
        else if (cv != null)
            Destroy(cv.gameObject);
    }

    static void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = UnityEngine.Random.Range(i, list.Count);
            CardData temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }

    Transform GetHandContainer(Owner owner)
    {
        return owner == Owner.Player ? playerHandContainer : enemyHandContainer;
    }

    List<CardData> GetDeck(Owner owner)
    {
        return owner == Owner.Player ? playerDeck : enemyDeck;
    }

    List<CardData> GetDiscard(Owner owner)
    {
        return owner == Owner.Player ? playerDiscard : enemyDiscard;
    }
}