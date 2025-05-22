using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Detection : MonoBehaviour
{
    public GameObject panel; // Arrastra el panel desde el Inspector
    public float delayBeforeLoad = 5f; // Tiempo de espera en segundos

    [Header("Configuración de Detección")]
    public bool puedeDetectarSombra = false; // Si está activo, detecta también el tag "Sombra"

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || (puedeDetectarSombra && other.CompareTag("Sombra")))
        {
            panel.SetActive(true); // Activa el panel
            StartCoroutine(LoadSceneWithDelay());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Puedes añadir lógica aquí si quieres que el panel se desactive al salir del trigger
            // panel.SetActive(false);
        }
    }

    IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene("GameOver");
    }
}