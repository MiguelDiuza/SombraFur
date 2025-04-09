using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscena : MonoBehaviour
{
    // Esta función será llamada desde el botón
    public void CambiarAEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
