using System.Collections;
using UnityEngine;

public class Sombra : MonoBehaviour
{
    public GameObject playerModel; // Modelo del jugador
    public ParticleSystem vanishEffect; // Sistema de partículas
    public float vanishTime = 7f; // Tiempo oculto
    public float fadeSpeed = 2f; // Velocidad del desvanecimiento
    public float sinkDistance = 2f; // Distancia que se hundirá
    public float sinkSpeed = 2f; // Velocidad de hundimiento

    private Renderer[] renderers;
    private bool isVanishing = false;
    private Vector3 originalPosition;

    void Start()
    {
        renderers = playerModel.GetComponentsInChildren<Renderer>();
        originalPosition = playerModel.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isVanishing)
        {
            StartCoroutine(Vanish());
        }
    }

    IEnumerator Vanish()
    {
        isVanishing = true;

        // Iniciar partículas dentro del personaje
        if (vanishEffect != null)
        {
            vanishEffect.transform.position = playerModel.transform.position;
            vanishEffect.Play();
        }

        // Fade out y hundirse en el suelo
        yield return StartCoroutine(FadeOutAndSink());

        // Ocultar el modelo
        foreach (Renderer rend in renderers)
        {
            rend.enabled = false;
        }

        // Mover el sistema de partículas bajo tierra mientras está oculto
        float elapsedTime = 0f;
        Vector3 startPosition = vanishEffect.transform.position;
        Vector3 targetPosition = startPosition - new Vector3(0, sinkDistance, 0);

        while (elapsedTime < vanishTime)
        {
            vanishEffect.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / vanishTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(vanishTime);

        // Reactivar el modelo del jugador antes de resurgir
        foreach (Renderer rend in renderers)
        {
            rend.enabled = true;
        }

        // Resurgir con partículas
        StartCoroutine(RiseAndFadeIn());
    }

    IEnumerator FadeOutAndSink()
    {
        float alpha = 1f;
        Vector3 targetPosition = playerModel.transform.position - new Vector3(0, sinkDistance, 0);

        while (alpha > 0 || playerModel.transform.position.y > targetPosition.y)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetAlpha(alpha);

            // Mover jugador hacia abajo
            playerModel.transform.position = Vector3.MoveTowards(playerModel.transform.position, targetPosition, sinkSpeed * Time.deltaTime);

            // Mover partículas con el jugador
            if (vanishEffect != null)
                vanishEffect.transform.position = playerModel.transform.position;

            yield return null;
        }
    }

    IEnumerator RiseAndFadeIn()
    {
        float alpha = 0f;
        Vector3 targetPosition = originalPosition;

        while (alpha < 1 || playerModel.transform.position.y < targetPosition.y)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetAlpha(alpha);

            // Mover jugador hacia arriba
            playerModel.transform.position = Vector3.MoveTowards(playerModel.transform.position, targetPosition, sinkSpeed * Time.deltaTime);

            // Mover partículas con el jugador mientras emerge
            if (vanishEffect != null)
                vanishEffect.transform.position = playerModel.transform.position;

            yield return null;
        }

        // Detener partículas después de la reaparición
        if (vanishEffect != null)
        {
            vanishEffect.Stop();
        }

        isVanishing = false;
    }

    void SetAlpha(float alpha)
    {
        foreach (Renderer rend in renderers)
        {
            if (rend.material.HasProperty("_Color"))
            {
                Color color = rend.material.color;
                color.a = Mathf.Clamp01(alpha);
                rend.material.color = color;
            }
        }
    }
}
