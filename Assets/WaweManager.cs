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
    public GameObject robotZombiePrefab;

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
    private bool bossDalgasiAktif;
    private bool bossDogdu;
    public bool JaponSehrindeyiz => aktifHaritaIndeksi == 1;
    public bool SonrakiHaritaJaponSehrinde => haritalar != null && haritalar.Length > 1 && (aktifHaritaIndeksi + 1) % haritalar.Length == 1;

    void Start()
    {
        MevcutHaritayiYukle();
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        isWavePreparing = true;
        bossDalgasiAktif = currentWave == 3 || currentWave == 4;
        bossDogdu = false;
        if (waveText != null) waveText.text = "DALGA: " + currentWave;

        if (duyuruYazisi != null)
        {
            duyuruYazisi.gameObject.SetActive(true);
            duyuruYazisi.text = "DALGA " + currentWave;

            if (hoparlor != null && levelUpSesi != null) hoparlor.PlayOneShot(levelUpSesi);
        }

        yield return new WaitForSeconds(2f);
        if (duyuruYazisi != null) duyuruYazisi.gameObject.SetActive(false);

        int spawnEdilecekZombiSayisi = bossDalgasiAktif ? 4 : zombiesToSpawn;
        for (int i = 0; i < spawnEdilecekZombiSayisi; i++)
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
            bool japonSehrindeyiz = aktifHaritaIndeksi == 1;
            bool robotDogacak = robotZombiePrefab != null && japonSehrindeyiz;
            GameObject dogacakPrefab = robotDogacak ? robotZombiePrefab : zombiePrefab;
            GameObject newZombie = Instantiate(dogacakPrefab, spawnPoints[randomIndex].position, Quaternion.identity);

            // ZOMBİ HIZLANDIRMA KISMI
            ZombieAI zombiScript = newZombie.GetComponent<ZombieAI>();
            if (zombiScript != null)
            {
                zombiScript.hiz = zombiScript.hiz * mevcutHizCarpani;
                if (robotDogacak && japonSehrindeyiz)
                {
                    zombiScript.hiz *= 0.7f;
                }
            }

            // --- GÖRSEL DEĞİŞTİRME KISMI (HER DALGADA FARKLI ASSET) ---
            SpriteRenderer sr = newZombie.GetComponent<SpriteRenderer>();
            if (!robotDogacak && sr != null && zombiGorselleri.Length > 0)
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

    IEnumerator BossuDogur()
    {
        isWavePreparing = true;

        if (duyuruYazisi != null)
        {
            duyuruYazisi.gameObject.SetActive(true);
            duyuruYazisi.text = "BOSS GELIYOR";
            if (hoparlor != null && levelUpSesi != null) hoparlor.PlayOneShot(levelUpSesi);
        }

        yield return new WaitForSeconds(1.5f);
        if (duyuruYazisi != null) duyuruYazisi.gameObject.SetActive(false);

        GameObject bossPrefab = JaponSehrindeyiz ? robotZombiePrefab : zombiePrefab;
        if (bossPrefab != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            GameObject boss = Instantiate(bossPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
            boss.name = JaponSehrindeyiz ? "RobotBoss" : "MezarlikBoss";
            boss.transform.localScale *= 6f;

            ZombieAI zombiScript = boss.GetComponent<ZombieAI>();
            if (zombiScript != null)
            {
                zombiScript.hiz = zombiScript.hiz * mevcutHizCarpani * 0.42f;
            }

            RobotZombiDayaniklilik robotDayaniklilik = boss.GetComponent<RobotZombiDayaniklilik>();
            if (robotDayaniklilik != null) robotDayaniklilik.enabled = false;

            BossZombiDayaniklilik bossDayaniklilik = boss.AddComponent<BossZombiDayaniklilik>();
            bossDayaniklilik.gerekenDarbeSayisi = JaponSehrindeyiz ? 40 : 20;
        }

        isWavePreparing = false;
    }

    void Update()
    {
        GameObject[] remainingZombies = GameObject.FindGameObjectsWithTag("Zombie");
        if (remainingZombies.Length == 0 && !isWavePreparing)
        {
            if (bossDalgasiAktif && !bossDogdu)
            {
                bossDogdu = true;
                StartCoroutine(BossuDogur());
            }
            else
            {
                NextWave();
            }
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

        OyuncuyuOdullendir();
        PerkSecimSistemi.Goster(HaritaGecisiniBaslat);
    }

    void HaritaGecisiniBaslat()
    {
        FadeManager fadeManager = FindAnyObjectByType<FadeManager>();
        if (fadeManager != null)
        {
            fadeManager.StartMapTransition();
        }
        else
        {
            HaritayiDegistir();
        }
    }

    public void HaritayiDegistir()
    {
        EskiCesetleriTemizle();

        if (mevcutHaritaObjesi != null) Destroy(mevcutHaritaObjesi);

        aktifHaritaIndeksi++;
        if (aktifHaritaIndeksi >= haritalar.Length) aktifHaritaIndeksi = 0;

        MevcutHaritayiYukle();
        OyuncuyuHaritaMerkezineTasi();
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

    void OyuncuyuHaritaMerkezineTasi()
    {
        if (mevcutHaritaObjesi == null) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector3 merkez = HaritaMerkeziniBul();
        player.transform.position = new Vector3(merkez.x, merkez.y, player.transform.position.z);
    }

    Vector3 HaritaMerkeziniBul()
    {
        Renderer[] renderers = mevcutHaritaObjesi.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return mevcutHaritaObjesi.transform.position;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds.center;
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
