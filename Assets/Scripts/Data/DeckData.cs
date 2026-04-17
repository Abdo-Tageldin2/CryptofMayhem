using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Card Game/Deck Data")]
public class DeckData : ScriptableObject
{
    public CardData[] cards;
}