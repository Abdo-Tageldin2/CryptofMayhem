using TMPro;
using UnityEngine;
using UnityEngine.UI;

// owns all the HUD text and end-turn button — call Refresh() to update everything at once
public class BattleUI : MonoBehaviour
{
    public static BattleUI I;

    [Header("HUD")]
    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;
    public TMP_Text turnText;
    public TMP_Text roundText;

    [Header("Buttons")]
    public Button endTurnButton;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
    }

    public void Refresh(int playerHp, int enemyHp, bool playerTurn, int round, int roundLimit, bool canEndTurn)
    {
        if (playerHpText) playerHpText.text = "Player HP: " + playerHp;
        if (enemyHpText)  enemyHpText.text  = "Enemy HP: " + enemyHp;
        if (turnText)     turnText.text      = playerTurn ? "Your Turn" : "Enemy Turn";
        if (roundText)    roundText.text     = "Round " + round + " / " + roundLimit;
        if (endTurnButton) endTurnButton.interactable = canEndTurn;
    }

    public void DisableEndTurn()
    {
        if (endTurnButton) endTurnButton.interactable = false;
    }
}
