using UnityEngine;
using System.Collections;

public class KameraTitremesi : MonoBehaviour
{
    public static KameraTitremesi instance;

    private void Awake()
    {
        instance = this;
    }

    public void Titret(float sure, float siddet)
    {
        StartCoroutine(TitremeRoutine(sure, siddet));
    }

    private IEnumerator TitremeRoutine(float sure, float siddet)
    {
        // VURUCU NOKTA BURASI: Kameranın yerini oyunun başında değil, tam ateş ettiğin an kaydediyoruz!
        Vector3 orijinalPozisyon = transform.localPosition;
        float gecenZaman = 0.0f;

        while (gecenZaman < sure)
        {
            float x = Random.Range(-1f, 1f) * siddet;
            float y = Random.Range(-1f, 1f) * siddet;

            transform.localPosition = new Vector3(orijinalPozisyon.x + x, orijinalPozisyon.y + y, orijinalPozisyon.z);

            gecenZaman += Time.deltaTime;
            yield return null;
        }

        // Titreme bitince kamerayı ateş etmeye başladığın o düzgün noktaya geri koy
        transform.localPosition = orijinalPozisyon;
    }
}