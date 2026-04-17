using UnityEngine;

public enum EffectType
{
    Damage,
    Heal,
    Shield,
    Draw,
    PlayAgain
}

[System.Serializable]
public class CardEffect
{
    public EffectType type;
    public int value;
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;
    public Sprite artwork;

    [Header("Effects")]
    public CardEffect[] effects;
}