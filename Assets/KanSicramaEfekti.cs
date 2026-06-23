using UnityEngine;

public class KanSicramaEfekti : MonoBehaviour
{
    private Vector3 baslangicKonumu;
    private Vector3 hedefKonum;
    private Vector3 baslangicOlcegi;
    private SpriteRenderer spriteRenderer;
    private float sure;
    private float gecenSure;

    public void Baslat(Vector2 yon, float mesafe, float efektSuresi)
    {
        baslangicKonumu = transform.position;
        hedefKonum = baslangicKonumu + (Vector3)(yon * mesafe);
        baslangicOlcegi = transform.localScale;
        sure = efektSuresi;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderer == null || sure <= 0f) return;

        gecenSure += Time.deltaTime;
        float ilerleme = Mathf.Clamp01(gecenSure / sure);
        float yumusakIlerleme = Mathf.SmoothStep(0f, 1f, ilerleme);

        transform.position = Vector3.Lerp(baslangicKonumu, hedefKonum, yumusakIlerleme);
        transform.localScale = Vector3.Lerp(baslangicOlcegi, baslangicOlcegi * 1.5f, ilerleme);

        Color renk = spriteRenderer.color;
        renk.a = Mathf.Lerp(0.85f, 0f, ilerleme);
        spriteRenderer.color = renk;

        if (ilerleme >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
