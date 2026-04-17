using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseView : MonoBehaviour
{
    public TMP_Text shieldValueText;
    public Image shieldIcon;

    public int ShieldValue { get; private set; }

    public void SetShield(int value)
    {
        ShieldValue = value;
        if (shieldValueText != null)
            shieldValueText.text = ShieldValue.ToString();
    }
    public void SetIconTint(Color c)
    {
        if (shieldIcon != null) shieldIcon.color = c;
    }

    public void SetTextColor(Color c)
    {
        if (shieldValueText != null) shieldValueText.color = c;
    }

    // returns leftover damage after shield absorbs
    public int AbsorbDamage(int dmg)
    {
        if (dmg <= 0) return 0;

        int absorbed = Mathf.Min(ShieldValue, dmg);
        ShieldValue -= absorbed;

        if (shieldValueText != null)
            shieldValueText.text = ShieldValue.ToString();

        if (ShieldValue <= 0)
            Destroy(gameObject);

        return dmg - absorbed;
    }
}