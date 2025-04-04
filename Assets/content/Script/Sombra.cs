using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Sombra : MonoBehaviour
{
    public SkinnedMeshRenderer playerRenderer; // Renderer del personaje
    public ParticleSystem sombraEffectPrefab; // Prefab del sistema de partículas
    public float fadeDuration = 1f; // Tiempo de transición
    private bool isShadowMode = false;
    private Material[] originalMaterials; // Guardamos los materiales originales
    private ParticleSystem activeParticles; // Partículas activas

    // Referencia al Animator del personaje
    public Animator animator;
    private const string SombraParameterName = "sombraP"; // Nombre del parámetro en el Animator

    // Referencia al Post-process Volume en el Empty "Dark"
    public PostProcessVolume darkPostProcessVolume;
    public float postProcessFadeDuration = 0.5f; // Tiempo para la transición del efecto de la cámara

    void Start()
    {
        // Guardamos una copia de los materiales originales para restaurarlos después
        originalMaterials = playerRenderer.materials;

        // Asegurarse de que la referencia al Animator esté asignada
        if (animator == null)
        {
            Debug.LogError("El Animator del personaje no ha sido asignado en el Inspector.");
        }

        // Asegurarse de que la referencia al PostProcessVolume esté asignada
        if (darkPostProcessVolume == null)
        {
            Debug.LogError("El PostProcessVolume 'Dark' no ha sido asignado en el Inspector.");
        }
        else
        {
            // Asegurarse de que el PostProcessVolume esté inicialmente desactivado
            darkPostProcessVolume.weight = 0f;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isShadowMode)
        {
            StartCoroutine(EnterShadowMode());
        }
        else if (Input.GetKeyDown(KeyCode.Q) && isShadowMode)
        {
            StartCoroutine(ExitShadowMode());
        }
    }

    IEnumerator EnterShadowMode()
    {
        isShadowMode = true;

        // Activar la animación de entrada a sombra
        if (animator != null)
        {
            animator.SetBool(SombraParameterName, true);
        }

        // Instanciar partículas en la posición del personaje
        activeParticles = Instantiate(sombraEffectPrefab, transform.position, Quaternion.identity);
        activeParticles.transform.SetParent(transform); // Asegurar que se mueva con el personaje
        activeParticles.Play();

        // Activar el efecto de la cámara
        if (darkPostProcessVolume != null)
        {
            yield return StartCoroutine(FadePostProcessWeight(0f, 1f));
        }

        // Crear un clon de los materiales para modificar su opacidad sin afectar el original
        Material[] shadowMaterials = new Material[originalMaterials.Length];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            shadowMaterials[i] = new Material(originalMaterials[i]); // Clonar material
            shadowMaterials[i].SetFloat("_Mode", 2); // Configurar en modo Fade
            shadowMaterials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            shadowMaterials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            shadowMaterials[i].SetInt("_ZWrite", 0);
            shadowMaterials[i].DisableKeyword("_ALPHATEST_ON");
            shadowMaterials[i].EnableKeyword("_ALPHABLEND_ON");
            shadowMaterials[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
            shadowMaterials[i].renderQueue = 3000;
        }

        // Aplicar materiales modificados
        playerRenderer.materials = shadowMaterials;

        // Reducir opacidad del personaje
        yield return StartCoroutine(FadeCharacter(0f));

        // Esperar 5 segundos en modo sombra
        yield return new WaitForSeconds(5f);

        // Iniciar la salida del modo sombra automáticamente después de 5 segundos
        StartCoroutine(ExitShadowMode());
    }

    IEnumerator ExitShadowMode()
    {
        isShadowMode = false;

        // Desactivar la animación de sombra
        if (animator != null)
        {
            animator.SetBool(SombraParameterName, false);
        }

        // Detener y destruir partículas
        if (activeParticles)
        {
            activeParticles.Stop();
            Destroy(activeParticles.gameObject, 1f); // Se destruye tras 1 segundo para que termine el efecto
            activeParticles = null; // Limpiar la referencia
        }

        // Restaurar visibilidad del personaje
        yield return StartCoroutine(FadeCharacter(1f));

        // Restaurar los materiales originales
        playerRenderer.materials = originalMaterials;

        // Desactivar el efecto de la cámara
        if (darkPostProcessVolume != null)
        {
            yield return StartCoroutine(FadePostProcessWeight(1f, 0f));
        }
    }

    IEnumerator FadeCharacter(float targetAlpha)
    {
        float elapsedTime = 0f;
        float startAlpha = playerRenderer.materials[0].color.a;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);

            foreach (Material mat in playerRenderer.materials)
            {
                Color newColor = mat.color;
                newColor.a = newAlpha;
                mat.color = newColor;
            }

            yield return null;
        }
    }

    IEnumerator FadePostProcessWeight(float startWeight, float endWeight)
    {
        float elapsedTime = 0f;
        float currentWeight = startWeight;

        while (elapsedTime < postProcessFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newWeight = Mathf.Lerp(startWeight, endWeight, elapsedTime / postProcessFadeDuration);

            if (darkPostProcessVolume != null)
            {
                darkPostProcessVolume.weight = newWeight;
            }

            yield return null;
        }

        // Asegurar que el peso final sea exactamente el objetivo
        if (darkPostProcessVolume != null)
        {
            darkPostProcessVolume.weight = endWeight;
        }
    }
}