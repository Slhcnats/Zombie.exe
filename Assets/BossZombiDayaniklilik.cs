using UnityEngine;

public class BossZombiDayaniklilik : MonoBehaviour
{
    [Min(1)] public int gerekenDarbeSayisi = 15;

    private int alinanDarbeSayisi;

    public bool DarbeAl(int hasar)
    {
        alinanDarbeSayisi += hasar;

        ZombiHasarFlash flash = GetComponent<ZombiHasarFlash>();
        if (flash != null) flash.HasarEfectiOynat();

        return alinanDarbeSayisi >= gerekenDarbeSayisi;
    }
}
