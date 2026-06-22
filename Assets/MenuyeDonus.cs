using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçişi için şart

public class MenuyeDonus : MonoBehaviour
{
    public void AnaMenuyeGit()
    {
        // ÖNEMLİ: Eğer oyuncu öldüğünde oyunu durduruyorsan (Time.timeScale = 0 yapıyorsan), 
        // ana menüye dönmeden önce zamanı tekrar normale (1) çevirmeliyiz. Yoksa menü donar!
        Time.timeScale = 1f;

        // Build Profiles'taki 0 numaralı sahnemizi (MainMenu) yükler
        SceneManager.LoadScene(0);
    }
}