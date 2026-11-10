using UnityEngine;

public class HareketliPlatform : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [Tooltip("Platformun gideceği noktalar")]
    public Transform[] hareketNoktalari;

    [Tooltip("Platformun hareket hızı")]
    public float hiz = 2.0f;

    [Tooltip("Noktaya ulaştığında bekleme süresi")]
    public float beklemeSuresi = 0.5f;

    [Tooltip("Döngü türü. İşaretli değilse: A->B->C->B->A yapar. İşaretliyse: A->B->C->A yapar.")]
    public bool daireselDongu = false;

    private int mevcutNoktaIndeksi = 0;
    private bool ileriGidiyor = true;
    private float beklemeSayaci = 0f;

    void Start()
    {
        // Eğer hiç nokta atanmadıysa uyarı ver
        if (hareketNoktalari == null || hareketNoktalari.Length == 0)
        {
            Debug.LogWarning("HareketliPlatform: Hiç hareket noktası atanmamış!");
            return;
        }
    }

    void Update()
    {
        if (hareketNoktalari == null || hareketNoktalari.Length < 2) return;

        HareketEt();
    }

    void HareketEt()
    {
        // Hedef noktayı belirle
        Vector3 hedefNokta = hareketNoktalari[mevcutNoktaIndeksi].position;

        // Z eksenini koru

        hedefNokta.z = transform.position.z;

        // Hedefe doğru hareket et
        transform.position = Vector3.MoveTowards(transform.position, hedefNokta, hiz * Time.deltaTime);

        Debug.Log($"[{gameObject.name}] Gidilen Hedef: {hedefNokta} | Şu anki Konum: {transform.position}");

        // Hedefe vardık mı?
        if (Vector3.Distance(transform.position, hedefNokta) < 0.01f)
        {
            // Bekleme süresi sayacı
            if (beklemeSayaci < beklemeSuresi)
            {
                beklemeSayaci += Time.deltaTime;
                return;
            }

            // Bekleme bitti, bir sonraki hedefi seç
            beklemeSayaci = 0;
            SonrakiNoktayiBelirle();
        }
    }

    void SonrakiNoktayiBelirle()
    {
        if (daireselDongu)
        {
            // 0 -> 1 -> 2 -> 0 -> 1 ... şeklinde gider
            mevcutNoktaIndeksi = (mevcutNoktaIndeksi + 1) % hareketNoktalari.Length;
        }
        else
        {
            // 0 -> 1 -> 2 -> 1 -> 0 ... şeklinde gider (Ping-Pong)
            if (ileriGidiyor)
            {
                mevcutNoktaIndeksi++;
                if (mevcutNoktaIndeksi >= hareketNoktalari.Length - 1)
                {
                    ileriGidiyor = false;
                }
            }
            else
            {
                mevcutNoktaIndeksi--;
                if (mevcutNoktaIndeksi <= 0)
                {
                    ileriGidiyor = true;
                }
            }
        }
    }

    // Editörde yolları çizgi ile göstermek için
    void OnDrawGizmos()
    {
        if (hareketNoktalari == null || hareketNoktalari.Length < 2) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < hareketNoktalari.Length - 1; i++)
        {
            if (hareketNoktalari[i] != null && hareketNoktalari[i+1] != null)
                Gizmos.DrawLine(hareketNoktalari[i].position, hareketNoktalari[i+1].position);
        }

        // Eğer dairesel ise son noktadan başa çizgi çek
        if (daireselDongu && hareketNoktalari[0] != null && hareketNoktalari[hareketNoktalari.Length - 1] != null)
        {
            Gizmos.DrawLine(hareketNoktalari[hareketNoktalari.Length - 1].position, hareketNoktalari[0].position);
        }
    }
    // Karakter platformun üzerine çıktığında
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    // Karakter platformdan ayrıldığında
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
