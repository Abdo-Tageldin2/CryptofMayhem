using TMPro;
using UnityEngine;

// reads player stats from PlayerPrefs (or Firebase if available) and shows the profile panel
public class ProfileDisplay : MonoBehaviour
{
    [Header("Panel")]
    public GameObject profilePanel;

    [Header("Text Fields")]
    public TMP_Text profileUsernameText;
    public TMP_Text winsText;
    public TMP_Text lossesText;

    void Start()
    {
        if (profilePanel != null)
            profilePanel.SetActive(false);
    }

    public void Show()
    {
        if (profilePanel == null) return;

        string name = PlayerPrefs.GetString("username", "Player");
        if (profileUsernameText != null)
            profileUsernameText.text = name;

        int wins   = PlayerPrefs.GetInt("wins",   0);
        int losses = PlayerPrefs.GetInt("losses", 0);

        // Firebase has the latest stats if the user is logged in
        if (FirebaseManager.I != null && FirebaseManager.I.IsReady)
        {
            wins   = FirebaseManager.I.Wins;
            losses = FirebaseManager.I.Losses;
        }

        if (winsText   != null) winsText.text   = wins.ToString();
        if (lossesText != null) lossesText.text = losses.ToString();

        profilePanel.SetActive(true);
    }

    public void Hide()
    {
        if (profilePanel != null)
            profilePanel.SetActive(false);
    }
}
