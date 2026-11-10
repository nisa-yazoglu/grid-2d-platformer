using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length;
    private float startPos;

    [Header("Ayarlar")]
    public GameObject cam;
    public float parallaxFactor; // Hız çarpanı (Örn: 0.8 uzak, 0.2 yakın)

    void Start()
    {
        // Başlangıç kaymasını önlemek için ofset hesapla

        startPos = transform.position.x - (cam.transform.position.x * parallaxFactor);

        // Genişliği hesapla
        // Eğer bu obje bir Parent ise altındaki tüm Sprite'ların toplam genişliğini bulur.
        SpriteRenderer mainSprite = GetComponent<SpriteRenderer>();

        if (mainSprite != null)
        {
            // Tek bir sprite varsa
            length = mainSprite.bounds.size.x;
        }
        else
        {
            // Eğer bu bir Parent objesiyse (Düz-Flip ikilisi gibi)
            SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
            length = 0;
            foreach (var child in children)
            {
                length += child.bounds.size.x;
            }
        }
    }

    void LateUpdate()
    {
        // Kameranın başlangıca göre ne kadar yol aldığını hesapla
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
