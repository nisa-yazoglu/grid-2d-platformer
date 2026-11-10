using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource audioSource;
    private const string MusicPath = "Musics/BackgroundMusic";

    public static AudioManager Instance
    {
        get { return instance; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (instance == null)
        {
            GameObject audioManagerObj = new GameObject("AudioManager");
            instance = audioManagerObj.AddComponent<AudioManager>();
            DontDestroyOnLoad(audioManagerObj); // Sahne geçişlerinde yok olmasın
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        SetupAudioSource();
        PlayBackgroundMusic();
    }

    private void SetupAudioSource()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    public void PlayBackgroundMusic()
    {
        AudioClip musicClip = Resources.Load<AudioClip>(MusicPath);

        if (musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.Play();
            Debug.Log("AudioManager: Background Music Loaded and Playing from Resources.");
        }
        else
        {
            Debug.LogError($"AudioManager: Could not load music at path '{MusicPath}'. Check if file is in Assets/Resources/Musics/");
        }
    }
}
