using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length;
    private float startPos;

    [Header("Ayarlar")]
    public GameObject cam; // Kamerayý Inspector'dan buraya sürükle
    public float parallaxFactor; // Hýz çarpaný (Örn: 0.8 uzak, 0.2 yakýn)

    void Start()
    {
        // 1. ADIM: Baþlangýç kaymasýný (zýplamayý) önlemek için ofset hesapla
        // Kamera 0'da deðilse bile arka plan Scene ekranýndaki yerini korur.
        startPos = transform.position.x - (cam.transform.position.x * parallaxFactor);

        // 2. ADIM: Geniþliði (Length) dinamik hesapla
        // Eðer bu obje bir Parent ise altýndaki tüm Sprite'larýn toplam geniþliðini bulur.
        SpriteRenderer mainSprite = GetComponent<SpriteRenderer>();

        if (mainSprite != null)
        {
            // Tek bir sprite varsa
            length = mainSprite.bounds.size.x;
        }
        else
        {
            // Eðer bu bir Parent objesiyse (Düz-Flip ikilisi gibi)
            SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
            length = 0;
            foreach (var child in children)
            {
                length += child.bounds.size.x;
            }
        }
    }

    // ParallaxEffect.cs içinde LateUpdate kýsmýný þu þekilde dene:
    void LateUpdate()
    {
        // Kameranýn baþlangýca göre ne kadar yol aldýðýný hesapla
        float camX = cam.transform.position.x;
        float distance = (camX * parallaxFactor);

        // Mevcut Y ve Z'yi koruyarak pozisyonu güncelle
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        // Sonsuz döngü kontrolü
        float temp = (camX * (1 - parallaxFactor));

        if (temp > startPos + length)
            startPos += length;
        else if (temp < startPos - length)
            startPos -= length;
    }
}