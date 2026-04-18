using System;
using UnityEngine;

// saves win/loss to PlayerPrefs, then fires events so listeners (like Firebase) can react
public static class GameResultRecorder
{
    public static event Action OnWinRecorded;
    public static event Action OnLossRecorded;

    public static void RecordWin()
    {
        PlayerPrefs.SetInt("wins", PlayerPrefs.GetInt("wins", 0) + 1);
        PlayerPrefs.Save();
        if (OnWinRecorded != null) OnWinRecorded.Invoke();
    }

    public static void RecordLoss()
    {
        PlayerPrefs.SetInt("losses", PlayerPrefs.GetInt("losses", 0) + 1);
        PlayerPrefs.Save();
        if (OnLossRecorded != null) OnLossRecorded.Invoke();
    }
}
