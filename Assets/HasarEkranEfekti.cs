using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HasarEkranEfekti : MonoBehaviour
{
    private static HasarEkranEfekti instance;

    private CanvasGroup grup;
    private Coroutine aktifRutin;
    private const float MaksAlpha = 0.72f;
    private const float EfektSuresi = 0.75f;

    public static void Goster()
    {
        if (instance == null)
        {
            GameObject efektObjesi = new GameObject("HasarEkranEfekti");
            instance = efektObjesi.AddComponent<HasarEkranEfekti>();
        }

        instance.EfektiBaslat();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        ArayuzuOlustur();
    }

    void ArayuzuOlustur()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObjesi = new GameObject("HasarCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObjesi.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        transform.SetParent(canvas.transform, false);
        transform.SetAsLastSibling();

        RectTransform rect = gameObject.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        grup = gameObject.AddComponent<CanvasGroup>();
        grup.alpha = 0f;
        grup.blocksRaycasts = false;
        grup.interactable = false;

        Image bulaniklik = new GameObject("KirmiziBulaniklik", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        bulaniklik.transform.SetParent(transform, false);
        RectTransform bulaniklikRect = bulaniklik.GetComponent<RectTransform>();
        bulaniklikRect.anchorMin = Vector2.zero;
        bulaniklikRect.anchorMax = Vector2.one;
        bulaniklikRect.offsetMin = Vector2.zero;
        bulaniklikRect.offsetMax = Vector2.zero;
        bulaniklik.color = new Color(0.55f, 0f, 0f, 0.22f);
        bulaniklik.raycastTarget = false;

        Sprite kanSprite = KanSpriteOlustur();
        KoseKaniOlustur(kanSprite, "SolUst", new Vector2(0f, 1f), new Vector2(0f, 1f), 0f);
        KoseKaniOlustur(kanSprite, "SagUst", Vector2.one, Vector2.one, -90f);
        KoseKaniOlustur(kanSprite, "SolAlt", Vector2.zero, Vector2.zero, 90f);
        KoseKaniOlustur(kanSprite, "SagAlt", new Vector2(1f, 0f), new Vector2(1f, 0f), 180f);
    }

    void KoseKaniOlustur(Sprite sprite, string isim, Vector2 anchor, Vector2 pivot, float zRotasyon)
    {
        Image kan = new GameObject(isim, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        kan.transform.SetParent(transform, false);

        RectTransform rect = kan.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(300f, 300f);
        rect.localRotation = Quaternion.Euler(0f, 0f, zRotasyon);

        kan.sprite = sprite;
        kan.color = new Color(1f, 1f, 1f, 0.9f);
        kan.raycastTarget = false;
    }

    Sprite KanSpriteOlustur()
    {
        const int boyut = 256;
        Texture2D texture = new Texture2D(boyut, boyut, TextureFormat.RGBA32, false);

        for (int y = 0; y < boyut; y++)
        {
            for (int x = 0; x < boyut; x++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }

        for (int i = 0; i < 24; i++)
        {
            Vector2 merkez = new Vector2(Random.Range(20f, 236f), Random.Range(20f, 236f));
            float yaricap = Random.Range(8f, 38f);
            Color renk = new Color(Random.Range(0.45f, 0.8f), 0f, 0f, Random.Range(0.45f, 0.85f));

            int minX = Mathf.Max(0, Mathf.FloorToInt(merkez.x - yaricap));
            int maxX = Mathf.Min(boyut - 1, Mathf.CeilToInt(merkez.x + yaricap));
            int minY = Mathf.Max(0, Mathf.FloorToInt(merkez.y - yaricap));
            int maxY = Mathf.Min(boyut - 1, Mathf.CeilToInt(merkez.y + yaricap));

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float uzaklik = Vector2.Distance(new Vector2(x, y), merkez);
                    if (uzaklik > yaricap) continue;

                    float alpha = Mathf.Pow(1f - uzaklik / yaricap, 0.55f) * renk.a;
                    Color mevcut = texture.GetPixel(x, y);
                    if (alpha > mevcut.a)
                    {
                        texture.SetPixel(x, y, new Color(renk.r, 0f, 0f, alpha));
                    }
                }
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, boyut, boyut), new Vector2(0.5f, 0.5f), 100f);
    }

    void EfektiBaslat()
    {
        transform.SetAsLastSibling();

        if (aktifRutin != null)
        {
            StopCoroutine(aktifRutin);
        }

        aktifRutin = StartCoroutine(EfektRutini());
    }

    IEnumerator EfektRutini()
    {
        float gecenSure = 0f;

        while (gecenSure < EfektSuresi)
        {
            gecenSure += Time.deltaTime;
            float oran = Mathf.Clamp01(gecenSure / EfektSuresi);
            float alpha = oran < 0.18f
                ? Mathf.Lerp(0f, MaksAlpha, oran / 0.18f)
                : Mathf.Lerp(MaksAlpha, 0f, (oran - 0.18f) / 0.82f);

            grup.alpha = alpha;
            yield return null;
        }

        grup.alpha = 0f;
        aktifRutin = null;
    }
}
