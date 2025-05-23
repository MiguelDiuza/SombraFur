using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("PANELES DE INTRO")]
    public GameObject[] panels;
    private int currentPanel = 0;

    [Header("TEXTOS")]
    public TextMeshProUGUI[] textos;
    [TextArea] public string[] textosCompletos;
    public float velocidadEscritura = 0.03f;

    [Header("TRANSICIÓN")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    public string nombreEscenaJuego = "Nivel1"; // Asegúrate de que este nombre coincida con tu escena.

    [Header("ANIMACIÓN DE IMAGEN")]
    [Tooltip("Factor de zoom final de la imagen (ej. 1.1 para un 10% más grande)")]
    public float zoomFactor = 1.1f;
    [Tooltip("Duración de la animación de zoom en segundos")]
    public float zoomDuration = 5f; // Dura 5 segundos el zoom

    // Corrutinas activas
    private Coroutine animacionTextoActual;
    private Coroutine animacionZoomActual; // Para la nueva animación de zoom

    void Start()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeIn()); // Transición suave de entrada
        MostrarPanel(0);
    }

    void MostrarPanel(int index)
    {
        // Desactivar todos los paneles y activar solo el panel en el índice actual.
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }

        currentPanel = index; // Actualiza el índice del panel actual.

        // Cancelar animación de texto previa si aún está escribiendo
        if (animacionTextoActual != null)
            StopCoroutine(animacionTextoActual);

        // Cancelar animación de zoom previa si está activa
        if (animacionZoomActual != null)
            StopCoroutine(animacionZoomActual);

        // Obtener el RectTransform del panel actual para animar su escala.
        // ASUNCIÓN: El GameObject en 'panels[index]' es el que tiene la imagen de fondo
        // o es el RectTransform que deseas escalar (ej. un Panel UI).
        RectTransform currentPanelRectTransform = panels[index].GetComponent<RectTransform>();
        if (currentPanelRectTransform != null)
        {
            // Reiniciar la escala a la original antes de iniciar un nuevo zoom
            currentPanelRectTransform.localScale = Vector3.one;
            // Iniciar la nueva animación de zoom para la imagen de fondo del panel actual
            animacionZoomActual = StartCoroutine(AnimarZoomImagen(currentPanelRectTransform));
        }
        else
        {
            Debug.LogWarning("El panel en el índice " + index + " no tiene un RectTransform o la imagen de fondo no es el GameObject raíz del panel. No se aplicará animación de zoom.");
        }

        // Iniciar animación del nuevo texto
        animacionTextoActual = StartCoroutine(AnimarTexto(textos[index], textosCompletos[index]));
    }

    IEnumerator AnimarTexto(TextMeshProUGUI textoTMP, string texto)
    {
        textoTMP.text = "";
        foreach (char c in texto)
        {
            textoTMP.text += c;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    /// <summary>
    /// Anima la escala de un RectTransform para crear un efecto de zoom suave.
    /// </summary>
    IEnumerator AnimarZoomImagen(RectTransform imageRectTransform)
    {
        Vector3 initialScale = Vector3.one; // Escala inicial (1,1,1)
        Vector3 targetScale = Vector3.one * zoomFactor; // Escala final basada en el zoomFactor

        // Bucle para interpolar la escala a lo largo de la duración
        for (float t = 0; t < zoomDuration; t += Time.deltaTime)
        {
            // Interpola linealmente la escala desde la inicial hasta la final
            imageRectTransform.localScale = Vector3.Lerp(initialScale, targetScale, t / zoomDuration);
            yield return null; // Espera al siguiente frame
        }

        // Asegurarse de que la escala final sea exactamente la deseada para evitar imprecisiones
        imageRectTransform.localScale = targetScale;
    }

    public void SiguientePanel()
    {
        int siguiente = currentPanel + 1;

        if (siguiente < panels.Length)
        {
            MostrarPanel(siguiente);
        }
        else
        {
            Debug.Log("Último panel alcanzado. Iniciando juego...");
            IniciarJuego();
        }
    }

    public void IniciarJuego()
    {
        StartCoroutine(TransicionYCarga(nombreEscenaJuego));
    }

    IEnumerator TransicionYCarga(string nombreEscena)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(nombreEscena);
    }

    IEnumerator FadeIn()
    {
        Color c = fadeImage.color;
        c.a = 1;
        fadeImage.color = c;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(1, 0, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0;
        fadeImage.color = c;
        fadeImage.gameObject.SetActive(false);
    }

    IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        Color c = fadeImage.color;
        c.a = 0;
        fadeImage.color = c;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1;
        fadeImage.color = c;
    }
}