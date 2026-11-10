using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Bu coini alınca kaç puan verilecek?")]
    public int scorePoint = 10;

    [Tooltip("Efekt ses klibi (opsiyonel)")]
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Çarpan nesnenin Player olup olmadığını kontrol et
        // Not: Player objenizin Tag'inin "Player" olduğundan emin olun!
        if (other.CompareTag("Player"))
        {
            // Önce Controller'ı almaya çalış
            Char1_0Controller controller = other.GetComponent<Char1_0Controller>();
            if (controller != null)
            {
                controller.TriggerCollectAction();
            }
            
            Collect();
        }
        else if (other.GetComponent<Char1_0Controller>() != null)
        {
            // Tag olmasa bile component varsa yine de topla
            other.GetComponent<Char1_0Controller>().TriggerCollectAction();
            Collect();
        }
    }

    void Collect()
    {
        // GameManager'a puan eklemesini söyle
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scorePoint);
        }
        else
        {
            Debug.LogWarning("Sahned GameManager bulunamadı! Puan eklenemedi.");
        }

        // Ses çalma (opsiyonel)
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Kendini yok et
        Destroy(gameObject);
    }
}
