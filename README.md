# Grid

Üç kişilik bir ekiple kendi isteğimizle yaptığımız 2D platform oyunu. Amaç, karakteri bölüm sonundaki kapıya ulaştırmak. Yolda toplanan coin'ler puan kazandırıyor, ama enerji sürekli azaldığı için hızlı olmak gerekiyor. Unity 6 ile geliştirildi.

## Oynanış

- Enerji zamanla azalıyor, hareket ederken daha hızlı bitiyor. Yoldaki enerji toplanabilirleri barı dolduruyor; enerji biterse oyun bitiyor.
- Coin'ler puan veriyor.
- Hareketli platformlar ve boşluklar var, boşluğa düşünce oyun bitiyor.
- Üç bölüm var. Bölüm sonundaki kapıya ulaşınca sonraki bölüme geçiliyor.
- Ana menü, duraklatma, oyun bitti ve bölüm tamamlandı ekranları; parallax arka plan, müzik ve ses efektleri.

## Kontroller

| Tuş | İşlev |
| --- | --- |
| A / D | Sola / sağa hareket |
| W | Zıplama (havadayken bir kez daha basınca çift zıplama) |
| Shift | Dash (kullandıktan sonra bir süre bekleme var) |

## Ekip

- **Furkan Oğuzay**: Proje yönetimi, programlama
- **Nisa Yazoğlu**: Programlama, karakter ve nesne animasyonları
- **Ayşe Eda Nur Öztürk**: Karakter tasarımı, menü ve oyun içi görseller

## Kullanılan hazır içerikler

- Ses efektleri: HALFTONE SFX Pack LITE (VOiD1 Gaming)
- Arka plan: Night Mountains Lake paketi
- TextMesh Pro (Unity)

## Projeyi açmak için

1. Unity Hub'da **Add → Add project from disk** ile proje klasörünü seçin.
2. Projeyi Unity 6.3 (6000.3.3f1) ile açın. İlk açılışta Unity eksik dosyaları kendisi oluşturduğu için birkaç dakika sürebilir.
3. Oyunu `Assets/Scenes/MainMenu` sahnesinden başlatın.
