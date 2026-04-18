using UnityEngine;

// reads the saved difficulty and applies the right settings to the enemy AI
public static class DifficultyApplier
{
    const int EasyHealThreshold   = 3, EasyMaxPlays   = 2, EasyRounds   = 12;
    const int NormalHealThreshold = 5, NormalMaxPlays  = 4, NormalRounds = 10;
    const int HardHealThreshold   = 7, HardMaxPlays   = 5, HardRounds   = 8;

    public static void Apply(EnemyAI ai, BattleManager bm)
    {
        string saved = PlayerPrefs.GetString("difficulty", "normal");

        switch (saved)
        {
            case "easy":
                ai.healThreshold = EasyHealThreshold; ai.maxPlaysPerTurn = EasyMaxPlays; bm.roundLimit = EasyRounds;
                break;
            case "hard":
                ai.healThreshold = HardHealThreshold; ai.maxPlaysPerTurn = HardMaxPlays; bm.roundLimit = HardRounds;
                break;
            default:
                ai.healThreshold = NormalHealThreshold; ai.maxPlaysPerTurn = NormalMaxPlays; bm.roundLimit = NormalRounds;
                break;
        }
    }
}
