using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;

// fetches placeholder text from Firebase Remote Config
public class RemoteConfigManager : MonoBehaviour
{
    public static RemoteConfigManager I;

    private string usernamePlaceholder = "Username";
    private string emailPlaceholder    = "Email";
    private string passwordPlaceholder = "Password";

    public string UsernamePlaceholder { get { return usernamePlaceholder; } }
    public string EmailPlaceholder    { get { return emailPlaceholder; } }
    public string PasswordPlaceholder { get { return passwordPlaceholder; } }

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // called by FirebaseManager once Firebase is up
    public void Fetch()
    {
        FirebaseRemoteConfig config = FirebaseRemoteConfig.DefaultInstance;

        // fallback values in case the server is unreachable
        var defaults = new Dictionary<string, object>
        {
            { "username_placeholder", "Username" },
            { "email_placeholder",    "Email" },
            { "password_placeholder", "Password" }
        };

        config.SetDefaultsAsync(defaults).ContinueWith(setTask =>
        {
            if (setTask.IsFaulted) return;

            config.FetchAsync(TimeSpan.FromHours(1)).ContinueWith(fetchTask =>
            {
                if (fetchTask.IsFaulted) return;

                config.ActivateAsync().ContinueWith(activateTask =>
                {
                    usernamePlaceholder = config.GetValue("username_placeholder").StringValue;
                    emailPlaceholder    = config.GetValue("email_placeholder").StringValue;
                    passwordPlaceholder = config.GetValue("password_placeholder").StringValue;
                    Debug.Log("[RemoteConfig] Placeholders fetched!");
                });
            });
        });
    }
}
