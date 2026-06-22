using UnityEngine;
using System.Collections;

public class ZombiHasarFlash : MonoBehaviour
{
    [Header("Görsel Ayarlar")]
    public SpriteRenderer spriteRenderer;
    public Color flashRengi = Color.white;
    public float etkiSuresi = 0.15f; // BulletDeleted içindeki gecikme süremizle aynı

    [Header("Titreme (Shake) Ayarları")]
    [Tooltip("Zombinin sağa sola ne kadar şiddetli sarsılacağı")]
    public float titremeSiddeti = 0.1f;

    private Color orijinalRenk;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        orijinalRenk = spriteRenderer.color;
    }

    public void HasarEfectiOynat()
    {
        StartCoroutine(FlashVeTitremeRoutine());
    }

    private IEnumerator FlashVeTitremeRoutine()
    {
        // 1. Zombiyi bembeyaz yap
        spriteRenderer.color = flashRengi;

        // 2. Titremeye başlamadan önce zombinin durduğu yeri hafızaya al
        Vector3 orijinalPozisyon = transform.position;
        float gecenZaman = 0f;

        // 3. 0.15 saniye boyunca (zombi silinene kadar) zombiyi titret
        while (gecenZaman < etkiSuresi)
        {
            // X ve Y ekseninde ufak, rastgele bir sarsıntı hesapla
            float rastgeleX = Random.Range(-1f, 1f) * titremeSiddeti;
            float rastgeleY = Random.Range(-1f, 1f) * titremeSiddeti;

            // Zombiyi o rastgele sarsıntı noktasına ışınla
            transform.position = orijinalPozisyon + new Vector3(rastgeleX, rastgeleY, 0);

            gecenZaman += Time.deltaTime;

            // Bir sonraki ekran karesini (frame) bekle
            yield return null;
        }

        // 4. Eğer zombi tek mermide ölmediyse (canı hala varsa) her şeyi normale döndür
        transform.position = orijinalPozisyon;
        spriteRenderer.color = orijinalRenk;
    }
}