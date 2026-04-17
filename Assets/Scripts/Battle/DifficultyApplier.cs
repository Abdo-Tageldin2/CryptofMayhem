using UnityEngine;

public enum Difficulty { Easy, Normal, Hard }

// reads the saved difficulty and applies the right settings to the enemy AI
public static class DifficultyApplier
{
    const int EasyHealThreshold   = 3, EasyMaxPlays   = 2, EasyRounds   = 12;
    const int NormalHealThreshold = 5, NormalMaxPlays  = 4, NormalRounds = 10;
    const int HardHealThreshold   = 7, HardMaxPlays   = 5, HardRounds   = 8;

    public static Difficulty Apply(EnemyAI ai, ref int roundLimit)
    {
        string saved = PlayerPrefs.GetString(PrefKeys.Difficulty, "normal");

        switch (saved)
        {
            case "easy":
                ai.healThreshold = EasyHealThreshold; ai.maxPlaysPerTurn = EasyMaxPlays; roundLimit = EasyRounds;
                return Difficulty.Easy;
            case "hard":
                ai.healThreshold = HardHealThreshold; ai.maxPlaysPerTurn = HardMaxPlays; roundLimit = HardRounds;
                return Difficulty.Hard;
            default:
                ai.healThreshold = NormalHealThreshold; ai.maxPlaysPerTurn = NormalMaxPlays; roundLimit = NormalRounds;
                return Difficulty.Normal;
        }
    }
}
