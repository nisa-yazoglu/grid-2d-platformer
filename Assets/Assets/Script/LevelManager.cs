using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class LevelManager : MonoBehaviour
{
    // Singleton örneği
    public static LevelManager Instance { get; private set; }

    [Header("UI Referansları")]
    [Tooltip("Bölüm bittiğinde açılacak panel")]
    public GameObject victoryPanel;

    [Header("Ses Efektleri")]
    [Tooltip("Bölüm Tamamlanma Sesi")]
    public AudioClip levelCompleteSound;

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton yapısı
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; // 2D Ses
        }
    }

    private void Start()
    {
        // Başlangıçta zafer paneli kapalı olsun
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Bölüm başarıyla tamamlandığında çağrılır.
    /// </summary>
    public void LevelComplete()
    {
        Debug.Log("Bölüm Tamamlandı!");

        // Oyunu durdurmak istersek burayı açabiliriz.
        // Time.timeScale = 0f; 

        // Zafer panelini göster
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Bitiş sesi çal
        if (audioSource != null && levelCompleteSound != null)
        {
            audioSource.PlayOneShot(levelCompleteSound);
        }

        // --- NEW: Karakterin hareketini durdur ---
        Char1_0Controller player = FindAnyObjectByType<Char1_0Controller>();
        if (player != null)
        {
            player.SetInputActive(false);
        }
    }

    /// <summary>
    /// Bir sonraki sahneye geçer.
    /// </summary>
    public void LoadNextLevel()
    {
        // Zamanı normale döndür (eğer durdurduysak)
        Time.timeScale = 1f;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Eğer sonraki sahne varsa yükle, yoksa başa dön (veya Ana Menüye)
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Son sahneye gelindi! Ana menüye dönülüyor (Index 0).");
            SceneManager.LoadScene(0);
        }
    }

    /// <summary>
    /// Mevcut bölümü yeniden başlatır.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
