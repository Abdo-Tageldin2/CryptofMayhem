using UnityEngine;

// everything to do with shields lives here — adding them, absorbing damage, reading values
public class ShieldSystem : MonoBehaviour
{
    [Header("Defense Areas")]
    public Transform playerDefenseArea;
    public Transform enemyDefenseArea;
    public DefenseView defensePrefab;

    [Header("Enemy Shield Colors")]
    [SerializeField] private Color enemyShieldTint      = Color.red;
    [SerializeField] private Color enemyShieldTextColor  = Color.white;

    public void AddPlayerShield(int amount)
    {
        var dv = GetOrCreate(playerDefenseArea);
        if (dv != null) dv.SetShield(dv.ShieldValue + amount);
    }

    public void AddEnemyShield(int amount)
    {
        var dv = GetOrCreate(enemyDefenseArea);
        if (dv != null)
        {
            dv.SetShield(dv.ShieldValue + amount);
            dv.SetIconTint(enemyShieldTint);
            dv.SetTextColor(enemyShieldTextColor);
        }
    }

    // player attacks enemy — enemy shield absorbs first
    public int AbsorbEnemyDamage(int dmg)
    {
        return Absorb(enemyDefenseArea, dmg);
    }

    // enemy attacks player — player shield absorbs first
    public int AbsorbPlayerDamage(int dmg)
    {
        return Absorb(playerDefenseArea, dmg);
    }

    public int GetEnemyShieldValue()
    {
        return GetShieldValue(enemyDefenseArea);
    }

    DefenseView GetOrCreate(Transform area)
    {
        if (area == null || defensePrefab == null) return null;

        if (area.childCount > 0)
            return area.GetChild(0).GetComponent<DefenseView>();

        var dv = Instantiate(defensePrefab, area);
        dv.SetShield(0);
        return dv;
    }

    int Absorb(Transform area, int dmg)
    {
        if (dmg <= 0) return 0;
        if (area == null || area.childCount == 0) return dmg;

        var dv = area.GetChild(0).GetComponent<DefenseView>();
        return dv != null ? dv.AbsorbDamage(dmg) : dmg;
    }

    int GetShieldValue(Transform area)
    {
        if (area == null || area.childCount == 0) return 0;
        var dv = area.GetChild(0).GetComponent<DefenseView>();
        return dv != null ? dv.ShieldValue : 0;
    }
}
