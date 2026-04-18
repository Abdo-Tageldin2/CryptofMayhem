using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// controls the login, signup, and guest buttons on the auth screen
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

    [Header("Background Swap")]
    public Image backgroundImage;
    public Sprite loginBackground;
    public Sprite signupBackground;
    public Sprite startBackground;


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
        PlayerPrefs.DeleteKey("username");
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
        ApplyPlaceholders();
    }

    public void OnLogInPressed()
    {
        mainPanel.SetActive(false);
        if (signupPanel != null) signupPanel.SetActive(false);
        if (loginPanel  != null) loginPanel.SetActive(true);
        SetBackground(loginBackground);
        ClearLogin();
        ApplyPlaceholders();
    }

    public void OnGuestPressed()
    {
        PlayerPrefs.SetString("username", "Guest");
        PlayerPrefs.Save();
        GoToHome();
    }

    void OnLoginSubmit()
    {
        if (!FirebaseReady(loginErrorText)) return;

        string email    = loginEmailField != null ? loginEmailField.text.Trim() : "";
        string password = loginPasswordField != null ? loginPasswordField.text : "";

        if (email == "")    { ShowError(loginErrorText, "Please enter your email.");          return; }
        if (password.Length < 6)            { ShowError(loginErrorText, "Password must be at least 6 characters."); return; }

        loginErrorText.text = "";
        FirebaseManager.I.LogIn(email, password, OnAuthDone);
    }

    void OnSignupSubmit()
    {
        if (!FirebaseReady(signupErrorText)) return;

        string username = signupUsernameField != null ? signupUsernameField.text.Trim() : "";
        string email    = signupEmailField    != null ? signupEmailField.text.Trim()    : "";
        string password = signupPasswordField != null ? signupPasswordField.text        : "";

        if (username == "") { ShowError(signupErrorText, "Please enter a username."); return; }
        if (email == "")    { ShowError(signupErrorText, "Please enter your email."); return; }
        if (password.Length < 6)            { ShowError(signupErrorText, "Password must be at least 6 characters."); return; }

        signupErrorText.text = "";
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

        if (startBackground != null)
            SetBackground(startBackground);
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

    void GoToHome()
    {
        SceneManager.LoadScene("home");
    }

    void SetBackground(Sprite sprite)
    {
        if (backgroundImage == null || sprite == null) return;
        backgroundImage.sprite = sprite;
    }
    // applies remote config placeholder text to all input fields
    void ApplyPlaceholders()
    {
        if (RemoteConfigManager.I == null) return;
        SetPlaceholder(signupUsernameField, RemoteConfigManager.I.UsernamePlaceholder);
        SetPlaceholder(signupEmailField,    RemoteConfigManager.I.EmailPlaceholder);
        SetPlaceholder(signupPasswordField, RemoteConfigManager.I.PasswordPlaceholder);
        SetPlaceholder(loginEmailField,     RemoteConfigManager.I.EmailPlaceholder);
        SetPlaceholder(loginPasswordField,  RemoteConfigManager.I.PasswordPlaceholder);
    }

    void SetPlaceholder(TMP_InputField field, string text)
    {
        if (field == null) return;
        if (field.placeholder == null) return;
        TMP_Text ph = field.placeholder.GetComponent<TMP_Text>();
        if (ph != null) ph.text = text;
    }
}
