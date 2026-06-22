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

    private bool oyunBittiMi = false;

    void Awake()
    {
        if (instance == null) instance = this;
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

        int dakika = Mathf.FloorToInt(gecenSure / 60F);
        int saniye = Mathf.FloorToInt(gecenSure - dakika * 60);
        string formatliSure = string.Format("{0:00}:{1:00}", dakika, saniye);

        // --- KLASİK RETRO TEMASI UYGULANDI ---
        if (lesSayisiText != null) lesSayisiText.text = "LES SAYISI: " + zombiLesSayisi;
        if (waveText != null) waveText.text = "DALGA: " + atlatilanWave;
        if (sureText != null) sureText.text = "HAYATTA KALINAN SURE: " + formatliSure;

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

    public void OyunuYenidenBaslat()
    {
        if (hoparlor != null) hoparlor.Stop();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}