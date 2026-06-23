using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkSecimSistemi : MonoBehaviour
{
    private static PerkSecimSistemi instance;

    private GameObject panel;
    private Action secimTamamlandi;
    private TextMeshProUGUI silahPerkiYazisi;

    public static void Goster(Action tamamlandiginda)
    {
        if (instance == null)
        {
            GameObject sistemObjesi = new GameObject("PerkSecimSistemi");
            instance = sistemObjesi.AddComponent<PerkSecimSistemi>();
        }

        instance.SecimiAc(tamamlandiginda);
    }

    void Awake()
    {
        instance = this;
        ArayuzuOlustur();
    }

    void ArayuzuOlustur()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObjesi = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystemObjesi.transform.SetParent(canvas.transform, false);
        }

        panel = new GameObject("PerkSecimPaneli", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        panel.transform.SetAsLastSibling();

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image arkaPlan = panel.GetComponent<Image>();
        arkaPlan.color = new Color(0.04f, 0.04f, 0.06f, 0.88f);
        arkaPlan.raycastTarget = true;

        TextMeshProUGUI baslik = YaziOlustur(panel.transform, "PERK SEC", 42f, TextAlignmentOptions.Center);
        RectTransform baslikRect = baslik.GetComponent<RectTransform>();
        baslikRect.anchorMin = new Vector2(0.5f, 0.5f);
        baslikRect.anchorMax = new Vector2(0.5f, 0.5f);
        baslikRect.pivot = new Vector2(0.5f, 0.5f);
        baslikRect.anchoredPosition = new Vector2(0f, 155f);
        baslikRect.sizeDelta = new Vector2(720f, 60f);
        baslik.color = new Color(1f, 0.84f, 0.2f);

        ButonOlustur(
            "HIZLI KOSMA",
            "+1 Hareket Hizi",
            new Vector2(-250f, 0f),
            new Color(0.15f, 0.62f, 0.42f),
            HizliKosmaSec);

        ButonOlustur(
            "MAXIMUM CAN",
            "+25 Maksimum Can",
            new Vector2(0f, 0f),
            new Color(0.72f, 0.2f, 0.25f),
            MaximumCanSec);

        silahPerkiYazisi = ButonOlustur(
            "BUYUK SARJOR",
            "+12 Mermi ve Seri Atis",
            new Vector2(250f, 0f),
            new Color(0.28f, 0.36f, 0.82f),
            SilahPerkiniSec);

        panel.SetActive(false);
    }

    TextMeshProUGUI ButonOlustur(string baslik, string aciklama, Vector2 konum, Color renk, UnityEngine.Events.UnityAction tiklama)
    {
        GameObject butonObjesi = new GameObject("Perk_" + baslik, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        butonObjesi.transform.SetParent(panel.transform, false);

        RectTransform rect = butonObjesi.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = konum;
        rect.sizeDelta = new Vector2(220f, 140f);

        Image image = butonObjesi.GetComponent<Image>();
        image.color = renk;

        Button buton = butonObjesi.GetComponent<Button>();
        ColorBlock renkler = buton.colors;
        renkler.normalColor = renk;
        renkler.highlightedColor = Color.Lerp(renk, Color.white, 0.24f);
        renkler.pressedColor = Color.Lerp(renk, Color.black, 0.25f);
        renkler.selectedColor = renkler.highlightedColor;
        buton.colors = renkler;
        buton.onClick.AddListener(tiklama);

        TextMeshProUGUI yazi = YaziOlustur(butonObjesi.transform, baslik + "\n<size=21>" + aciklama + "</size>", 26f, TextAlignmentOptions.Center);
        RectTransform yaziRect = yazi.GetComponent<RectTransform>();
        yaziRect.anchorMin = Vector2.zero;
        yaziRect.anchorMax = Vector2.one;
        yaziRect.offsetMin = new Vector2(12f, 12f);
        yaziRect.offsetMax = new Vector2(-12f, -12f);
        yazi.color = Color.white;
        return yazi;
    }

    TextMeshProUGUI YaziOlustur(Transform parent, string metin, float boyut, TextAlignmentOptions hizalama)
    {
        GameObject yaziObjesi = new GameObject("Yazi", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        yaziObjesi.transform.SetParent(parent, false);

        TextMeshProUGUI yazi = yaziObjesi.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ornekYazi = parent.GetComponentInParent<Canvas>().GetComponentInChildren<TextMeshProUGUI>();
        yazi.font = ornekYazi != null ? ornekYazi.font : TMP_Settings.defaultFontAsset;
        yazi.text = metin;
        yazi.fontSize = boyut;
        yazi.alignment = hizalama;
        yazi.textWrappingMode = TextWrappingModes.Normal;
        yazi.raycastTarget = false;
        return yazi;
    }

    void SecimiAc(Action tamamlandiginda)
    {
        if (panel == null) return;

        secimTamamlandi = tamamlandiginda;
        SilahPerkiGorseliniGuncelle();
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
        Time.timeScale = 0f;
    }

    void HizliKosmaSec()
    {
        GameObject oyuncu = GameObject.FindWithTag("Player");
        PlayerMovement hareket = oyuncu != null ? oyuncu.GetComponent<PlayerMovement>() : null;
        if (hareket != null) hareket.hareketHizi += 1f;
        SecimiBitir();
    }

    void MaximumCanSec()
    {
        GameObject oyuncu = GameObject.FindWithTag("Player");
        PlayerHealth can = oyuncu != null ? oyuncu.GetComponent<PlayerHealth>() : null;
        if (can != null) can.MaximumCaniArtir(25);
        SecimiBitir();
    }

    void SilahPerkiniSec()
    {
        GameObject oyuncu = GameObject.FindWithTag("Player");
        PlayerShooting silah = oyuncu != null ? oyuncu.GetComponent<PlayerShooting>() : null;
        WaveManager dalgaYonetici = FindAnyObjectByType<WaveManager>();
        bool sonrakiHaritaJapon = dalgaYonetici != null && dalgaYonetici.SonrakiHaritaJaponSehrinde;

        if (silah != null)
        {
            if (sonrakiHaritaJapon) silah.LazerGucunuArtir();
            else silah.BuyukSarjorVeSeriAtisVer(12);
        }
        SecimiBitir();
    }

    void SilahPerkiGorseliniGuncelle()
    {
        if (silahPerkiYazisi == null) return;

        WaveManager dalgaYonetici = FindAnyObjectByType<WaveManager>();
        bool sonrakiHaritaJapon = dalgaYonetici != null && dalgaYonetici.SonrakiHaritaJaponSehrinde;
        silahPerkiYazisi.text = sonrakiHaritaJapon
            ? "LAZER GUCU\n<size=21>+1 Lazer Hasari</size>"
            : "BUYUK SARJOR\n<size=21>+12 Mermi ve Seri Atis</size>";
    }

    void SecimiBitir()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;

        Action devam = secimTamamlandi;
        secimTamamlandi = null;
        devam?.Invoke();
    }
}
