using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FallDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Çarpan obje Player ise
        if (other.CompareTag("Player"))
        {
            Debug.Log("Oyuncu boşluğa düştü!");

            // GameManager üzerinden Game Over fonksiyonunu çağır
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("FallDetector: GameManager bulunamadı!");
            }
        }
    }
}
