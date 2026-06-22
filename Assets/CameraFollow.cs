using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Kameranın takip edeceği hedef (Karakterimiz)
    public Transform hedef;

    // Kameranın hedefe yetişme hızı (Yumuşaklık ayarı)
    public float takipHizi = 5f;

    // Kamera takipleri her zaman Update yerine LateUpdate içinde yapılır ki titreme olmasın
    void LateUpdate()
    {
        // Hedef (Player) sahnede var olduğu sürece çalışır
        if (hedef != null)
        {
            // Kameranın gitmesi gereken yeni yer. 
            // DİKKAT: Z ekseni her zaman -10 kalmalı, yoksa kamera 2D zeminin içine girer ve ekran kör olur!
            Vector3 yeniPozisyon = new Vector3(hedef.position.x, hedef.position.y, -10f);

            // Kamerayı bulunduğu yerden yeni pozisyona yumuşakça (Lerp ile) kaydır
            transform.position = Vector3.Lerp(transform.position, yeniPozisyon, takipHizi * Time.deltaTime);
        }
    }
}