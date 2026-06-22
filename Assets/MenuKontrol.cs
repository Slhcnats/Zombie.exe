using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Slider ve UI elemanları için gerekli kütüphane

public class MenuKontrol : MonoBehaviour
{
    [Header("Ayarlar Arayüzü")]
    public GameObject ayarlarPaneli;
    public Slider sesAyarSlideri;

    void Start()
    {
        // Oyun başlarken ayarlar panelini gizle (Eğer editörde açık unuttuysan diye)
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false);

        // Slider'ın değerini oyunun o anki ses seviyesine eşitle
        if (sesAyarSlideri != null)
        {
            sesAyarSlideri.value = AudioListener.volume;
            // Kaydırma çubuğu hareket ettikçe SesiAyarla fonksiyonunu otomatik çalıştır
            sesAyarSlideri.onValueChanged.AddListener(SesiAyarla);
        }
    }

    public void OyunaBasla()
    {
        SceneManager.LoadScene(1);
    }

    // AYARLAR butonuna basıldığında bu çalışacak
    public void AyarlariAc()
    {
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(true);
    }

    // Ayarların içindeki KAPAT butonuna basıldığında bu çalışacak
    public void AyarlariKapat()
    {
        if (ayarlarPaneli != null) ayarlarPaneli.SetActive(false);
    }

    // Slider'ı kaydırdıkça çalışan fonksiyon
    public void SesiAyarla(float deger)
    {
        // Unity'nin Ana Ses Düğmesidir (AudioListener). 0 (Sessiz) ile 1 (Maksimum) arası değer alır.
        AudioListener.volume = deger;
    }

    public void OyundanCik()
    {
        Application.Quit();
    }
}