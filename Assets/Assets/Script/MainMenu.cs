using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// "Oyna" (Play) butonuna basıldığında çağrılır.
    /// Build Settings'deki bir sonraki sahneye geçer (Genelde Level 1).
    /// </summary>
    public void PlayGame()
    {
        // Build Settings'de MainMenu 0. sırada, Oyun ise 1. sırada olmalı.
        // Bu yüzden index 1'i yüklüyoruz.
        SceneManager.LoadScene(1);
    }
    
    /// <summary>
    /// Oyundan çıkış butonu (Opsiyonel)
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor...");
        Application.Quit();
    }
}
