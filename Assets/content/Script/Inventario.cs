using UnityEngine;
using TMPro;

public class Inventario : MonoBehaviour
{
    public TextMeshProUGUI textoBalas;
    public TextMeshProUGUI textoLatas;

    private int cantidadBalas = 0;
    private int cantidadLatas = 0;

    public void AgregarBalas(int cantidad)
    {
        cantidadBalas += cantidad;
        ActualizarUI();
    }

    public void AgregarLatas(int cantidad)
    {
        cantidadLatas += cantidad;
        ActualizarUI();
    }

    public bool UseBullet()
    {
        if (cantidadBalas > 0)
        {
            cantidadBalas--;
            ActualizarUI();
            return true;
        }
        else
        {
            Debug.Log("No hay balas suficientes.");
            return false;
        }
    }

    public int GetLatas()  // Nuevo método para obtener la cantidad de latas
    {
        return cantidadLatas;
    }

    private void ActualizarUI()
    {
        if (textoBalas != null)
            textoBalas.text = cantidadBalas.ToString("00");

        if (textoLatas != null)
            textoLatas.text = cantidadLatas.ToString("00");
    }
}
