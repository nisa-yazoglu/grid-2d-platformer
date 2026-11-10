using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    /// <summary>
    /// "Tekrar Dene" butonu tıklandığında çağrılacak.
    /// </summary>
    public void OnRetryClicked()
    {
        // Zamanı normale döndür
        Time.timeScale = 1f;

        // Mevcut sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Ana Menü butonu tıklandığında çağrılacak (Opsiyonel).
    /// </summary>
    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
