using UnityEngine;

public class Detection : MonoBehaviour
{
    public GameObject panel; // Arrastra el panel desde el Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Verifica si el objeto que colisiona tiene el tag "Player"
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
