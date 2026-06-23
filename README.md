# Zombie.exe

Zombie.exe, Unity ile geliştirilmiş 2D top-down hayatta kalma oyunudur. Oyuncu, giderek zorlaşan düşman dalgalarına karşı savaşır, harita değişimlerine uyum sağlar ve mümkün olan en yüksek skora ulaşmaya çalışır.

## Oyun Özellikleri

- **İki farklı harita:** Mezarlık ve Japon şehri, farklı düşman türleri ve silah akışı sunar.
- **Dalga sistemi:** Her dalgada düşman sayısı ve mücadele temposu artar.
- **Haritaya özel mücadele:** Mezarlıkta klasik zombilere karşı normal mermi, Japon şehrinde robot zombilere karşı hızlı lazer kullanılır.
- **Boss karşılaşmaları:** İlerleyen dalgalarda yüksek cana ve güçlü saldırılara sahip büyük boss düşmanlar gelir.
- **Perk seçimi:** Dalga geçişlerinde hareket hızı, maksimum can, şarjör kapasitesi, seri atış veya lazer gücü gibi üç seçenekten biri seçilir.
- **Savaş geri bildirimi:** Kan efektleri, cesetler, ekran hasar etkisi, mermi/lazer sesleri ve atmosfer sesi bulunur.
- **Kayıtlı rekor:** En yüksek skor ve ulaşılan dalga Game Over ekranında saklanır.
- **Game Over akışı:** Oyuncu yeniden deneyebilir, ana menüye dönebilir veya gösterim amaçlı reklam izleyerek oyuna devam edebilir.
- **Duraklatma menüsü:** Oyun sırasında `Esc` tuşu ile oyun duraklatılabilir.
- **TR / ENG arayüzü:** Ana menüden dil seçimi yapılabilir.

## Kontroller

| Tuş | İşlev |
| --- | --- |
| `WASD` | Hareket |
| Sol tık | Ateş et |
| `R` | Şarjör doldur (normal mermi) |
| `Esc` | Oyunu duraklat |

## Çalıştırma

### Hazır Windows sürümü

`Zombie.exeBuild` klasörü içindeki `Zombie.exe` dosyasını çalıştırın. Aynı klasördeki `Zombie.exe_Data`, `MonoBleedingEdge` ve `D3D12` klasörleri oyunun çalışması için gereklidir.

### Unity üzerinden

1. Projeyi Unity Hub ile açın.
2. `Assets/Scenes/MainMenu.unity` sahnesini açın.
3. Play tuşuna basın.

## Teknolojiler

- Unity 6
- C#
- TextMeshPro
- Universal Render Pipeline (URP)

## Proje Yapısı

- `Assets/Scenes`: Ana menü ve oyun sahneleri
- `Assets/Prefabs`: Oyuncu, zombi, robot zombi, boss ve mermi prefabları
- `Assets/Audio`: Müzik ve oyun sesleri
- `Assets`: Dalga, oyuncu, düşman, perk ve Game Over sistemlerinin C# kodları
