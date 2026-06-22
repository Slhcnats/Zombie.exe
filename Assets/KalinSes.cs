using UnityEngine;

public class KalinSes : MonoBehaviour
{
    private static KalinSes instance;

    void Awake()
    {
        // Sahne değiştiğinde objenin yok olmasını engeller, böylece rüzgar hiç kesilmez!
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}