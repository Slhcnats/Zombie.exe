using UnityEngine;

public class BulletDeleted : MonoBehaviour
{
    [Header("Ses Ayarları")]
    public AudioClip zombiOlumSesi;

    [Header("Efekt Ayarları")]
    public GameObject cesetPrefab;

    void Start()
    {
        Destroy(gameObject, 2f); // Mermiyi 2 saniye sonra sil
    }

    void OnTriggerEnter2D(Collider2D temasEdenObje)
    {
        if (temasEdenObje.CompareTag("Zombie"))
        {
            // 1. Skor Ekle
            GameManager yonetici = Object.FindAnyObjectByType<GameManager>();
            if (yonetici != null)
            {
                yonetici.SkorEkle(10);
            }

            // 2. Ölüm Sesini Çal
            if (zombiOlumSesi != null)
            {
                AudioSource.PlayClipAtPoint(zombiOlumSesi, Camera.main.transform.position, 2.0f);
            }

            // 3. --- SİHİRLİ RASTGELELİK KISMI ---
            if (cesetPrefab != null)
            {
                // Rastgele bir dönüş açısı hesapla (0 ile 360 derece arası)
                float rastgeleDonus = Random.Range(0f, 360f);
                Quaternion cesetAcisi = Quaternion.Euler(0, 0, rastgeleDonus);

                // Cesedi yarat: Zombinin olduğu konumda ama RASTGELE bir açıda!
                GameObject ceset = Instantiate(cesetPrefab, temasEdenObje.transform.position, cesetAcisi);

                // --- EK OLARAK: Rastgele Boyut (Opsiyonel) ---
                // Bazı cesetler biraz daha büyük veya küçük olsun (0.9 ile 1.2 arası)
                float rastgeleBoyut = Random.Range(0.9f, 1.2f);
                ceset.transform.localScale = new Vector3(rastgeleBoyut, rastgeleBoyut, 1f);

                // Cesedin 10 saniye sonra silinmesi (performans için)
                Destroy(ceset, 10f);
            }

            // 4. Zombiyi sil
            Destroy(temasEdenObje.gameObject);

            // 5. Mermiyi sil
            Destroy(gameObject);
        }
    }
}