using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FinishGate : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("FinishGate: Bir obje tetikleyiciye girdi: " + other.gameObject.name + " Tag: " + other.tag);

        // Sadece "Player" tag'ine sahip objelerle etkileşime gir
        if (other.CompareTag("Player"))
        {
            Debug.Log("FinishGate: Player algılandı!");

            // LevelManager üzerinden bölüm bitişini tetikle
            if (LevelManager.Instance != null)
            {
                Debug.Log("FinishGate: LevelManager bulundu, LevelComplete çağrılıyor...");
                LevelManager.Instance.LevelComplete();
            }
            else
            {
                Debug.LogError("Sahne üzerinde LevelManager bulunamadı!");
            }
        }
    }
}
