using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AuthManager : MonoBehaviour
{
    [Header("Main Buttons Panel")]
    public GameObject mainPanel;
    public Button signUpButton;
    public Button logInButton;
    public Button guestButton;

    [Header("Login Panel")]
    public GameObject loginPanel;
    public TMP_InputField loginEmailField;
    public TMP_InputField loginPasswordField;
    public TMP_Text loginErrorText;
    public Button loginSubmitButton;
    public Button loginBackButton;

    [Header("Signup Panel")]
    public GameObject signupPanel;
    public TMP_InputField signupUsernameField;
    public TMP_InputField signupEmailField;
    public TMP_InputField signupPasswordField;
    public TMP_Text signupErrorText;
    public Button signupSubmitButton;
    public Button signupBackButton;

    [Header("Loading")]
    public GameObject loadingOverlay;

    [Header("Background Swap")]
    public Image backgroundImage;
    public Sprite loginBackground;
    public Sprite signupBackground;
    public Sprite startBackground;
    public bool startOnHomeBackground = true;

    void Start()
    {
        signUpButton.onClick.AddListener(OnSignUpPressed);
        logInButton.onClick.AddListener(OnLogInPressed);
        guestButton.onClick.AddListener(OnGuestPressed);

        if (loginSubmitButton  != null) loginSubmitButton.onClick.AddListener(OnLoginSubmit);
        if (loginBackButton    != null) loginBackButton.onClick.AddListener(ShowMainPanel);
        if (signupSubmitButton != null) signupSubmitButton.onClick.AddListener(OnSignupSubmit);
        if (signupBackButton   != null) signupBackButton.onClick.AddListener(ShowMainPanel);

        if (FirebaseManager.I != null) FirebaseManager.I.SignOut();
        PlayerPrefs.DeleteKey(PrefKeys.Username);
        PlayerPrefs.Save();

        ShowMainPanel();
    }

    public void OnSignUpPressed()
    {
        mainPanel.SetActive(false);
        if (loginPanel  != null) loginPanel.SetActive(false);
        if (signupPanel != null) signupPanel.SetActive(true);
        SetBackground(signupBackground);
        ClearSignup();
    }

    public void OnLogInPressed()
    {
        mainPanel.SetActive(false);
        if (signupPanel != null) signupPanel.SetActive(false);
        if (loginPanel  != null) loginPanel.SetActive(true);
        SetBackground(loginBackground);
        ClearLogin();
    }

    public void OnGuestPressed()
    {
        PlayerPrefs.SetString(PrefKeys.Username, "Guest");
        PlayerPrefs.Save();
        GoToHome();
    }

    void OnLoginSubmit()
    {
        if (!FirebaseReady(loginErrorText)) return;

        string email    = loginEmailField != null ? loginEmailField.text.Trim() : "";
        string password = loginPasswordField != null ? loginPasswordField.text : "";

        if (string.IsNullOrEmpty(email))    { ShowError(loginErrorText, "Please enter your email.");          return; }
        if (password.Length < 6)            { ShowError(loginErrorText, "Password must be at least 6 characters."); return; }

        loginErrorText.text = "";
        SetLoading(true);
        FirebaseManager.I.LogIn(email, password, OnAuthDone);
    }

    void OnSignupSubmit()
    {
        if (!FirebaseReady(signupErrorText)) return;

        string username = signupUsernameField != null ? signupUsernameField.text.Trim() : "";
        string email    = signupEmailField    != null ? signupEmailField.text.Trim()    : "";
        string password = signupPasswordField != null ? signupPasswordField.text        : "";

        if (string.IsNullOrEmpty(username)) { ShowError(signupErrorText, "Please enter a username."); return; }
        if (string.IsNullOrEmpty(email))    { ShowError(signupErrorText, "Please enter your email."); return; }
        if (password.Length < 6)            { ShowError(signupErrorText, "Password must be at least 6 characters."); return; }

        signupErrorText.text = "";
        SetLoading(true);
        FirebaseManager.I.SignUp(email, password, username, OnAuthDone);
    }

    bool FirebaseReady(TMP_Text errorOut)
    {
        if (FirebaseManager.I == null || FirebaseManager.I.HasFailed)
        {
            ShowError(errorOut, "Login unavailable on this device. Use Guest instead.");
            return false;
        }
        if (!FirebaseManager.I.IsReady)
        {
            ShowError(errorOut, "Still connecting... try again in a moment.");
            return false;
        }
        return true;
    }

    void OnAuthDone(bool success, string error)
    {
        SetLoading(false);
        if (success) GoToHome();
        else
        {
            if (loginPanel  != null && loginPanel.activeSelf)  ShowError(loginErrorText,  error);
            if (signupPanel != null && signupPanel.activeSelf) ShowError(signupErrorText, error);
        }
    }

    void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        if (loginPanel  != null) loginPanel.SetActive(false);
        if (signupPanel != null) signupPanel.SetActive(false);
        if (loginErrorText  != null) loginErrorText.text  = "";
        if (signupErrorText != null) signupErrorText.text = "";

        if (startOnHomeBackground && startBackground != null)
            SetBackground(startBackground);
        else
            SetBackground(loginBackground);
    }

    void ClearLogin()
    {
        if (loginEmailField    != null) loginEmailField.text    = "";
        if (loginPasswordField != null) loginPasswordField.text = "";
        if (loginErrorText     != null) loginErrorText.text     = "";
    }

    void ClearSignup()
    {
        if (signupUsernameField != null) signupUsernameField.text = "";
        if (signupEmailField    != null) signupEmailField.text    = "";
        if (signupPasswordField != null) signupPasswordField.text = "";
        if (signupErrorText     != null) signupErrorText.text     = "";
    }

    void ShowError(TMP_Text target, string message)
    {
        if (target != null) target.text = message;
    }

    void SetLoading(bool active)
    {
        if (loadingOverlay != null) loadingOverlay.SetActive(active);
        if (loginSubmitButton  != null) loginSubmitButton.interactable  = !active;
        if (signupSubmitButton != null) signupSubmitButton.interactable = !active;
    }

    void GoToHome() => SceneManager.LoadScene(SceneNames.Home);

    void SetBackground(Sprite sprite)
    {
        if (backgroundImage == null || sprite == null) return;
        backgroundImage.sprite = sprite;
    }
}
