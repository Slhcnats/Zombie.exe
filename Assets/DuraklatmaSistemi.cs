using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DuraklatmaSistemi : MonoBehaviour
{
    private GameObject panel;

    void Start()
    {
        ArayuzuOlustur();
    }

    void Update()
    {
        if (GameManager.instance == null || GameManager.instance.OyunBittiMi) return;
        if (!panel.activeSelf && Time.timeScale == 0f) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panel.activeSelf)
            {
                DevamEt();
            }
            else
            {
                Duraklat();
            }
        }
    }

    void ArayuzuOlustur()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        if (FindAnyObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        panel = new GameObject("DuraklatmaPaneli", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        panel.transform.SetAsLastSibling();

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image arkaPlan = panel.GetComponent<Image>();
        arkaPlan.color = new Color(0f, 0f, 0f, 0.78f);
        arkaPlan.raycastTarget = true;

        TextMeshProUGUI baslik = YaziOlustur(panel.transform, "DURAKLATILDI", 44f);
        RectTransform baslikRect = baslik.GetComponent<RectTransform>();
        baslikRect.anchorMin = new Vector2(0.5f, 0.5f);
        baslikRect.anchorMax = new Vector2(0.5f, 0.5f);
        baslikRect.pivot = new Vector2(0.5f, 0.5f);
        baslikRect.anchoredPosition = new Vector2(0f, 100f);
        baslikRect.sizeDelta = new Vector2(500f, 60f);
        baslik.color = new Color(1f, 0.82f, 0.15f);

        ButonOlustur("DEVAM ET", new Vector2(0f, 20f), new Color(0.16f, 0.58f, 0.34f), DevamEt);
        ButonOlustur("ANA MENU", new Vector2(0f, -65f), new Color(0.62f, 0.12f, 0.16f), AnaMenuyeDon);

        panel.SetActive(false);
    }

    void ButonOlustur(string metin, Vector2 konum, Color renk, UnityEngine.Events.UnityAction tiklama)
    {
        GameObject butonObjesi = new GameObject(metin, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        butonObjesi.transform.SetParent(panel.transform, false);

        RectTransform rect = butonObjesi.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = konum;
        rect.sizeDelta = new Vector2(250f, 58f);

        Image image = butonObjesi.GetComponent<Image>();
        image.color = renk;

        Button buton = butonObjesi.GetComponent<Button>();
        ColorBlock renkler = buton.colors;
        renkler.normalColor = renk;
        renkler.highlightedColor = Color.Lerp(renk, Color.white, 0.22f);
        renkler.pressedColor = Color.Lerp(renk, Color.black, 0.25f);
        renkler.selectedColor = renkler.highlightedColor;
        buton.colors = renkler;
        buton.onClick.AddListener(tiklama);

        TextMeshProUGUI yazi = YaziOlustur(butonObjesi.transform, metin, 28f);
        RectTransform yaziRect = yazi.GetComponent<RectTransform>();
        yaziRect.anchorMin = Vector2.zero;
        yaziRect.anchorMax = Vector2.one;
        yaziRect.offsetMin = Vector2.zero;
        yaziRect.offsetMax = Vector2.zero;
        yazi.color = Color.white;
    }

    TextMeshProUGUI YaziOlustur(Transform parent, string metin, float boyut)
    {
        GameObject yaziObjesi = new GameObject("Yazi", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        yaziObjesi.transform.SetParent(parent, false);

        TextMeshProUGUI yazi = yaziObjesi.GetComponent<TextMeshProUGUI>();
        Canvas canvas = parent.GetComponentInParent<Canvas>();
        TextMeshProUGUI ornek = canvas != null ? canvas.GetComponentInChildren<TextMeshProUGUI>(true) : null;
        yazi.font = ornek != null && ornek.font != null ? ornek.font : TMP_Settings.defaultFontAsset;
        yazi.text = metin;
        yazi.fontSize = boyut;
        yazi.alignment = TextAlignmentOptions.Center;
        yazi.raycastTarget = false;
        return yazi;
    }

    void Duraklat()
    {
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
        Time.timeScale = 0f;
    }

    void DevamEt()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    void AnaMenuyeDon()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
