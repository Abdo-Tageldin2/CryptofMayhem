using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

// handles Firebase Auth and Firestore — persists across scenes
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager I;

    [SerializeField] private float initTimeout = 6f;

    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private string userId;

    private Queue<Action> mainThreadQueue = new Queue<Action>();

    private bool isReady;
    private bool hasFailed;
    private int wins;
    private int losses;

    public bool IsReady   { get { return isReady; } }
    public bool HasFailed { get { return hasFailed; } }
    public int  Wins      { get { return wins; } }
    public int  Losses    { get { return losses; } }

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GameResultRecorder.OnWinRecorded  += OnWin;
        GameResultRecorder.OnLossRecorded += OnLoss;

        // on some devices CheckAndFixDependenciesAsync just hangs, so bail out after 6s
        Invoke("FirebaseTimeout", initTimeout);

        try
        {
            Debug.Log("[Firebase] Checking dependencies...");
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                Debug.Log("[Firebase] Dependency check result: " + task.Result);

                if (task.Result != DependencyStatus.Available)
                {
                    Debug.LogError("[Firebase] Not ready: " + task.Result);
                    RunOnMainThread(() => hasFailed = true);
                    return;
                }

                auth    = FirebaseAuth.DefaultInstance;
                db      = FirebaseFirestore.DefaultInstance;
                RunOnMainThread(() =>
                {
                    CancelInvoke("FirebaseTimeout");
                    isReady = true;

                    // fetch remote config now that Firebase is up
                    if (RemoteConfigManager.I != null)
                        RemoteConfigManager.I.Fetch();
                });
                Debug.Log("[Firebase] Ready!");
            });
        }
        catch (System.Exception e)
        {
            // Firebase native DLL failed to load (e.g. macOS code signing)
            Debug.LogError("[Firebase] Failed to start: " + e.Message);
            hasFailed = true;
        }
    }

    void OnDestroy()
    {
        GameResultRecorder.OnWinRecorded  -= OnWin;
        GameResultRecorder.OnLossRecorded -= OnLoss;
    }

    void FirebaseTimeout()
    {
        if (!IsReady && !HasFailed)
        {
            Debug.LogWarning("[Firebase] Timed out waiting for initialization — marking as failed.");
            hasFailed = true;
        }
    }

    void Update()
    {
        List<Action> pending = new List<Action>();

        lock (mainThreadQueue)
        {
            while (mainThreadQueue.Count > 0)
                pending.Add(mainThreadQueue.Dequeue());
        }

        foreach (var action in pending)
            action.Invoke();
    }

    public void SignUp(string email, string password, string username, Action<bool, string> onDone)
    {
        Debug.Log("[Firebase] SignUp called");
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                string error = "Something went wrong. Please try again.";
                Debug.LogError("[Firebase] SignUp failed: " + task.Exception);
                RunOnMainThread(() => onDone(false, error));
                return;
            }

            Debug.Log("[Firebase] SignUp success, saving stats...");
            userId  = auth.CurrentUser.UserId;

            SaveStats(username, () =>
            {
                RunOnMainThread(() =>
                {
                    PlayerPrefs.SetString("username", username);
                    PlayerPrefs.Save();
                    Debug.Log("[Firebase] SignUp fully done");
                    onDone(true, "");
                });
            });
        });
    }

    public void LogIn(string email, string password, Action<bool, string> onDone)
    {
        Debug.Log("[Firebase] LogIn called");
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                string error = "Something went wrong. Please try again.";
                Debug.LogError("[Firebase] LogIn failed: " + task.Exception);
                RunOnMainThread(() => onDone(false, error));
                return;
            }

            Debug.Log("[Firebase] LogIn success, loading stats...");
            userId  = auth.CurrentUser.UserId;

            LoadStats(() => RunOnMainThread(() =>
            {
                Debug.Log("[Firebase] LogIn fully done");
                onDone(true, "");
            }));
        });
    }

    public void SignOut()
    {
        if (auth != null)
            auth.SignOut();

        userId = null;
        wins   = 0;
        losses = 0;
        Debug.Log("[Firebase] Signed out.");
    }

    void OnWin()
    {
        wins++;
        SaveStats(PlayerPrefs.GetString("username", "Player"), null);
    }

    void OnLoss()
    {
        losses++;
        SaveStats(PlayerPrefs.GetString("username", "Player"), null);
    }

    void LoadStats(Action onDone)
    {
        db.Collection("users").Document(userId).GetSnapshotAsync().ContinueWith(task =>
        {
            if (!task.IsFaulted && task.Result.Exists)
            {
                DocumentSnapshot snap = task.Result;
                wins   = snap.ContainsField("wins")   ? snap.GetValue<int>("wins")   : 0;
                losses = snap.ContainsField("losses") ? snap.GetValue<int>("losses") : 0;

                if (snap.ContainsField("username"))
                {
                    string name = snap.GetValue<string>("username");
                    // PlayerPrefs must be set on main thread
                    RunOnMainThread(() =>
                    {
                        PlayerPrefs.SetString("username", name);
                        PlayerPrefs.Save();
                    });
                }
            }

            if (onDone != null) onDone.Invoke();
        });
    }

    void SaveStats(string username, Action onDone)
    {
        if (userId == null) { if (onDone != null) onDone.Invoke(); return; }

        var data = new Dictionary<string, object>
        {
            { "username", username },
            { "wins",     Wins     },
            { "losses",   Losses   }
        };

        db.Collection("users").Document(userId).SetAsync(data).ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.LogError("Save failed: " + task.Exception);

            if (onDone != null) onDone.Invoke();
        });
    }

    void RunOnMainThread(Action action)
    {
        lock (mainThreadQueue) { mainThreadQueue.Enqueue(action); }
    }
}
