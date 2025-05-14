using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int canCount = 0;
    public int bulletCount = 10; // cantidad inicial, ajustable

    public void AddCan(int amount)
    {
        canCount += amount;
        Debug.Log("Lata recogida. Total: " + canCount);
    }

    public void UseCan()
    {
        if (canCount > 0)
        {
            canCount--;
            Debug.Log("Lata usada. Restantes: " + canCount);
            // Aquí podrías activar algún efecto de curación o animación
        }
        else
        {
            Debug.Log("No hay latas para usar.");
        }
    }

    public bool UseBullet()
    {
        if (bulletCount > 0)
        {
            bulletCount--;
            Debug.Log("Bala disparada. Restantes: " + bulletCount);
            return true;
        }
        else
        {
            Debug.Log("Sin balas.");
            return false;
        }
    }

    public void AddBullets(int amount)
    {
        bulletCount += amount;
        Debug.Log("Balas añadidas. Total: " + bulletCount);
    }
}
