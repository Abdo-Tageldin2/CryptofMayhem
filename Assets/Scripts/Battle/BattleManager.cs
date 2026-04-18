using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// core game loop — tracks HP, manages turns, resolves card effects
public class BattleManager : MonoBehaviour
{
    public static BattleManager I;

    [Header("Managers")]
    public DeckManager deckManager;

    [Header("Buttons")]
    public Button quitButton;

    [Header("HP")]
    [Range(1, 30)] public int playerHp    = 10;
    [Range(1, 30)] public int enemyHp     = 10;
    [Range(1, 30)] public int playerMaxHp = 10;
    [Range(1, 30)] public int enemyMaxHp  = 10;

    [Header("Round Limit")]
    [Range(1, 30)] public int roundLimit = 10;

    [Header("Enemy Cards Stay Visible (seconds)")]
    [Range(0f, 10f)] public float enemyHoldSeconds = 2.5f;

    [Header("Timing")]
    [SerializeField, Range(0f, 2f)] private float turnTransitionDelay = 0.35f;

    [Header("Draw Settings")]
    public int startingDraw  = 2;
    public int drawPerTurn   = 1;
    public int emptyHandDraw = 2;

    [Header("Actions")]
    public int actionsPerTurn = 1;

    private bool playerTurn = true;

    private int currentRound = 0;
    private int actionsRemaining;
    private bool playedAtLeastOneCard = false;

    private ShieldSystem shields;
    private EnemyAI enemyAI;
    private CardEffectResolver resolver;
    private GameOverHandler gameOver;
    private BattleUI ui;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        shields  = GetComponent<ShieldSystem>();
        enemyAI  = GetComponent<EnemyAI>();
        resolver = GetComponent<CardEffectResolver>();
        gameOver = GetComponent<GameOverHandler>();
        ui       = GetComponent<BattleUI>();

        if (deckManager == null)
            deckManager = GetComponent<DeckManager>();

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitPressed);

        ApplyDifficulty();

        deckManager.Setup();
        deckManager.DrawCards(Owner.Player, startingDraw);
        deckManager.DrawCards(Owner.Enemy, startingDraw);

        StartPlayerTurn();
    }

    void ApplyDifficulty()
    {
        DifficultyApplier.Apply(enemyAI, this);
    }
    // is the player allowed to play a card right now?
    public bool CanPlayerAct
    {
        get { return playerTurn && actionsRemaining > 0; }
    }

    public void RefreshUI()
    {
        bool handEmpty = deckManager.HandCount(Owner.Player) == 0;
        bool canEnd    = playerTurn && playedAtLeastOneCard && (actionsRemaining <= 0 || handEmpty);
        if (ui != null) ui.Refresh(playerHp, enemyHp, playerTurn, currentRound, roundLimit, canEnd);
    }

    void StartPlayerTurn()
    {
        playerTurn           = true;
        actionsRemaining     = actionsPerTurn;
        playedAtLeastOneCard = false;

        if (PlayedCardFXController.I != null)
            PlayedCardFXController.I.ClearPlayedCards();

        deckManager.DrawCards(Owner.Player, drawPerTurn);
        if (deckManager.HandCount(Owner.Player) == 0)
            deckManager.DrawCards(Owner.Player, emptyHandDraw);

        RefreshUI();
    }

    public void OnEndTurnPressed()
    {
        if (!playerTurn) return;

        if (PlayedCardFXController.I != null)
            PlayedCardFXController.I.ClearPlayedCards();

        playerTurn = false;
        RefreshUI();
        Invoke("TriggerEnemyTurn", turnTransitionDelay);
    }

    void TriggerEnemyTurn()
    {
        enemyAI.StartTurn();
    }

    public void OnPlayerCardClicked(CardView view)
    {
        if (view == null || view.data == null) return;

        bool played = PlayCard(view.data);
        if (!played) return;

        if (PlayedCardFXController.I != null && view.cardArtImage != null)
        {
            Vector3 pos = view.cardArtImage.rectTransform.position;
            PlayedCardFXController.I.AddPlayedCardAnimatedFromWorldPos(pos, view.data.artwork);
        }

        deckManager.RemoveSpecificCardFromHandUI(Owner.Player, view.data);
        RefreshUI();
    }

    private bool PlayCard(CardData card)
    {
        if (!playerTurn || card == null || actionsRemaining <= 0) return false;

        playedAtLeastOneCard = true;

        actionsRemaining += resolver.ApplyPlayerCard(card);
        actionsRemaining--;
        deckManager.Discard(Owner.Player, card);

        RefreshUI();

        if (enemyHp  <= 0) EndGame(true);
        else if (playerHp <= 0) EndGame(false);

        return true;
    }

    public void EndEnemyTurn()
    {
        currentRound++;
        RefreshUI();

        if (playerHp <= 0) { EndGame(false); return; }
        if (enemyHp  <= 0) { EndGame(true);  return; }

        if (currentRound >= roundLimit)
        {
            if (ui != null) ui.DisableEndTurn();
            gameOver.ShowTimeUp(playerHp, enemyHp);
            return;
        }

        Invoke("StartPlayerTurn", enemyHoldSeconds + turnTransitionDelay);
    }

    void EndGame(bool playerWon)
    {
        if (ui != null) ui.DisableEndTurn();
        gameOver.Show(playerWon);
    }

    public void OnRestartPressed()
    {
        SceneManager.LoadScene("home");
    }

    public void OnQuitPressed()
    {
        SceneManager.LoadScene("home");
    }
}
