using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy Settings")]
    [Tooltip("Maksimum enerji miktarı")]
    public float maxEnergy = 100f;

    [Tooltip("Karakter dururken saniyede harcanan enerji")]
    public float drainRateIdle = 1f;

    [Tooltip("Karakter hareket ederken (koşma, zıplama, dash) saniyede harcanan enerji")]
    public float drainRateMoving = 3f;

    // Mevcut enerji
    private float currentEnergy;

    // Referanslar
    private Char1_0Controller playerController;

    public float CurrentEnergy => currentEnergy;

    void Start()
    {
        // Enerjiyi fulle
        currentEnergy = maxEnergy;

        // Controller referansını al
        playerController = GetComponent<Char1_0Controller>();
        if (playerController == null)
        {
            Debug.LogError("PlayerEnergy: Char1_0Controller bulunamadı! Lütfen bu scripti karakterin üzerine atayın.");
        }
    }

    void Update()
    {
        if (currentEnergy > 0)
        {
            DrainEnergy();
        }
        else
        {
            currentEnergy = 0;
            // Enerji bittiğinde ne olacağını buraya ekleyebilirsiniz (Örn: Oyun Bitti, Hareket Kısıtlama vs.)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }

        // UI Güncelleme (Her frame çağırmak yerine event bazlı da yapılabilir ama basitlik için böyle bırakıyoruz)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateEnergyUI(currentEnergy, maxEnergy);
        }
    }

    void DrainEnergy()
    {
        float drainRate = drainRateIdle;

        if (playerController != null)
        {
            // Hareket ediyor mu, zıplıyor mu veya dash atıyor mu kontrol et
            // Char1_0Controller'da public propertyler üzerinden okuyoruz
            bool isMoving = Mathf.Abs(playerController.MoveDirection) > 0.1f;
            bool isJumping = !playerController.IsGrounded; // Havadaysa zıplıyordur (veya düşüyordur)
            bool isDashing = playerController.IsDashing;

            if (isMoving || isJumping || isDashing)
            {
                drainRate = drainRateMoving;
            }
        }

        // Enerjiyi azalt
        currentEnergy -= drainRate * Time.deltaTime;

        // Enerjiyi clamp'le (0'ın altına düşmesin)
        if (currentEnergy < 0) currentEnergy = 0;
        
        // Debug için konsola yaz (Sadece test ederken açık kalsın)
        // Debug.Log($"Enerji: {currentEnergy:F1} / {maxEnergy} - Tüketim Hızı: {drainRate}");
    }

    /// <summary>
    /// Enerji ekler (Collectibles vb. için)
    /// </summary>
    /// <param name="amount">Eklenecek miktar</param>
    public void AddEnergy(float amount)
    {
        currentEnergy += amount;
        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
    }
}
