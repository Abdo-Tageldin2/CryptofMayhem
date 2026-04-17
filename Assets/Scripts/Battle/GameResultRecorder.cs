using System;
using UnityEngine;

// saves win/loss/draw to PlayerPrefs, then fires events so listeners (like Firebase) can react
public static class GameResultRecorder
{
    public static event Action OnWinRecorded;
    public static event Action OnLossRecorded;
    public static event Action OnDrawRecorded;

    public static void RecordWin()
    {
        PlayerPrefs.SetInt(PrefKeys.Wins, PlayerPrefs.GetInt(PrefKeys.Wins, 0) + 1);
        PlayerPrefs.Save();
        OnWinRecorded?.Invoke();
    }

    public static void RecordLoss()
    {
        PlayerPrefs.SetInt(PrefKeys.Losses, PlayerPrefs.GetInt(PrefKeys.Losses, 0) + 1);
        PlayerPrefs.Save();
        OnLossRecorded?.Invoke();
    }

    public static void RecordDraw()
    {
        PlayerPrefs.SetInt(PrefKeys.Draws, PlayerPrefs.GetInt(PrefKeys.Draws, 0) + 1);
        PlayerPrefs.Save();
        OnDrawRecorded?.Invoke();
    }
}
