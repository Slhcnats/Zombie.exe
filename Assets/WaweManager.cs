using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Ayarlari")]
    public int currentWave = 1;
    public int zombiesToSpawn = 5;
    public float spawnInterval = 2.0f;

    [Header("Zorluk Ayarlari (Zombi Akini)")]
    public float zorlukCarpani = 1.3f;
    public int ekstraZombi = 3;
    public float minSpawnAraligi = 0.2f;

    [Header("Zombi Hız Ayarları")]
    public float mevcutHizCarpani = 1.0f;
    public float herWaveHizArtisi = 0.1f;

    [Header("Gereksinimler")]
    public GameObject zombiePrefab;

    [Header("Zombi Görselleri (Assetler)")]
    [Tooltip("Hocaya göstereceğin 4 farklı zombi resmini buraya sürükle.")]
    public Sprite[] zombiGorselleri;

    [HideInInspector]
    public bool isGraveyard = false;

    public Transform[] spawnPoints;
    public Text waveText;

    [Header("Duyuru Sistemi")]
    public TextMeshProUGUI duyuruYazisi;
    public AudioSource hoparlor;
    public AudioClip levelUpSesi;

    [Header("Harita (Tilemap) Prefabları")]
    public GameObject[] haritalar;
    private int aktifHaritaIndeksi = 0;
    private GameObject mevcutHaritaObjesi;

    private bool isWavePreparing = false;

    void Start()
    {
        MevcutHaritayiYukle();
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        isWavePreparing = true;
        if (waveText != null) waveText.text = "WAVE: " + currentWave;

        if (duyuruYazisi != null)
        {
            duyuruYazisi.gameObject.SetActive(true);
            duyuruYazisi.text = "WAVE " + currentWave;

            if (hoparlor != null && levelUpSesi != null) hoparlor.PlayOneShot(levelUpSesi);
        }

        yield return new WaitForSeconds(2f);
        if (duyuruYazisi != null) duyuruYazisi.gameObject.SetActive(false);

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnInterval);
        }
        isWavePreparing = false;
    }

    void SpawnZombie()
    {
        if (spawnPoints.Length > 0 && zombiePrefab != null)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            GameObject newZombie = Instantiate(zombiePrefab, spawnPoints[randomIndex].position, Quaternion.identity);

            // ZOMBİ HIZLANDIRMA KISMI
            ZombieAI zombiScript = newZombie.GetComponent<ZombieAI>();
            if (zombiScript != null)
            {
                zombiScript.hiz = zombiScript.hiz * mevcutHizCarpani;
            }

            // --- GÖRSEL DEĞİŞTİRME KISMI (HER DALGADA FARKLI ASSET) ---
            SpriteRenderer sr = newZombie.GetComponent<SpriteRenderer>();
            if (sr != null && zombiGorselleri.Length > 0)
            {
                int gorselIndeksi = (currentWave - 1) % zombiGorselleri.Length;

                if (zombiGorselleri[gorselIndeksi] != null)
                {
                    sr.sprite = zombiGorselleri[gorselIndeksi];
                    sr.color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
    }

    void Update()
    {
        GameObject[] remainingZombies = GameObject.FindGameObjectsWithTag("Zombie");
        if (remainingZombies.Length == 0 && !isWavePreparing)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        isWavePreparing = true;
        currentWave++;

        // --- YENİ EKLENEN KISIM: GameManager'a yeni wave bilgisini gönder ---
        if (GameManager.instance != null)
        {
            GameManager.instance.WaveAtlandi(currentWave);
        }

        // ZOMBİ AKINI VE HIZ MANTIĞI:
        zombiesToSpawn = Mathf.RoundToInt(zombiesToSpawn * zorlukCarpani) + ekstraZombi;
        spawnInterval = Mathf.Max(minSpawnAraligi, spawnInterval * 0.75f);
        mevcutHizCarpani += herWaveHizArtisi;

        FindAnyObjectByType<FadeManager>().StartMapTransition();
        OyuncuyuOdullendir();
    }

    public void HaritayiDegistir()
    {
        EskiCesetleriTemizle();

        if (mevcutHaritaObjesi != null) Destroy(mevcutHaritaObjesi);

        aktifHaritaIndeksi++;
        if (aktifHaritaIndeksi >= haritalar.Length) aktifHaritaIndeksi = 0;

        MevcutHaritayiYukle();
        StartCoroutine(StartWave());
    }

    public void EskiCesetleriTemizle()
    {
        GameObject[] tumCesetler = GameObject.FindGameObjectsWithTag("Ceset");

        foreach (GameObject ceset in tumCesetler)
        {
            Destroy(ceset);
        }
    }

    void MevcutHaritayiYukle()
    {
        if (haritalar != null && haritalar.Length > 0 && haritalar[aktifHaritaIndeksi] != null)
        {
            mevcutHaritaObjesi = Instantiate(haritalar[aktifHaritaIndeksi]);
        }
    }

    void OyuncuyuOdullendir()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.CaniFulle(100);
        }
    }
}