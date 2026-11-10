using UnityEngine;

public class PausePanel : MonoBehaviour
{
    /// <summary>
    /// "Devam Et" butonuna basıldığında
    /// </summary>
    public void OnResumeClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    /// <summary>
    /// "Tekrar Başla" butonuna basıldığında
    /// </summary>
    public void OnRestartClicked()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartLevel();
        }
    }

    /// <summary>
    /// "Ana Menü" (Opsiyonel)
    /// </summary>
    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f; // Zamanı düzeltmeyi unutma
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
