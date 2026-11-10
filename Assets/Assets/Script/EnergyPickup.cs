using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Bu obje alındığında karaktere eklenecek enerji miktarı")]
    public float energyAmount = 10f;

    [Tooltip("Efekt ses klibi (opsiyonel)")]
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"EnergyPickup: Çarpışma algılandı. Çarpan Obje: {other.name}, Tag: {other.tag}");

        // Çarpan nesne Player mı?
        if (other.CompareTag("Player"))
        {
            // PlayerEnergy scriptini bul
            PlayerEnergy playerEnergy = other.GetComponent<PlayerEnergy>();

            if (playerEnergy != null)
            {
                Debug.Log($"EnergyPickup: PlayerEnergy bulundu. {energyAmount} enerji ekleniyor.");
                // Enerji ekle
                playerEnergy.AddEnergy(energyAmount);

                // Ses çal ve objeyi yok et
                Collect();
            }
            else
            {
                Debug.LogError("EnergyPickup: Player objesinde 'PlayerEnergy' scripti BULUNAMADI! Lütfen Player'a scripti eklediğinizden emin olun.");
            }
        }
        else
        {
            Debug.LogWarning($"EnergyPickup: Çarpan obje 'Player' tagine sahip değil. Algılanan Tag: '{other.tag}'");
        }
    }

    void Collect()
    {
        // Ses çal
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Obje yok edilir
        Destroy(gameObject);
    }
}
