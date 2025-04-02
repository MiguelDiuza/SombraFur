using System.Collections;
using UnityEngine;

public class Sombra : MonoBehaviour
{
    public SkinnedMeshRenderer playerRenderer; // Renderer del personaje
    public ParticleSystem sombraEffectPrefab; // Prefab del sistema de partículas
    public float fadeDuration = 1f; // Tiempo de transición
    private bool isShadowMode = false;
    private Material[] originalMaterials; // Guardamos los materiales originales
    private ParticleSystem activeParticles; // Partículas activas

    void Start()
    {
        // Guardamos una copia de los materiales originales para restaurarlos después
        originalMaterials = playerRenderer.materials;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isShadowMode)
        {
            StartCoroutine(EnterShadowMode());
        }
    }

    IEnumerator EnterShadowMode()
    {
        isShadowMode = true;

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

        // Instanciar partículas en la posición del personaje
        activeParticles = Instantiate(sombraEffectPrefab, transform.position, Quaternion.identity);
        activeParticles.transform.SetParent(transform); // Asegurar que se mueva con el personaje
        activeParticles.Play();

        // Esperar 5 segundos en modo sombra
        yield return new WaitForSeconds(5f);

        // Detener y destruir partículas
        if (activeParticles)
        {
            activeParticles.Stop();
            Destroy(activeParticles.gameObject, 1f); // Se destruye tras 1 segundo para que termine el efecto
        }

        // Restaurar visibilidad del personaje
        yield return StartCoroutine(FadeCharacter(1f));

        // Restaurar los materiales originales
        playerRenderer.materials = originalMaterials;

        isShadowMode = false;
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
}
