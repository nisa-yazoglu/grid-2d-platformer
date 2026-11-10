using UnityEngine;

public class KameraTakip : MonoBehaviour
{
    public Transform hedef;
    public float beklemeMesafesi = 2.0f;
    public float yumusaklik = 0.15f;

    private Vector3 mevcutHiz = Vector3.zero;

    [Header("Sınır Ayarları")]
    [Tooltip("Kamera sınırlarını aktif etmek için bu kutucuğu işaretleyin.")]
    public bool sinirlariAktifEt = false;

    [Header("Sınır Değerleri Nasıl Belirlenir?")]
    [Tooltip("Oyun sahnesinde kamerayı en sola götürün ve o anki Transform X değerini buraya yazın.")]
    public float minX;
    [Tooltip("Oyun sahnesinde kamerayı en sağa götürün ve o anki Transform X değerini buraya yazın.")]
    public float maxX;
    [Tooltip("Oyun sahnesinde kamerayı en aşağı götürün ve o anki Transform Y değerini buraya yazın.")]
    public float minY;
    [Tooltip("Oyun sahnesinde kamerayı en yukarı götürün ve o anki Transform Y değerini buraya yazın.")]
    public float maxY;

    [Header("Ekstra Ayarlar")]
    [Tooltip("Kamerayı karakterin merkezinden yukarı veya aşağı kaydırır. Pozitif değer vererek karakterin üst kısmını daha çok görebilirsiniz.")]
    public float yOffset = 1.0f; // Varsayılan olarak biraz yukarıyı gösterelim

    void LateUpdate()
    {
        if (hedef == null) return;

        // Mevcut kamera pozisyonunu baz alıyoruz
        float hedefX = transform.position.x;
        float hedefY = transform.position.y;

        // --- X EKSENİ HESAPLAMA ---
        float mesafeX = hedef.position.x - transform.position.x;
        if (Mathf.Abs(mesafeX) > beklemeMesafesi)
        {
            hedefX = hedef.position.x - (Mathf.Sign(mesafeX) * beklemeMesafesi);
        }

        // --- Y EKSENİ HESAPLAMA ---
        // Karakterin Y pozisyonuna offset ekleyerek kameranın dikeyde nerede durması gerektiğini belirliyoruz.
        float hedeflenenY = hedef.position.y + yOffset;
        float mesafeY = hedeflenenY - transform.position.y;

        if (Mathf.Abs(mesafeY) > beklemeMesafesi)
        {
            hedefY = hedeflenenY - (Mathf.Sign(mesafeY) * beklemeMesafesi);
        }

        // --- SINIRLANDIRMA (CLAMPING) ---
        if (sinirlariAktifEt)
        {
            // Kullanıcı yanlışlıkla min > max girerse diye önlem alıyoruz
            float realMinX = Mathf.Min(minX, maxX);
            float realMaxX = Mathf.Max(minX, maxX);
            float realMinY = Mathf.Min(minY, maxY);
            float realMaxY = Mathf.Max(minY, maxY);

            hedefX = Mathf.Clamp(hedefX, realMinX, realMaxX);
            hedefY = Mathf.Clamp(hedefY, realMinY, realMaxY);
        }

        // --- HAREKET KISMI (Z Eksenini Sabitleyen SmoothDamp) ---
        // Z eksenini direkt -10f olarak sabitleyerek parallax derinlik hatalarını önlüyoruz.
        Vector3 hedefPozisyon = new Vector3(hedefX, hedefY, -10f);

        // transform.position'ı doğrudan güncelliyoruz
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
            // Not: Bu sadece merkez noktanın sınırlarını gösterir.
            Gizmos.DrawWireCube(new Vector3(centerX, centerY, 0), new Vector3(sizeX, sizeY, 1));
        }
    }
}
