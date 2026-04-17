using UnityEngine;

// plays bg music and keeps it alive when we change scenes
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer I;  // singleton (lec 3)

    [SerializeField] AudioSource clickSfx;  // card click sound

    void Awake()
    {
        // stop duplicates if we come back to the first scene
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);  // survive scene changes (lec 7)
    }

    // called from other scripts to play the card click sound
    public void PlayClickSfx()
    {
        if (clickSfx != null) clickSfx.Play();
    }
}