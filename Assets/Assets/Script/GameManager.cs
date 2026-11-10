using UnityEngine;
using TMPro; // TextMeshPro kütüphanesi (Unity'nin modern text sistemi)
using UnityEngine.SceneManagement; // İlerde sahne yenilemek gerekirse diye

public class GameManager : MonoBehaviour
{
    // Singleton: Her yerden GameManager.Instance diyerek ulaşmak için
    public static GameManager Instance { get; private set; }

    [Header("Oyun Ayarları")]
    // [Tooltip("Oyun süresi (saniye)")]
    // public float gameTime = 60f; // Zaman sayacı kaldırıldı

    [Header("UI Referansları")]
    [Tooltip("Puanın yazacağı Text")]
    public TextMeshProUGUI scoreText;

    // [Tooltip("Sürenin yazacağı Text")]
    // public TextMeshProUGUI timerText; // Zaman sayacı UI kaldırıldı

    [Tooltip("Enerji Barı Görseli (Image - Fill Type: Horizontal)")]
    public UnityEngine.UI.Image energyBarImage;

    [Tooltip("Oyun Bitti Paneli (Game Over)")]
    public GameObject gameOverPanel;

    [Tooltip("Pause Paneli")]
    public GameObject pausePanel;

    // Private değişkenler
    private int score = 0;
    private bool isGameActive = true;

    void Awake()
    {
        // Singleton Pattern kurulumu
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Eğer sahneler arası geçişte yok olmasın isterseniz bunu açın
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI();
        // UpdateTimerUI(); // Kaldırıldı
        
        // Enerji barı rengini mavi yap
        if (energyBarImage != null)
        {
            energyBarImage.color = Color.blue;
        }
        else
        {
            Debug.LogError("GameManager: Enerji Barı Resmi (Image) atanmamış! Lütfen inspector'dan atayın.");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    void Update()
    {
        /*
        if (isGameActive)
        {
            // Süreyi geri say
            gameTime -= Time.deltaTime;

            if (gameTime <= 0)
            {
                gameTime = 0;
                GameOver();
            }

            // Her frame'de değil de saniye değiştikçe güncellemek daha performanslıdır ama şimdilik her frame güncelleyelim (akıcı görünsün)
            UpdateTimerUI();
        }
        */
    }

    /// <summary>
    /// Skora puan ekler
    /// </summary>
    /// <param name="amount">Eklenecek puan miktarı</param>
    public void AddScore(int amount)
    {
        if (!isGameActive) return;

        score += amount;
        UpdateScoreUI();
    }

    /// <summary>
    /// UI üzerindeki Skor yazısını günceller
    /// </summary>
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    /*
    /// <summary>
    /// UI üzerindeki Süre yazısını günceller
    /// </summary>
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // Süreyi 00 formatında göster (tam sayı)
            timerText.text = "Time: " + Mathf.CeilToInt(gameTime).ToString();
        }
    }
    */

    /// <summary>
    /// Enerji barını günceller
    /// </summary>
    /// <param name="current">Mevcut enerji</param>
    /// <param name="max">Maksimum enerji</param>
    public void UpdateEnergyUI(float current, float max)
    {
        if (energyBarImage != null)
        {
            // Oranı hesapla (0 ile 1 arasında)
            float fillAmount = Mathf.Clamp01(current / max);
            energyBarImage.fillAmount = fillAmount;
        }
    }

    /// <summary>
    /// Oyunu duraklatır
    /// </summary>
    public void PauseGame()
    {
        if (!isGameActive) return;

        Time.timeScale = 0f; // Zamanı durdur
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    /// <summary>
    /// Oyunu devam ettirir
    /// </summary>
    public void ResumeGame()
    {
        if (!isGameActive) return;

        Time.timeScale = 1f; // Zamanı devam ettir

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    /// <summary>
    /// Oyun Bitti (Game Over) işlemlerini başlatır
    /// </summary>
    public void GameOver()
    {
        if (!isGameActive) return; // Zaten oyun bittiyse tekrar yapma

        isGameActive = false;
        Debug.Log("Oyun Bitti! Enerji Tükendi.");

        // Karakterin hareketini durdur
        Char1_0Controller player = FindAnyObjectByType<Char1_0Controller>();
        if (player != null)
        {
            player.SetInputActive(false);
        }

        // Game Over panelini aç
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
             Debug.LogWarning("GameManager: GameOverPanel atanmamış!");
        }
    }
}
