// builds the effect summary string shown on a card (e.g. "⚔ 2 Damage\n💚 1 Heal")
public static class CardEffectText
{
    public static string Build(CardData card)
    {
        if (card == null || card.effects == null) return "";

        string result = "";
        foreach (var e in card.effects)
        {
            if (e == null) continue;
            if (result.Length > 0) result += "\n";

            switch (e.type)
            {
                case EffectType.Damage:    result += "⚔ " + e.value + " Damage";  break;
                case EffectType.Heal:      result += "💚 " + e.value + " Heal";    break;
                case EffectType.Shield:    result += "🛡 " + e.value + " Shield";  break;
                case EffectType.Draw:      result += "🃏 Draw " + e.value;          break;
                case EffectType.PlayAgain: result += "🔄 Play Again";               break;
            }
        }
        return result;
    }
}
