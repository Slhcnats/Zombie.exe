using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public float hiz = 2.5f;

    [Header("Ölüm Ayarları")]
    public GameObject cesetPrefab; // Kanlı Ceset Prefab'ı

    private Transform oyuncu;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    private float hasarBeklemeSuresi = 1f;
    private float sonHasarZamani = 0f;

    void Start()
    {
        oyuncu = GameObject.Find("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Zombilerin fiziksel olarak sağa sola savrulmasını engelliyoruz
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        if (oyuncu != null)
        {
            Vector2 yon = (oyuncu.position - transform.position).normalized;

            // --- ESKİ SİSTEME DÖNÜŞ (SADECE SAĞ/SOL) ---
            if (yon.x > 0)
                spriteRenderer.flipX = false;
            else if (yon.x < 0)
                spriteRenderer.flipX = true;

            // Zombinin yanlışlıkla yamuk durmamasını garantile (hep dik dursunlar)
            transform.rotation = Quaternion.identity;

            // Oyuncuya doğru hareket et
            transform.Translate(yon * hiz * Time.deltaTime, Space.World);
        }
    }

    void OnCollisionStay2D(Collision2D temas)
    {
        if (temas.gameObject.CompareTag("Player"))
        {
            if (Time.time >= sonHasarZamani + hasarBeklemeSuresi)
            {
                PlayerHealth ph = temas.gameObject.GetComponent<PlayerHealth>();

                if (ph != null)
                {
                    BossZombiDayaniklilik boss = GetComponent<BossZombiDayaniklilik>();
                    int verilecekHasar = boss != null ? ph.maxHealth : 20;
                    ph.TakeDamage(verilecekHasar);
                    sonHasarZamani = Time.time;
                }
            }
        }
    }

    // Zombi öldüğünde...
    public void Ol()
    {
        if (cesetPrefab != null)
        {
            // Cesedi zombinin tam koordinatında yarat (rotasyon sıfır, yani dik şekilde)
            GameObject ceset = Instantiate(cesetPrefab, transform.position, Quaternion.identity);

            // Zombi ölmeden önce sağa mı bakıyordu sola mı? Bunu cesede kopyala!
            SpriteRenderer cesetSprite = ceset.GetComponent<SpriteRenderer>();
            if (cesetSprite != null && spriteRenderer != null)
            {
                cesetSprite.flipX = spriteRenderer.flipX;
            }

            // Cesedin 10 saniye sonra silinmesi
            Destroy(ceset, 10f);
        }

        // Zombinin kendisini anında yok et
        Destroy(gameObject);
    }
}
