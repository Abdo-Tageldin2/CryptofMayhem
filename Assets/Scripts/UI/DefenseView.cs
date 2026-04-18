using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenseView : MonoBehaviour
{
    public TMP_Text shieldValueText;
    public Image shieldIcon;

    private int shieldValue;
    public int ShieldValue { get { return shieldValue; } }

    public void SetShield(int value)
    {
        shieldValue = value;
        if (shieldValueText != null)
            shieldValueText.text = shieldValue.ToString();
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

        int absorbed = Mathf.Min(shieldValue, dmg);
        shieldValue -= absorbed;

        if (shieldValueText != null)
            shieldValueText.text = shieldValue.ToString();

        if (shieldValue <= 0)
            Destroy(gameObject);

        return dmg - absorbed;
    }
}