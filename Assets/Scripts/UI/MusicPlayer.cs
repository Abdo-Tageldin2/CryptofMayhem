using UnityEngine;

// plays bg music and keeps it alive when we change scenes
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer I;

    void Awake()
    {
        // stop sound from starting again if we come back to the first scene
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject); 
    }
}