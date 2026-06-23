using UnityEngine;

public class RobotZombiDayaniklilik : MonoBehaviour
{
    [Min(1)] public int gerekenDarbeSayisi = 3;

    private int alinanDarbeSayisi;

    public bool DarbeAl(int hasar)
    {
        alinanDarbeSayisi += hasar;

        ZombiHasarFlash flash = GetComponent<ZombiHasarFlash>();
        if (flash != null) flash.HasarEfectiOynat();

        return alinanDarbeSayisi >= gerekenDarbeSayisi;
    }
}
