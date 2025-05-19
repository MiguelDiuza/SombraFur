using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI; // Agrega esta línea para acceder a la clase Image
using TMPro; // Agrega esta línea si vas a usar TextMeshPro

public class Sombra : MonoBehaviour
{
    public SkinnedMeshRenderer playerRenderer; // Renderer del personaje
    public ParticleSystem sombraEffectPrefab; // Prefab del sistema de partículas
    public float fadeDuration = 1f; // Tiempo de transición
    private bool isShadowMode = false;
    private Material[] originalMaterials; // Guardamos los materiales originales
    private ParticleSystem activeParticles; // Partículas activas


    private bool puedeEntrarEnSombra = true; // NUEVO: Bandera para controlar el cooldown
    public float cooldownDuracion = 5f; // Duración del cooldown en segundos


    // Referencia al Animator del personaje
    public Animator animator;
    private const string SombraParameterName = "sombraP"; // Nombre del parámetro en el Animator

    // Referencia al Post-process Volume en el Empty "Dark"
    public PostProcessVolume darkPostProcessVolume;
    public float postProcessFadeDuration = 0.5f; // Tiempo para la transición del efecto de la cámara

    // Nuevo: Referencias a la barra de duración del modo sombra
    public Image barraDuracionSombraUI; // Asigna este objeto en el Inspector
    public TMP_Text contadorDuracionSombraTMP; // Asigna este objeto en el Inspector (opcional)

    // Nuevo: Referencias a la barra de cooldown
    public Image barraCooldownUI; // Asigna este objeto en el Inspector
    public TMP_Text contadorCooldownTMP; // Asigna este objeto en el Inspector (opcional)

    // Nuevo: Duración total del modo sombra en segundos
    private const float duracionModoSombra = 5f; // ¡Recordatorio: esta es la duración del modo sombra!

    // Nuevo: Tiempo restante en modo sombra
    private float tiempoRestanteEnSombra;

    private Coroutine modoSombraCoroutine;
    private Coroutine cooldownCoroutine;

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

        // Nuevo: Asegurarse de que las referencias a las barras de UI estén asignadas
        if (barraDuracionSombraUI == null)
        {
            Debug.LogError("La barra de duración del modo sombra de la UI no ha sido asignada en el Inspector.");
        }
        else
        {
            barraDuracionSombraUI.gameObject.SetActive(false); // Inicialmente oculta
        }

        if (barraCooldownUI == null)
        {
            Debug.LogError("La barra de cooldown de la UI no ha sido asignada en el Inspector.");
        }
        else
        {
            barraCooldownUI.gameObject.SetActive(false); // Inicialmente oculta
        }

        // Nuevo: Inicialmente ocultar los contadores de texto si están asignados
        if (contadorDuracionSombraTMP != null)
        {
            contadorDuracionSombraTMP.gameObject.SetActive(false);
        }

        if (contadorCooldownTMP != null)
        {
            contadorCooldownTMP.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isShadowMode && puedeEntrarEnSombra)
        {
            modoSombraCoroutine = StartCoroutine(ActivarModoSombra());
        }
        else if (Input.GetKeyDown(KeyCode.Q) && isShadowMode)
        {
            StopCoroutine(modoSombraCoroutine);
            StartCoroutine(DesactivarModoSombra());
        }
    }

    IEnumerator CooldownModoSombra()
    {
        puedeEntrarEnSombra = false; // Desactivar la entrada al modo sombra

        float cooldownRestante = cooldownDuracion;

        // Mostrar barra de cooldown
        barraCooldownUI.gameObject.SetActive(true);
        barraCooldownUI.fillAmount = 0f;
        if (contadorCooldownTMP != null)
        {
            contadorCooldownTMP.gameObject.SetActive(true);
        }

        while (cooldownRestante > 0f)
        {
            cooldownRestante -= Time.deltaTime;

            // Actualizar barra y contador progresivamente
            barraCooldownUI.fillAmount = 1f - (cooldownRestante / cooldownDuracion);
            if (contadorCooldownTMP != null)
            {
                contadorCooldownTMP.text = Mathf.CeilToInt(cooldownRestante).ToString();
            }

            yield return null;
        }

        // Cooldown finalizado
        barraCooldownUI.gameObject.SetActive(false);
        if (contadorCooldownTMP != null)
        {
            contadorCooldownTMP.gameObject.SetActive(false);
        }

        puedeEntrarEnSombra = true; // NUEVO: permitir volver a entrar en modo sombra
        cooldownCoroutine = null; // Limpiar la referencia al coroutine de cooldown
    }

    IEnumerator ActivarModoSombra()
    {
        isShadowMode = true;
        tiempoRestanteEnSombra = duracionModoSombra;

        // Cambiar el tag a "Sombra"
        gameObject.tag = "Sombra";

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

        // Mostrar la barra de duración del modo sombra
        barraDuracionSombraUI.gameObject.SetActive(true);
        barraDuracionSombraUI.fillAmount = 1f; // Inicialmente llena
        if (contadorDuracionSombraTMP != null)
        {
            contadorDuracionSombraTMP.gameObject.SetActive(true);
            contadorDuracionSombraTMP.text = Mathf.CeilToInt(tiempoRestanteEnSombra).ToString();
        }

        // Loop mientras el tiempo restante sea mayor que cero
        while (tiempoRestanteEnSombra > 0)
        {
            // Actualizar la barra de duración progresivamente
            barraDuracionSombraUI.fillAmount = tiempoRestanteEnSombra / duracionModoSombra;

            // Actualizar el contador de tiempo de duración
            if (contadorDuracionSombraTMP != null)
            {
                contadorDuracionSombraTMP.text = Mathf.CeilToInt(tiempoRestanteEnSombra).ToString();
            }

            // Esperar un pequeño intervalo para la actualización progresiva
            yield return new WaitForSeconds(Time.deltaTime);

            // Decrementar el tiempo restante basado en el tiempo real transcurrido
            tiempoRestanteEnSombra -= Time.deltaTime;
        }

        // Asegurar que la barra llegue a 0 al final
        barraDuracionSombraUI.fillAmount = 0f;
        if (contadorDuracionSombraTMP != null)
        {
            contadorDuracionSombraTMP.text = "0";
        }

        // Iniciar la salida del modo sombra automáticamente al terminar el tiempo
        StartCoroutine(DesactivarModoSombra());
    }

    IEnumerator DesactivarModoSombra()
    {
        isShadowMode = false;

        // Cambiar el tag de vuelta a "Player"
        gameObject.tag = "Player";

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

        // Ocultar la barra de duración del modo sombra y su contador
        barraDuracionSombraUI.gameObject.SetActive(false);
        if (contadorDuracionSombraTMP != null)
        {
            contadorDuracionSombraTMP.gameObject.SetActive(false);
            contadorDuracionSombraTMP.text = duracionModoSombra.ToString(); // Resetear el contador visualmente
        }

        // Iniciar el cooldown si no está ya en curso
        if (cooldownCoroutine == null)
        {
            cooldownCoroutine = StartCoroutine(CooldownModoSombra());
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