using UnityEngine;

public class KameraTakip : MonoBehaviour
{
    public Transform hedef;
    public float beklemeMesafesi = 2.0f;
    public float yumusaklik = 0.15f;

    private Vector3 mevcutHiz = Vector3.zero;

    [Header("Sınır Ayarları")]
    [Tooltip("Kamera sınırlarını aktif eder")]
    public bool sinirlariAktifEt = false;

    [Header("Sınır Değerleri")]
    [Tooltip("Kameranın gidebileceği en küçük X değeri")]
    public float minX;
    [Tooltip("Kameranın gidebileceği en büyük X değeri")]
    public float maxX;
    [Tooltip("Kameranın gidebileceği en küçük Y değeri")]
    public float minY;
    [Tooltip("Kameranın gidebileceği en büyük Y değeri")]
    public float maxY;

    [Header("Ekstra Ayarlar")]
    [Tooltip("Kameranın karaktere göre dikey kayması")]
    public float yOffset = 1.0f;

    void LateUpdate()
    {
        if (hedef == null) return;

        float hedefX = transform.position.x;
        float hedefY = transform.position.y;

        // X ekseni
        float mesafeX = hedef.position.x - transform.position.x;
        if (Mathf.Abs(mesafeX) > beklemeMesafesi)
        {
            hedefX = hedef.position.x - (Mathf.Sign(mesafeX) * beklemeMesafesi);
        }

        // Y ekseni

        float hedeflenenY = hedef.position.y + yOffset;
        float mesafeY = hedeflenenY - transform.position.y;

        if (Mathf.Abs(mesafeY) > beklemeMesafesi)
        {
            hedefY = hedeflenenY - (Mathf.Sign(mesafeY) * beklemeMesafesi);
        }

        // Sınırlandırma
        if (sinirlariAktifEt)
        {
            float realMinX = Mathf.Min(minX, maxX);
            float realMaxX = Mathf.Max(minX, maxX);
            float realMinY = Mathf.Min(minY, maxY);
            float realMaxY = Mathf.Max(minY, maxY);

            hedefX = Mathf.Clamp(hedefX, realMinX, realMaxX);
            hedefY = Mathf.Clamp(hedefY, realMinY, realMaxY);
        }

        // Kamerayı yumuşakça hedefe taşı

        Vector3 hedefPozisyon = new Vector3(hedefX, hedefY, -10f);

        transform.position = Vector3.SmoothDamp(transform.position, hedefPozisyon, ref mevcutHiz, yumusaklik);
    }

    // Seçili iken sahne ekranında sınırları çizdiren fonksiyon
    void OnDrawGizmosSelected()
    {
        if (sinirlariAktifEt)
        {
            Gizmos.color = Color.red;

            // Gerçek sınırları hesapla (ters girilme ihtimaline karşı)
            float realMinX = Mathf.Min(minX, maxX);
            float realMaxX = Mathf.Max(minX, maxX);
            float realMinY = Mathf.Min(minY, maxY);
            float realMaxY = Mathf.Max(minY, maxY);

            // Kutunun merkezi ve boyutlarını hesapla
            float centerX = (realMinX + realMaxX) / 2f;
            float centerY = (realMinY + realMaxY) / 2f;
            float sizeX = Mathf.Abs(realMaxX - realMinX);
            float sizeY = Mathf.Abs(realMaxY - realMinY);

            // Kamera görüş alanı (yaklaşık) için bir kutu çiz

            Gizmos.DrawWireCube(new Vector3(centerX, centerY, 0), new Vector3(sizeX, sizeY, 1));
        }
    }
}
