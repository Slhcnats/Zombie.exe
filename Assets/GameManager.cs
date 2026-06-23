using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Skor Ayarları")]
    public int toplamSkor = 0;
    public Text pointTableEkrani;

    [Header("Panel Ayarları")]
    public GameObject gameOverPanel;
    public Sprite reklamButonGorseli;

    [Header("Ses Ayarları")]
    public AudioSource hoparlor;
    public AudioClip gameOverSesi;

    [Header("Oyun İstatistikleri")]
    public int zombiLesSayisi = 0;
    public int atlatilanWave = 1;
    public float gecenSure = 0f;

    [Header("Ölüm Ekranı Yazıları")]
    public TextMeshProUGUI lesSayisiText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI sureText;

    private const string EnYuksekSkorKey = "EnYuksekSkor";
    private const string EnYuksekDalgaKey = "EnYuksekDalga";
    private const string EnUzunSureKey = "EnUzunSure";

    private TextMeshProUGUI rekorText;
    private string lesEtiketi;
    private string dalgaEtiketi;
    private GameObject reklamPaneli;
    private TextMeshProUGUI reklamSayacText;
    private Button reklamDevamButonu;
    private PlayerHealth aktifOyuncu;
    private readonly string[] oyunHudNesneAdlari = { "PointTable", "HealthBar", "WaweText", "DuyuruYazisi", "MermiGosterge" };

    private bool oyunBittiMi = false;
    public bool OyunBittiMi => oyunBittiMi;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (GetComponent<DuraklatmaSistemi>() == null)
        {
            gameObject.AddComponent<DuraklatmaSistemi>();
        }

        RekorYazisiniHazirla();
        HayattaKalmaMetniniGizle();
        OyunSonuEtiketleriniHatirla();
        ReklamDevamButonunuHazirla();
    }

    void Update()
    {
        if (!oyunBittiMi)
        {
            gecenSure += Time.deltaTime;
        }
    }

    public void SkorEkle(int eklenecekPuan)
    {
        toplamSkor += eklenecekPuan;
        zombiLesSayisi++;

        if (pointTableEkrani != null)
        {
            pointTableEkrani.text = "PUAN: " + toplamSkor;
        }
    }

    public void WaveAtlandi(int yeniWave)
    {
        atlatilanWave = yeniWave;
    }

    public void OyunuBitir()
    {
        oyunBittiMi = true;

        int gecenSureSaniye = Mathf.FloorToInt(gecenSure);

        // --- KLASİK RETRO TEMASI UYGULANDI ---

        RekorlariGuncelle(gecenSureSaniye);
        OyunSonuDegerleriniGuncelle();
        OyunHudunuAyarla(false);
        ReklamDevamButonunuGoster();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (hoparlor != null && gameOverSesi != null)
        {
            hoparlor.PlayOneShot(gameOverSesi);
        }

        Time.timeScale = 0f;
    }

    public void OyuncuOldu(PlayerHealth oyuncu)
    {
        aktifOyuncu = oyuncu;
        aktifOyuncu.OlumAnindaDevreDisiBirak();
        OyunuBitir();
    }

    private void OyunSonuEtiketleriniHatirla()
    {
        if (gameOverPanel == null) return;

        // Sahne referanslari eksikse, kullanicinin panelde yazdigi iki etiketi bul.
        foreach (TextMeshProUGUI metin in gameOverPanel.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            string icerik = metin.text.ToUpperInvariant();
            if (waveText == null && icerik.StartsWith("DALGA")) waveText = metin;
            if (lesSayisiText == null && icerik.StartsWith("CESET")) lesSayisiText = metin;
        }

        lesEtiketi = EtiketMetniniTemizle(lesSayisiText);
        dalgaEtiketi = EtiketMetniniTemizle(waveText);
    }

    private string EtiketMetniniTemizle(TextMeshProUGUI etiket)
    {
        if (etiket == null) return string.Empty;

        etiket.textWrappingMode = TextWrappingModes.NoWrap;
        etiket.overflowMode = TextOverflowModes.Overflow;

        string metin = etiket.text;
        int renkBaslangici = metin.IndexOf("<color", System.StringComparison.OrdinalIgnoreCase);
        if (renkBaslangici >= 0) metin = metin.Substring(0, renkBaslangici);

        return metin.TrimEnd(' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
    }

    private void OyunSonuDegerleriniGuncelle()
    {
        if (string.IsNullOrEmpty(lesEtiketi) || string.IsNullOrEmpty(dalgaEtiketi))
        {
            OyunSonuEtiketleriniHatirla();
        }

        if (lesSayisiText != null) lesSayisiText.text = lesEtiketi + " <color=#FFFFFF>" + zombiLesSayisi + "</color>";
        if (waveText != null) waveText.text = dalgaEtiketi + " <color=#FFFFFF>" + atlatilanWave + "</color>";
    }

    private void HayattaKalmaMetniniGizle()
    {
        if (gameOverPanel == null) return;

        TextMeshProUGUI[] metinler = gameOverPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI metin in metinler)
        {
            if (metin.text.ToUpperInvariant().Contains("HAYATTA"))
            {
                metin.gameObject.SetActive(false);
            }
        }
    }

    private void ReklamDevamButonunuHazirla()
    {
        if (gameOverPanel == null) return;

        Transform mevcutButon = gameOverPanel.transform.Find("ReklamDevamButonu");
        if (mevcutButon != null)
        {
            reklamDevamButonu = mevcutButon.GetComponent<Button>();
            if (reklamDevamButonu != null)
            {
                reklamDevamButonu.onClick.RemoveListener(ReklamIzlemeyiBaslat);
                reklamDevamButonu.onClick.AddListener(ReklamIzlemeyiBaslat);
            }
            return;
        }

        GameObject bilgiObjesi = new GameObject("ReklamBilgiYazisi", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        bilgiObjesi.transform.SetParent(gameOverPanel.transform, false);
        TextMeshProUGUI bilgiYazisi = bilgiObjesi.GetComponent<TextMeshProUGUI>();
        bilgiYazisi.font = lesSayisiText != null ? lesSayisiText.font : TMP_Settings.defaultFontAsset;
        bilgiYazisi.text = "REKLAM IZLE";
        bilgiYazisi.fontSize = 20f;
        bilgiYazisi.alignment = TextAlignmentOptions.Center;
        bilgiYazisi.color = Color.white;
        RectTransform bilgiRect = bilgiYazisi.rectTransform;
        bilgiRect.anchorMin = new Vector2(0.5f, 0.5f);
        bilgiRect.anchorMax = new Vector2(0.5f, 0.5f);
        bilgiRect.pivot = new Vector2(0.5f, 0.5f);
        bilgiRect.anchoredPosition = new Vector2(0f, -58f);
        bilgiRect.sizeDelta = new Vector2(260f, 32f);

        GameObject butonObjesi = new GameObject("ReklamDevamButonu", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        butonObjesi.transform.SetParent(gameOverPanel.transform, false);
        RectTransform butonRect = butonObjesi.GetComponent<RectTransform>();
        butonRect.anchorMin = new Vector2(0.5f, 0.5f);
        butonRect.anchorMax = new Vector2(0.5f, 0.5f);
        butonRect.pivot = new Vector2(0.5f, 0.5f);
        butonRect.anchoredPosition = new Vector2(0f, -102f);
        butonRect.sizeDelta = new Vector2(48f, 48f);

        Image butonGorseli = butonObjesi.GetComponent<Image>();
        Color butonRengi = reklamButonGorseli != null ? Color.white : new Color(0.12f, 0.52f, 0.28f);
        butonGorseli.sprite = reklamButonGorseli;
        butonGorseli.preserveAspect = reklamButonGorseli != null;
        butonGorseli.color = butonRengi;
        reklamDevamButonu = butonObjesi.GetComponent<Button>();
        ColorBlock renkler = reklamDevamButonu.colors;
        renkler.normalColor = butonRengi;
        renkler.highlightedColor = Color.Lerp(butonRengi, Color.white, 0.22f);
        renkler.pressedColor = Color.Lerp(butonRengi, Color.black, 0.25f);
        reklamDevamButonu.colors = renkler;
        reklamDevamButonu.onClick.AddListener(ReklamIzlemeyiBaslat);

    }

    private void ReklamDevamButonunuGoster()
    {
        ReklamDevamButonunuHazirla();
        if (gameOverPanel == null) return;

        Transform bilgi = gameOverPanel.transform.Find("ReklamBilgiYazisi");
        if (bilgi != null) bilgi.gameObject.SetActive(true);

        Transform buton = gameOverPanel.transform.Find("ReklamDevamButonu");
        if (buton != null) buton.gameObject.SetActive(true);
    }

    private void ReklamIzlemeyiBaslat()
    {
        if (aktifOyuncu == null) return;
        ReklamPaneliniHazirla();
        if (reklamPaneli == null) return;

        gameOverPanel.SetActive(false);
        reklamPaneli.SetActive(true);
        reklamPaneli.transform.SetAsLastSibling();
        StartCoroutine(ReklamSayaci());
    }

    private void ReklamPaneliniHazirla()
    {
        if (reklamPaneli != null) return;

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        reklamPaneli = new GameObject("OdulluReklamPaneli", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        reklamPaneli.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = reklamPaneli.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        reklamPaneli.GetComponent<Image>().color = new Color(0.02f, 0.02f, 0.02f, 0.94f);

        TextMeshProUGUI baslik = ReklamYazisiOlustur("ODULLU REKLAM", 42f, new Vector2(0f, 120f));
        baslik.color = new Color(1f, 0.82f, 0.2f);

        reklamSayacText = ReklamYazisiOlustur("REKLAM IZLENIYOR... 3", 27f, new Vector2(0f, 55f));
        reklamSayacText.color = Color.white;

        reklamPaneli.SetActive(false);
    }

    private TextMeshProUGUI ReklamYazisiOlustur(string metin, float boyut, Vector2 konum)
    {
        GameObject yaziObjesi = new GameObject("Yazi", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        yaziObjesi.transform.SetParent(reklamPaneli.transform, false);

        TextMeshProUGUI yazi = yaziObjesi.GetComponent<TextMeshProUGUI>();
        yazi.font = lesSayisiText != null ? lesSayisiText.font : TMP_Settings.defaultFontAsset;
        yazi.text = metin;
        yazi.fontSize = boyut;
        yazi.alignment = TextAlignmentOptions.Center;
        yazi.color = Color.white;

        RectTransform rect = yazi.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = konum;
        rect.sizeDelta = new Vector2(850f, 60f);
        return yazi;
    }

    private IEnumerator ReklamSayaci()
    {
        for (int kalanSure = 3; kalanSure > 0; kalanSure--)
        {
            reklamSayacText.text = "REKLAM IZLENIYOR... " + kalanSure;
            yield return new WaitForSecondsRealtime(1f);
        }

        if (aktifOyuncu == null) yield break;

        reklamSayacText.text = "DEVAM EDILIYOR";
        reklamPaneli.SetActive(false);
        aktifOyuncu.HayataDondur();
        aktifOyuncu = null;
        oyunBittiMi = false;
        OyunHudunuAyarla(true);
        Time.timeScale = 1f;
    }

    private void OyunHudunuAyarla(bool gorunur)
    {
        if (gameOverPanel == null) return;

        Canvas canvas = gameOverPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        foreach (string hudAdi in oyunHudNesneAdlari)
        {
            Transform hudNesnesi = canvas.transform.Find(hudAdi);
            if (hudNesnesi != null) hudNesnesi.gameObject.SetActive(gorunur);
        }
    }

    private void RekorYazisiniHazirla()
    {
        if (gameOverPanel == null) return;

        Transform mevcutYazi = gameOverPanel.transform.Find("RekorYazisi");
        if (mevcutYazi != null)
        {
            rekorText = mevcutYazi.GetComponent<TextMeshProUGUI>();
            return;
        }

        GameObject yaziObjesi = new GameObject("RekorYazisi", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        yaziObjesi.transform.SetParent(gameOverPanel.transform, false);

        rekorText = yaziObjesi.GetComponent<TextMeshProUGUI>();
        RectTransform rectTransform = rekorText.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.anchoredPosition = new Vector2(0f, 36f);
        rectTransform.sizeDelta = new Vector2(850f, 44f);

        if (lesSayisiText != null) rekorText.font = lesSayisiText.font;
        else if (waveText != null) rekorText.font = waveText.font;

        rekorText.fontSize = 22f;
        rekorText.alignment = TextAlignmentOptions.Center;
        rekorText.color = new Color(1f, 0.82f, 0.2f);
        rekorText.textWrappingMode = TextWrappingModes.NoWrap;
    }

    private void RekorlariGuncelle(int gecenSureSaniye)
    {
        int enYuksekSkor = Mathf.Max(toplamSkor, PlayerPrefs.GetInt(EnYuksekSkorKey, 0));
        int enYuksekDalga = Mathf.Max(atlatilanWave, PlayerPrefs.GetInt(EnYuksekDalgaKey, 1));
        int enUzunSure = Mathf.Max(gecenSureSaniye, PlayerPrefs.GetInt(EnUzunSureKey, 0));

        PlayerPrefs.SetInt(EnYuksekSkorKey, enYuksekSkor);
        PlayerPrefs.SetInt(EnYuksekDalgaKey, enYuksekDalga);
        PlayerPrefs.SetInt(EnUzunSureKey, enUzunSure);
        PlayerPrefs.Save();

        if (rekorText == null) RekorYazisiniHazirla();
        if (rekorText != null)
        {
            rekorText.text = "REKOR | SKOR: " + enYuksekSkor + " | DALGA: " + enYuksekDalga + " | SURE: " + SaniyeyiFormatla(enUzunSure);
        }
    }

    private string SaniyeyiFormatla(int toplamSaniye)
    {
        int dakika = toplamSaniye / 60;
        int saniye = toplamSaniye % 60;
        return string.Format("{0:00}:{1:00}", dakika, saniye);
    }

    public void OyunuYenidenBaslat()
    {
        if (hoparlor != null) hoparlor.Stop();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AnaMenuyeDon()
    {
        if (hoparlor != null) hoparlor.Stop();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
