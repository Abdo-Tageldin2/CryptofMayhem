using TMPro;
using UnityEngine;

// shows the end-of-game panel and records the result
public class GameOverHandler : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_Text resultText;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Show(bool playerWon)
    {
        if (PlayedCardFXController.I != null) PlayedCardFXController.I.ClearPlayedCards();
        if (resultText) resultText.text = playerWon ? "You Win!" : "You Lose!";
        if (panel) panel.SetActive(true);

        if (playerWon) GameResultRecorder.RecordWin();
        else           GameResultRecorder.RecordLoss();
    }

    public void ShowTimeUp(int playerHp, int enemyHp)
    {
        if (PlayedCardFXController.I != null) PlayedCardFXController.I.ClearPlayedCards();

        bool draw      = playerHp == enemyHp;
        bool playerWon = playerHp > enemyHp;

        if (resultText)
        {
            if (draw)           resultText.text = "Draw! Same HP!";
            else if (playerWon) resultText.text = "Time's Up! You Win!";
            else                resultText.text = "Time's Up! You Lose!";
        }

        if (panel) panel.SetActive(true);

        if (draw)
        {
            PlayerPrefs.SetInt("draws", PlayerPrefs.GetInt("draws", 0) + 1);
            PlayerPrefs.Save();
        }
        else if (playerWon) GameResultRecorder.RecordWin();
        else                GameResultRecorder.RecordLoss();
    }
}
