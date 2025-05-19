using UnityEngine;

public class Detection : MonoBehaviour
{
    public GameObject panel; // Arrastra el panel desde el Inspector

    [Header("Configuración de Detección")]
    public bool puedeDetectarSombra = false; // Si está activo, detecta también el tag "Sombra"

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || (puedeDetectarSombra && other.CompareTag("Sombra")))
        {
            panel.SetActive(true); // Activa el panel
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
        }
    }
}
