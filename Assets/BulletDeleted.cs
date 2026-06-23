using UnityEngine;

public class BulletDeleted : MonoBehaviour
{
    [Header("Ses Ayarlari")]
    public AudioClip zombiOlumSesi;

    [Header("Efekt Ayarlari")]
    public Sprite kanLekesi;
    public float cesetSilinmeSuresi = 10f;
    [Range(0.1f, 1f)] public float kanBoyutu = 0.45f;
    [Range(1, 5)] public int kanLekesiSayisi = 1;
    [Range(1, 12)] public int kanSicramaParcacikSayisi = 5;
    [Min(1)] public int hasar = 1;

    void Start()
    {
        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter2D(Collider2D temasEdenObje)
    {
        if (temasEdenObje.CompareTag("Zombie"))
        {
            BossZombiDayaniklilik boss = temasEdenObje.GetComponent<BossZombiDayaniklilik>();
            if (boss != null && !boss.DarbeAl(hasar))
            {
                Destroy(gameObject);
                return;
            }

            if (boss == null)
            {
                RobotZombiDayaniklilik robot = temasEdenObje.GetComponent<RobotZombiDayaniklilik>();
                if (robot != null && !robot.DarbeAl(hasar))
                {
                    Destroy(gameObject);
                    return;
                }
            }

            GameManager yonetici = Object.FindAnyObjectByType<GameManager>();
            if (yonetici != null)
            {
                yonetici.SkorEkle(boss != null ? 100 : 10);
            }

            if (zombiOlumSesi != null)
            {
                AudioSource.PlayClipAtPoint(zombiOlumSesi, Camera.main.transform.position, 2.0f);
            }

            if (kanLekesi != null)
            {
                CesetOlustur(temasEdenObje);
            }

            Destroy(temasEdenObje.gameObject);
            Destroy(gameObject);
        }
    }

    void CesetOlustur(Collider2D zombiCollider)
    {
        SpriteRenderer zombiSprite = zombiCollider.GetComponent<SpriteRenderer>();
        if (zombiSprite == null || kanLekesi == null) return;

        GameObject oyuncu = GameObject.FindWithTag("Player");
        SpriteRenderer oyuncuSprite = oyuncu != null ? oyuncu.GetComponent<SpriteRenderer>() : null;
        int sortingLayerId = oyuncuSprite != null ? oyuncuSprite.sortingLayerID : zombiSprite.sortingLayerID;
        int cesetSortingOrder = (oyuncuSprite != null ? oyuncuSprite.sortingOrder : zombiSprite.sortingOrder) - 2;
        int kanSortingOrder = cesetSortingOrder + 1;

        float rastgeleDonus = Random.Range(0f, 360f);
        float rastgeleBoyut = Random.Range(1.15f, 1.35f);

        GameObject ceset = new GameObject("CanliZombiKanliCeset");
        ceset.tag = "Ceset";
        ceset.transform.position = zombiCollider.transform.position;
        ceset.transform.rotation = Quaternion.Euler(0f, 0f, rastgeleDonus);
        Vector3 zombiOlcegi = zombiCollider.transform.lossyScale;
        ceset.transform.localScale = new Vector3(
            Mathf.Abs(zombiOlcegi.x) * rastgeleBoyut,
            Mathf.Abs(zombiOlcegi.y) * rastgeleBoyut,
            1f);

        SpriteRenderer govde = ceset.AddComponent<SpriteRenderer>();
        govde.sprite = zombiSprite.sprite;
        govde.flipX = zombiSprite.flipX;
        govde.color = Color.white;
        govde.sortingLayerID = sortingLayerId;
        govde.sortingOrder = cesetSortingOrder;

        for (int i = 0; i < kanLekesiSayisi; i++)
        {
            GameObject kan = new GameObject("Kan");
            kan.transform.SetParent(ceset.transform);
            kan.transform.localPosition = i == 0 ? Vector3.zero : (Vector3)(Random.insideUnitCircle * 0.25f);
            kan.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            kan.transform.localScale = Vector3.one * Random.Range(kanBoyutu * 0.55f, kanBoyutu);

            SpriteRenderer kanRender = kan.AddComponent<SpriteRenderer>();
            kanRender.sprite = kanLekesi;
            kanRender.color = Color.white;
            kanRender.sortingLayerID = sortingLayerId;
            kanRender.sortingOrder = kanSortingOrder;
        }

        KanSicramasiOlustur(zombiCollider.transform.position, sortingLayerId, kanSortingOrder);
        Destroy(ceset, cesetSilinmeSuresi);
    }

    void KanSicramasiOlustur(Vector3 konum, int sortingLayerId, int sortingOrder)
    {
        for (int i = 0; i < kanSicramaParcacikSayisi; i++)
        {
            GameObject damla = new GameObject("KanDamlasi");
            damla.transform.position = konum;
            damla.transform.localScale = Vector3.one * Random.Range(0.04f, 0.09f);
            damla.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            SpriteRenderer damlaRender = damla.AddComponent<SpriteRenderer>();
            damlaRender.sprite = kanLekesi;
            damlaRender.color = new Color(1f, 1f, 1f, 0.85f);
            damlaRender.sortingLayerID = sortingLayerId;
            damlaRender.sortingOrder = sortingOrder;

            KanSicramaEfekti efekt = damla.AddComponent<KanSicramaEfekti>();
            efekt.Baslat(
                Random.insideUnitCircle.normalized,
                Random.Range(0.25f, 0.6f),
                Random.Range(0.2f, 0.4f));
        }
    }
}
