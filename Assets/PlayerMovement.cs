using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float hareketHizi = 5f;

    // Karakterin görselini çevirmek için SpriteRenderer'ı tanımlıyoruz
    private SpriteRenderer spriteRenderer;

    // --- YENİ EKLENEN: Animasyon Kontrolcüsü ---
    private Animator anim;

    void Start()
    {
        // Kod başladığında karakterin üzerindeki görseli bul
        spriteRenderer = GetComponent<SpriteRenderer>();

        // --- YENİ EKLENEN: Animator'u bul ve tanımla ---
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // --- 1. HAREKET KISMI ---
        float yatayGirdi = Input.GetAxisRaw("Horizontal");
        float dikeyGirdi = Input.GetAxisRaw("Vertical");
        Vector2 hareketYonu = new Vector2(yatayGirdi, dikeyGirdi).normalized;

        transform.Translate(hareketYonu * hareketHizi * Time.deltaTime, Space.World);

        // --- YENİ EKLENEN: Yürüme Animasyonunu Tetikleme ---
        if (hareketYonu != Vector2.zero)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        // --- 2. YÖNELME (FAREYE BAKMA) KISMI ---
        Vector3 farePozisyonu = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        farePozisyonu.z = 0f;

        Vector2 bakisYonu = (farePozisyonu - transform.position).normalized;

        if (bakisYonu.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (bakisYonu.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}