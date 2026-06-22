using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Hangi mermiyi fırlatacağımızı Unity'den seçeceğiz
    public GameObject mermiPrefab;

    // Merminin uçuş hızı
    public float mermiHizi = 15f;

    [Header("Ses Ayarları")]
    public AudioSource hoparlor; // Sesi çalacak cihaz
    public AudioClip atesSesi;   // Çalınacak kaset (ses dosyası)

    // --- YENİ EKLENEN: Animasyon Kontrolcüsü ---
    private Animator anim;

    void Start()
    {
        // --- YENİ EKLENEN: Kod başladığında Animator'u bul ---
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Farenin sol tuşuna (0) basıldığında
        if (Input.GetMouseButtonDown(0))
        {
            AtesEt();
        }

        // --- YENİ EKLENEN: Klavyeden 'R' tuşuna basıldığında Şarjör (Reload) animasyonu oynasın ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Reload");
        }
    }

    void AtesEt()
    {
        // --- YENİ EKLENEN: Ateş edildiği an 'Shoot' animasyonunu tetikle ---
        anim.SetTrigger("Shoot");

        Vector3 farePozisyonu = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        farePozisyonu.z = 0f;

        Vector2 atesYonu = (farePozisyonu - transform.position).normalized;

        float aci = Mathf.Atan2(atesYonu.y, atesYonu.x) * Mathf.Rad2Deg;

        Quaternion mermiDonusu = Quaternion.Euler(0f, 0f, aci);

        GameObject yeniMermi = Instantiate(mermiPrefab, transform.position, mermiDonusu);

        Rigidbody2D rb = yeniMermi.GetComponent<Rigidbody2D>();
        rb.linearVelocity = atesYonu * mermiHizi;

        if (hoparlor != null && atesSesi != null)
        {
            hoparlor.PlayOneShot(atesSesi);
        }

        if (KameraTitremesi.instance != null)
        {
            KameraTitremesi.instance.Titret(0.1f, 0.05f);
        }
    }
}