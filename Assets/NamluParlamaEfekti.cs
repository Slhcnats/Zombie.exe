using UnityEngine;

public class NamluParlamaEfekti : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 baslangicOlcegi;
    private float sure;
    private float gecenSure;

    public void Baslat(float efektSuresi)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baslangicOlcegi = transform.localScale;
        sure = efektSuresi;
    }

    void Update()
    {
        if (spriteRenderer == null || sure <= 0f) return;

        gecenSure += Time.deltaTime;
        float ilerleme = Mathf.Clamp01(gecenSure / sure);

        transform.localScale = Vector3.Lerp(baslangicOlcegi, baslangicOlcegi * 1.8f, ilerleme);
        Color renk = spriteRenderer.color;
        renk.a = Mathf.Lerp(0.95f, 0f, ilerleme);
        spriteRenderer.color = renk;

        if (ilerleme >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
