using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Oyun Ayarları")]

    [Header("UI Referansları")]
    [Tooltip("Puanın yazacağı Text")]
    public TextMeshProUGUI scoreText;

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
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI();

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
    }

    public void AddScore(int amount)
    {
        if (!isGameActive) return;

        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void UpdateEnergyUI(float current, float max)
    {
        if (energyBarImage != null)
        {
            // Oranı hesapla (0 ile 1 arasında)
            float fillAmount = Mathf.Clamp01(current / max);
            energyBarImage.fillAmount = fillAmount;
        }
    }

    public void PauseGame()
    {
        if (!isGameActive) return;

        Time.timeScale = 0f; // Zamanı durdur

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (!isGameActive) return;

        Time.timeScale = 1f; // Zamanı devam ettir

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

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
