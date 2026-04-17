using System.Text;

// builds the effect summary string shown on a card (e.g. "⚔ 2 Damage\n💚 1 Heal")
public static class CardEffectText
{
    public static string Build(CardData card)
    {
        if (card == null || card.effects == null) return "";

        var sb = new StringBuilder();
        foreach (var e in card.effects)
        {
            if (e == null) continue;
            if (sb.Length > 0) sb.Append("\n");

            switch (e.type)
            {
                case EffectType.Damage:    sb.Append($"⚔ {e.value} Damage");  break;
                case EffectType.Heal:      sb.Append($"💚 {e.value} Heal");    break;
                case EffectType.Shield:    sb.Append($"🛡 {e.value} Shield");  break;
                case EffectType.Draw:      sb.Append($"🃏 Draw {e.value}");    break;
                case EffectType.PlayAgain: sb.Append("🔄 Play Again");         break;
            }
        }
        return sb.ToString();
    }
}
