using UnityEngine;
using TMPro;
using System.Collections;

public class ConsoleToTMP : MonoBehaviour
{
    public TextMeshProUGUI logText;
    public float typeDelay = 0.05f; // Tiempo entre letras

    private Coroutine clearCoroutine;
    private Coroutine typewriterCoroutine;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            // Detener cualquier animación anterior
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
            if (clearCoroutine != null)
                StopCoroutine(clearCoroutine);

            // Iniciar animación de escritura
            typewriterCoroutine = StartCoroutine(TypeText(logString));

            // Borrar después de 15 segundos
            clearCoroutine = StartCoroutine(ClearAfterDelay(15f));
        }
    }

    IEnumerator TypeText(string message)
    {
        logText.text = "";
        foreach (char c in message)
        {
            logText.text += c;
            yield return new WaitForSeconds(typeDelay);
        }
        typewriterCoroutine = null;
    }

    IEnumerator ClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        logText.text = "";
        clearCoroutine = null;
    }
}
