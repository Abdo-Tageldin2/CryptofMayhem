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
    public TMP_Text drawsText;

    void Start()
    {
        if (profilePanel != null)
            profilePanel.SetActive(false);
    }

    public void Show()
    {
        if (profilePanel == null) return;

        string name = PlayerPrefs.GetString(PrefKeys.Username, "Player");
        if (profileUsernameText != null)
            profileUsernameText.text = name;

        int wins   = PlayerPrefs.GetInt(PrefKeys.Wins,   0);
        int losses = PlayerPrefs.GetInt(PrefKeys.Losses, 0);
        int draws  = PlayerPrefs.GetInt(PrefKeys.Draws,  0);

        // Firebase has synced stats if the user is logged in
        if (FirebaseManager.I != null && FirebaseManager.I.IsReady)
        {
            wins   = FirebaseManager.I.Wins;
            losses = FirebaseManager.I.Losses;
        }

        if (winsText   != null) winsText.text   = wins.ToString();
        if (lossesText != null) lossesText.text = losses.ToString();
        if (drawsText  != null) drawsText.text  = draws.ToString();

        profilePanel.SetActive(true);
    }

    public void Hide()
    {
        if (profilePanel != null)
            profilePanel.SetActive(false);
    }
}
