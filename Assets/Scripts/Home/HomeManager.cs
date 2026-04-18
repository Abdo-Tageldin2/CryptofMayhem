using UnityEngine;
using UnityEngine.SceneManagement;

// handles home screen navigation — buttons, panels
public class HomeManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject howToPlayPanel;
    public GameObject difficultyPanel;

    private ProfileDisplay profile;

    void Start()
    {
        profile = GetComponent<ProfileDisplay>();

        if (howToPlayPanel  != null) howToPlayPanel.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);
    }

    public void OnPlayPressed()
    {
        if (difficultyPanel != null)
            difficultyPanel.SetActive(true);
        else
            SceneManager.LoadScene("game");
    }

    public void OnEasyPressed()   { StartGame("easy"); }
    public void OnNormalPressed() { StartGame("normal"); }
    public void OnHardPressed()   { StartGame("hard"); }

    void StartGame(string difficulty)
    {
        PlayerPrefs.SetString("difficulty", difficulty);
        PlayerPrefs.Save();
        SceneManager.LoadScene("game");
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
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.Save();

        if (FirebaseManager.I != null)
            FirebaseManager.I.SignOut();

        SceneManager.LoadScene("auth");
    }
}
