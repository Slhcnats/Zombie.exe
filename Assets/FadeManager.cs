using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [Header("UI Ayarları")]
    public Image bloodFadeImage;
    public float fadeSpeed = 1.5f;

    public void StartMapTransition()
    {
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        // 1. EKRAN KANLA KAPLANIR
        while (bloodFadeImage.color.a < 1)
        {
            Color c = bloodFadeImage.color;
            c.a += Time.deltaTime * fadeSpeed;
            bloodFadeImage.color = c;
            yield return null;
        }

        // --- EKRAN SİMSİYAHKEN HARİTAYI DEĞİŞTİR ---
        ChangeMapToGraveyard();

        yield return new WaitForSeconds(1.0f); // Harita yüklenmesi için kısa bekleme

        // 2. EKRAN TEMİZLENİR
        while (bloodFadeImage.color.a > 0)
        {
            Color c = bloodFadeImage.color;
            c.a -= Time.deltaTime * fadeSpeed;
            bloodFadeImage.color = c;
            yield return null;
        }
    }

    private void ChangeMapToGraveyard()
    {
        // WaveManager'ı bul ve haritayı değiştirmesini söyle (Unity 6 güncel kodu)
        WaveManager wm = FindAnyObjectByType<WaveManager>();
        if (wm != null)
        {
            wm.HaritayiDegistir();
            wm.isGraveyard = true; // Mezarlık aşamasını başlat
        }
    }
}