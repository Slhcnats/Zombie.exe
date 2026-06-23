using UnityEngine;
using UnityEngine.SceneManagement;

public class KalinSes : MonoBehaviour
{
    private static KalinSes instance;

    [Range(0f, 1f)] public float menuSesSeviyesi = 0.5f;
    [Range(0f, 1f)] public float oyunSesSeviyesi = 0.25f;
    public float gecisHizi = 1.5f;

    private AudioSource sesKaynak;
    private float hedefSesSeviyesi;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        sesKaynak = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SahneYuklendi;
        SesSeviyesiniAyarla(SceneManager.GetActiveScene());
    }

    void Update()
    {
        if (sesKaynak == null) return;

        sesKaynak.volume = Mathf.MoveTowards(
            sesKaynak.volume,
            hedefSesSeviyesi,
            gecisHizi * Time.unscaledDeltaTime);
    }

    void SahneYuklendi(Scene sahne, LoadSceneMode mod)
    {
        SesSeviyesiniAyarla(sahne);
    }

    void SesSeviyesiniAyarla(Scene sahne)
    {
        hedefSesSeviyesi = sahne.buildIndex == 0 ? menuSesSeviyesi : oyunSesSeviyesi;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= SahneYuklendi;
            instance = null;
        }
    }
}
