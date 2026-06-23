using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuKontrol : MonoBehaviour
{
    [Header("Ayarlar Arayuzu")]
    public GameObject ayarlarPaneli;
    public Slider sesAyarSlideri;
    public TMP_FontAsset baslikFontu;
    public Button dilButonu;
    public TextMeshProUGUI dilButonYazisi;

    private bool ingilizce;

    void Start()
    {
        ingilizce = PlayerPrefs.GetInt("DilIngilizce", 0) == 1;

        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false);

        if (sesAyarSlideri != null)
        {
            sesAyarSlideri.value = AudioListener.volume;
            sesAyarSlideri.onValueChanged.AddListener(SesiAyarla);
        }

        MenuBasligiOlustur();
        DilButonunuBagla();
        MetinleriGuncelle();
    }

    public void OyunaBasla()
    {
        SceneManager.LoadScene(1);
    }

    public void AyarlariAc()
    {
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(true);
    }

    public void AyarlariKapat()
    {
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false);
    }

    public void SesiAyarla(float deger)
    {
        AudioListener.volume = deger;
    }

    public void DiliDegistir()
    {
        ingilizce = !ingilizce;
        PlayerPrefs.SetInt("DilIngilizce", ingilizce ? 1 : 0);
        PlayerPrefs.Save();
        MetinleriGuncelle();
    }

    public void OyundanCik()
    {
        Application.Quit();
    }

    void DilButonunuBagla()
    {
        if (ayarlarPaneli == null) return;

        Transform butonTransform = ayarlarPaneli.transform.Find("DilButonu");
        if (dilButonu == null && butonTransform != null)
        {
            dilButonu = butonTransform.GetComponent<Button>();
        }

        if (dilButonYazisi == null && butonTransform != null)
        {
            dilButonYazisi = butonTransform.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (dilButonu != null)
        {
            dilButonu.onClick.RemoveListener(DiliDegistir);
            dilButonu.onClick.AddListener(DiliDegistir);
        }

        DilEtiketiniGoster();
    }

    void DilEtiketiniGoster()
    {
        if (dilButonYazisi == null) return;

        TextMeshProUGUI ornek = MenuFontuBul();
        dilButonYazisi.gameObject.SetActive(true);
        dilButonYazisi.enabled = true;
        dilButonYazisi.font = baslikFontu != null ? baslikFontu : (ornek != null ? ornek.font : TMP_Settings.defaultFontAsset);
        dilButonYazisi.fontSize = 28f;
        dilButonYazisi.alignment = TextAlignmentOptions.Center;
        dilButonYazisi.color = Color.white;
        dilButonYazisi.text = ingilizce ? "TR | [ENG]" : "[TR] | ENG";
    }

    void MenuBasligiOlustur()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null || canvas.transform.Find("AnaMenuBasligi") != null) return;

        GameObject baslikObjesi = new GameObject("AnaMenuBasligi", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        baslikObjesi.transform.SetParent(canvas.transform, false);
        baslikObjesi.transform.SetAsLastSibling();

        RectTransform rect = baslikObjesi.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -70f);
        rect.sizeDelta = new Vector2(1000f, 150f);

        TextMeshProUGUI baslik = baslikObjesi.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ornek = MenuFontuBul();
        baslik.font = baslikFontu != null ? baslikFontu : (ornek != null ? ornek.font : TMP_Settings.defaultFontAsset);
        baslik.text = "ZOMBIE.EXE";
        baslik.fontSize = 100f;
        baslik.fontStyle = FontStyles.Bold;
        baslik.characterSpacing = 1f;
        baslik.alignment = TextAlignmentOptions.Center;
        baslik.color = new Color(0.48f, 0.05f, 0.1f);
        baslik.raycastTarget = false;
    }

    TextMeshProUGUI MenuFontuBul()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return null;

        TextMeshProUGUI[] metinler = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI metin in metinler)
        {
            if (metin != dilButonYazisi && metin.font != null) return metin;
        }

        return null;
    }

    void MetinleriGuncelle()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        TextMeshProUGUI[] metinler = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI metin in metinler)
        {
            string mevcut = metin.text.Trim().ToUpperInvariant();

            if (mevcut == "OYNA" || mevcut == "PLAY")
                metin.text = ingilizce ? "PLAY" : "OYNA";
            else if (mevcut == "AYARLAR" || mevcut == "SETTINGS")
                metin.text = ingilizce ? "SETTINGS" : "AYARLAR";
            else if (mevcut == "KAPAT" || mevcut == "CLOSE")
                metin.text = ingilizce ? "CLOSE" : "KAPAT";
            else if (mevcut == "SES AYARI" || mevcut == "SOUND")
                metin.text = ingilizce ? "SOUND" : "SES AYARI";
        }

        if (dilButonYazisi != null)
        {
            DilEtiketiniGoster();
        }
    }
}
