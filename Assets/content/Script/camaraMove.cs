using System.Collections;
using UnityEngine;

public class CamaraMove : MonoBehaviour
{
    [Header("Sonidos Cámara")]
    public AudioClip sonidoBom;
    public AudioClip sonidoChispas;

    public float rotationSpeed = 30f;
    public float maxRotation = 45f;
    public float pauseDuration = 2f;
    public float lookDownDuration = 1f;
    public GameObject detectorObject;
    public GameObject particleEffectPrefab;
    public GameObject additionalEffect1;
    public GameObject additionalEffect2;
    public GameObject objetoAActivar;

    private float currentRotation = 0f;
    private int direction = 1;
    private bool isPaused = false;
    public bool hasCollided = false; // Esta bandera ahora indica que la cámara está "desactivada"
    private Quaternion targetRotation;
    private bool isLookingDown = false;
    private float lookDownStartTime;
    private GameObject instantiatedEffect1;
    private GameObject instantiatedEffect2;

    // Ya no necesitamos destroyDelay aquí si no vamos a destruir el GameObject.
    // public float destroyDelay = 5f; 

    void Start()
    {
        // Registrar esta cámara en el inventario al inicio, solo si no ha sido "desactivada" previamente.
        // Esto es útil si cargas la escena y una cámara ya estaba en estado "hasCollided".
        if (CameraInventory.Instance != null && !hasCollided)
        {
            CameraInventory.Instance.AddCamera(gameObject);
        }

        targetRotation = Quaternion.Euler(60f, transform.eulerAngles.y, transform.eulerAngles.z);

        if (additionalEffect1 != null)
        {
            instantiatedEffect1 = Instantiate(additionalEffect1, transform);
            instantiatedEffect1.SetActive(false);
        }

        if (additionalEffect2 != null)
        {
            instantiatedEffect2 = Instantiate(additionalEffect2, transform);
            instantiatedEffect2.SetActive(false);
        }
    }

    void Update()
    {
        // Solo ejecuta la lógica de movimiento si no ha colisionado (está activa)
        if (!hasCollided)
        {
            if (!isPaused)
            {
                float rotationStep = rotationSpeed * Time.deltaTime * direction;
                currentRotation += rotationStep;
                transform.Rotate(Vector3.up, rotationStep);

                if (Mathf.Abs(currentRotation) >= maxRotation)
                {
                    StartCoroutine(PauseRotation());
                }
            }
        }
        else if (isLookingDown) // Esta parte seguirá ejecutándose si la cámara está "desactivada" y mirando hacia abajo
        {
            float elapsedTime = Time.time - lookDownStartTime;
            if (elapsedTime < lookDownDuration)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedTime / lookDownDuration);
            }
            else
            {
                transform.rotation = targetRotation;
                isLookingDown = false;
            }
        }
    }

    IEnumerator PauseRotation()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        direction *= -1;
        isPaused = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si ya colisionó para evitar múltiples restas
        if (other.CompareTag("Bala") && !hasCollided)
        {
            hasCollided = true; // Marca la cámara como "desactivada"
            StopAllCoroutines(); // Detiene la rotación y pausas
            isPaused = true;
            rotationSpeed = 0; // Detiene la rotación

            if (detectorObject != null)
            {
                detectorObject.SetActive(false); // Desactiva el detector (si es un componente visual)
            }

            isLookingDown = true;
            lookDownStartTime = Time.time;

            if (particleEffectPrefab != null)
            {
                GameObject explosion = Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 2f);
            }

            if (instantiatedEffect1 != null)
            {
                instantiatedEffect1.SetActive(true);
            }

            if (instantiatedEffect2 != null)
            {
                instantiatedEffect2.SetActive(true);
            }

            if (objetoAActivar != null)
            {
                objetoAActivar.SetActive(true);
                StartCoroutine(DesactivarObjeto(5f));
            }
            SendMessage("PlayClip", sonidoBom, SendMessageOptions.DontRequireReceiver);
            SendMessage("PlayClip", sonidoChispas, SendMessageOptions.DontRequireReceiver);

            // ¡Lo nuevo! Notificar al inventario que esta cámara ya no está "activa"
            if (CameraInventory.Instance != null)
            {
                CameraInventory.Instance.RemoveCamera(gameObject); // Remueve del conteo
            }

            // Opcional: Desactivar el componente CamaraMove si no quieres que siga haciendo nada más
            // Esto es útil si quieres que la cámara solo "muera" una vez y luego no ejecute más lógica.
            // this.enabled = false; 
            // Si la desactivas, la lógica isLookingDown no se ejecutará más. Decide si quieres eso.
            // Si quieres que siga mirando hacia abajo, no desactives el script.
        }
    }

    IEnumerator DesactivarObjeto(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (objetoAActivar != null)
        {
            objetoAActivar.SetActive(false);
        }
    }

    // Ya no necesitas OnDestroy si no vas a destruir el GameObject.
    // La notificación de "remoción" ya se hace en OnTriggerEnter.
    // private void OnDestroy()
    // {
    //     if (CameraInventory.Instance != null)
    //     {
    //         CameraInventory.Instance.RemoveCamera(gameObject);
    //     }
    //     Debug.Log($"Cámara '{gameObject.name}' destruida y removida del inventario.");
    // }
}