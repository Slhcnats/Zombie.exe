using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public GameObject mermiPrefab;
    public float mermiHizi = 15f;

    [Header("Lazer Ayarlari")]
    public GameObject lazerMermiPrefab;
    public AudioClip lazerAtesSesi;
    [Min(0.03f)] public float lazerAtisAraligi = 0.09f;

    [Header("Sarjor Ayarlari")]
    [Min(1)] public int sarjorKapasitesi = 12;
    [Min(0.05f)] public float reloadSuresi = 0.35f;
    public bool seriAtisAktif;
    [Min(0.05f)] public float atisAraligi = 0.12f;

    [Header("Ses Ayarlari")]
    public AudioSource hoparlor;
    public AudioClip atesSesi;
    public AudioClip bosTetikSesi;
    public AudioClip reloadSesi;

    [SerializeField] private int mevcutMermi;
    private bool sarjorDolduruluyor;
    private Animator anim;
    private TextMeshProUGUI mermiYazisi;
    private float sonrakiAtisZamani;
    private bool oncekiLazerModu;
    private WaveManager dalgaYonetici;
    private int lazerHasari = 1;

    public int MevcutMermi => mevcutMermi;
    public bool SarjorDolduruluyor => sarjorDolduruluyor;

    void Start()
    {
        anim = GetComponent<Animator>();
        mevcutMermi = sarjorKapasitesi;
        MermiGostergeOlustur();
        MermiYazisiniGuncelle();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        bool lazerModu = LazerModuAktif();
        if (lazerModu != oncekiLazerModu)
        {
            oncekiLazerModu = lazerModu;
            MermiYazisiniGuncelle();
        }

        if (!lazerModu && Input.GetKeyDown(KeyCode.R))
        {
            ReloadBaslat();
        }

        bool atesIstendi = lazerModu || seriAtisAktif ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        if (atesIstendi && (lazerModu || !sarjorDolduruluyor) && Time.time >= sonrakiAtisZamani)
        {
            if (lazerModu || mevcutMermi > 0)
            {
                AtesEt();
            }
            else
            {
                BosTetikSesiCal();
            }
        }
    }

    public void ReloadBaslat()
    {
        if (LazerModuAktif()) return;
        if (sarjorDolduruluyor || mevcutMermi >= sarjorKapasitesi) return;

        StartCoroutine(ReloadRutini());
    }

    public void BuyukSarjorVeSeriAtisVer(int sarjorArtisi)
    {
        sarjorKapasitesi += sarjorArtisi;
        mevcutMermi = sarjorKapasitesi;
        seriAtisAktif = true;
        atisAraligi = Mathf.Min(atisAraligi, 0.11f);
        MermiYazisiniGuncelle();
    }

    public void LazerGucunuArtir()
    {
        lazerHasari++;
    }

    IEnumerator ReloadRutini()
    {
        sarjorDolduruluyor = true;
        MermiYazisiniGuncelle();

        if (anim != null)
        {
            anim.SetTrigger("Reload");
        }

        if (hoparlor != null && reloadSesi != null)
        {
            hoparlor.PlayOneShot(reloadSesi);
        }

        yield return new WaitForSeconds(reloadSuresi);

        mevcutMermi = sarjorKapasitesi;
        sarjorDolduruluyor = false;
        MermiYazisiniGuncelle();
    }

    void AtesEt()
    {
        bool lazerModu = LazerModuAktif();
        GameObject atilacakMermi = lazerModu && lazerMermiPrefab != null ? lazerMermiPrefab : mermiPrefab;
        if (atilacakMermi == null || Camera.main == null) return;

        Vector3 farePozisyonu = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        farePozisyonu.z = 0f;

        Vector2 atesYonu = (farePozisyonu - transform.position).normalized;
        float aci = Mathf.Atan2(atesYonu.y, atesYonu.x) * Mathf.Rad2Deg;
        Quaternion mermiDonusu = Quaternion.Euler(0f, 0f, aci);

        GameObject yeniMermi = Instantiate(atilacakMermi, transform.position, mermiDonusu);
        BulletDeleted mermiHasari = yeniMermi.GetComponent<BulletDeleted>();
        if (mermiHasari != null && lazerModu) mermiHasari.hasar = lazerHasari;
        Rigidbody2D rb = yeniMermi.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = atesYonu * mermiHizi;
        }

        NamluParlamasiOlustur(atesYonu, atilacakMermi, lazerModu);
        if (!lazerModu)
        {
            mevcutMermi--;
            sonrakiAtisZamani = Time.time + (seriAtisAktif ? atisAraligi : 0.08f);
            MermiYazisiniGuncelle();

            if (mevcutMermi == 0)
            {
                ReloadBaslat();
            }
        }
        else
        {
            sonrakiAtisZamani = Time.time + lazerAtisAraligi;
        }

        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }

        AudioClip calinacakSes = lazerModu && lazerAtesSesi != null ? lazerAtesSesi : atesSesi;
        if (hoparlor != null && calinacakSes != null)
        {
            hoparlor.PlayOneShot(calinacakSes);
        }

        if (KameraTitremesi.instance != null)
        {
            KameraTitremesi.instance.Titret(0.1f, 0.05f);
        }
    }

    void BosTetikSesiCal()
    {
        if (hoparlor != null && bosTetikSesi != null)
        {
            hoparlor.PlayOneShot(bosTetikSesi);
        }
    }

    bool LazerModuAktif()
    {
        if (dalgaYonetici == null)
        {
            dalgaYonetici = Object.FindAnyObjectByType<WaveManager>();
        }
        return dalgaYonetici != null && dalgaYonetici.JaponSehrindeyiz;
    }

    void NamluParlamasiOlustur(Vector2 atesYonu, GameObject kullanilanMermi, bool lazerModu)
    {
        SpriteRenderer mermiSprite = kullanilanMermi.GetComponent<SpriteRenderer>();
        if (mermiSprite == null || mermiSprite.sprite == null) return;

        GameObject parlama = new GameObject("NamluParlamasi");
        parlama.transform.position = transform.position + (Vector3)(atesYonu * 0.65f);
        parlama.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(atesYonu.y, atesYonu.x) * Mathf.Rad2Deg);
        parlama.transform.localScale = lazerModu ? new Vector3(0.28f, 0.28f, 1f) : new Vector3(0.6f, 0.6f, 1f);

        SpriteRenderer parlamaRender = parlama.AddComponent<SpriteRenderer>();
        parlamaRender.sprite = mermiSprite.sprite;
        parlamaRender.color = lazerModu ? new Color(0.15f, 0.95f, 1f, 1f) : new Color(1f, 0.84f, 0.2f, 0.95f);

        SpriteRenderer oyuncuSprite = GetComponent<SpriteRenderer>();
        parlamaRender.sortingLayerID = oyuncuSprite != null ? oyuncuSprite.sortingLayerID : mermiSprite.sortingLayerID;
        parlamaRender.sortingOrder = (oyuncuSprite != null ? oyuncuSprite.sortingOrder : mermiSprite.sortingOrder) + 2;

        NamluParlamaEfekti efekt = parlama.AddComponent<NamluParlamaEfekti>();
        efekt.Baslat(0.07f);
    }

    void MermiGostergeOlustur()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        Transform mevcutGosterge = canvas.transform.Find("MermiGosterge");
        if (mevcutGosterge != null)
        {
            mermiYazisi = mevcutGosterge.GetComponent<TextMeshProUGUI>();
            return;
        }

        GameObject gosterge = new GameObject(
            "MermiGosterge",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        gosterge.transform.SetParent(canvas.transform, false);

        RectTransform rect = gosterge.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.one;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one;
        rect.anchoredPosition = new Vector2(-32f, -32f);
        rect.sizeDelta = new Vector2(130f, 44f);

        mermiYazisi = gosterge.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ornekYazi = canvas.GetComponentInChildren<TextMeshProUGUI>();
        mermiYazisi.font = ornekYazi != null ? ornekYazi.font : TMP_Settings.defaultFontAsset;
        mermiYazisi.fontSize = 26f;
        mermiYazisi.alignment = TextAlignmentOptions.MidlineRight;
        mermiYazisi.margin = new Vector4(0f, 0f, 26f, 0f);
        mermiYazisi.raycastTarget = false;

        MermiIkonuOlustur(gosterge.transform);
    }

    void MermiIkonuOlustur(Transform parent)
    {
        if (mermiPrefab == null) return;

        SpriteRenderer mermiSprite = mermiPrefab.GetComponent<SpriteRenderer>();
        if (mermiSprite == null || mermiSprite.sprite == null) return;

        GameObject ikon = new GameObject("MermiIkonu", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        ikon.transform.SetParent(parent, false);

        RectTransform rect = ikon.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0.5f);
        rect.anchorMax = new Vector2(1f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(-8f, 0f);
        rect.sizeDelta = new Vector2(26f, 12f);
        rect.localRotation = Quaternion.Euler(0f, 0f, 90f);

        Image mermiIkonu = ikon.GetComponent<Image>();
        mermiIkonu.sprite = mermiSprite.sprite;
        mermiIkonu.preserveAspect = true;
        mermiIkonu.raycastTarget = false;
    }

    void MermiYazisiniGuncelle()
    {
        if (mermiYazisi == null) return;

        if (LazerModuAktif())
        {
            mermiYazisi.text = "<color=#49DFFF>LASER</color>";
            mermiYazisi.color = Color.white;
            return;
        }

        if (sarjorDolduruluyor)
        {
            mermiYazisi.text = "RELOAD";
            mermiYazisi.color = new Color(1f, 0.78f, 0.2f);
            return;
        }

        if (mevcutMermi == 0)
        {
            mermiYazisi.text = "<color=#FFD400>0</color><color=#111111> / " + sarjorKapasitesi + "</color>";
            return;
        }

        mermiYazisi.text = "<color=#FFD400>" + mevcutMermi + "</color><color=#111111> / " + sarjorKapasitesi + "</color>";
        mermiYazisi.color = Color.white;
    }
}
