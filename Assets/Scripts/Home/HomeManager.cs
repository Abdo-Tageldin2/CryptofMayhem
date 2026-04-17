using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// handles home screen navigation — buttons, panels, and the welcome message
public class HomeManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text usernameText;
    public GameObject howToPlayPanel;
    public GameObject difficultyPanel;

    [Header("Timing")]
    [SerializeField] private float howToPlayDelay = 0.5f;

    ProfileDisplay profile;

    void Start()
    {
        profile = GetComponent<ProfileDisplay>();

        string name = PlayerPrefs.GetString(PrefKeys.Username, "Player");
        if (usernameText != null)
            usernameText.text = "Welcome, " + name + "!";

        if (howToPlayPanel   != null) howToPlayPanel.SetActive(false);
        if (difficultyPanel  != null) difficultyPanel.SetActive(false);

        bool firstTime = PlayerPrefs.GetInt(PrefKeys.HasPlayedBefore, 0) == 0;
        if (firstTime)
        {
            PlayerPrefs.SetInt(PrefKeys.HasPlayedBefore, 1);
            PlayerPrefs.Save();
            Invoke(nameof(OpenHowToPlayDelayed), howToPlayDelay);
        }
    }

    void OpenHowToPlayDelayed()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    public void OnPlayPressed()
    {
        if (difficultyPanel != null)
            difficultyPanel.SetActive(true);
        else
            SceneManager.LoadScene(SceneNames.Game);
    }

    public void OnEasyPressed()   => StartGame("easy");
    public void OnNormalPressed() => StartGame("normal");
    public void OnHardPressed()   => StartGame("hard");

    void StartGame(string difficulty)
    {
        PlayerPrefs.SetString(PrefKeys.Difficulty, difficulty);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneNames.Game);
    }

    public void OnDifficultyBackPressed()
    {
        if (difficultyPanel != null)
            difficultyPanel.SetActive(false);
    }

    public void OnHowToPlayPressed()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    public void OnHowToPlayClosePressed()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }

    public void OnProfilePressed()
    {
        if (profile != null) profile.Show();
    }

    public void OnProfileClosePressed()
    {
        if (profile != null) profile.Hide();
    }

    public void OnLogoutPressed()
    {
        PlayerPrefs.DeleteKey(PrefKeys.Username);
        PlayerPrefs.Save();

        if (FirebaseManager.I != null)
            FirebaseManager.I.SignOut();

        SceneManager.LoadScene(SceneNames.Auth);
    }
}
