using System.Collections;
using UnityEngine;

public class CamaraMove : MonoBehaviour
{
    public float rotationSpeed = 30f; // Velocidad de giro
    public float maxRotation = 45f; // Límite del giro en cada dirección
    public float pauseDuration = 2f; // Tiempo de pausa en cada extremo
    public float lookDownDuration = 1f; // Tiempo de la animación de mirar hacia abajo
    public GameObject particleEffectPrefab; // Prefab del efecto de partículas
    public GameObject additionalEffect1; // Primer efecto adicional
    public GameObject additionalEffect2; // Segundo efecto adicional

    private float currentRotation = 0f;
    private int direction = 1;
    private bool isPaused = false;
    private bool hasCollided = false; // Evita repeticiones
    private Quaternion targetRotation;
    private bool isLookingDown = false; // Controla la animación de mirar abajo
    private float lookDownStartTime;
    private GameObject instantiatedEffect1;
    private GameObject instantiatedEffect2;

    void Start()
    {
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
        else if (isLookingDown)
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
        if (other.CompareTag("Bala") && !hasCollided)
        {
            hasCollided = true;
            StopAllCoroutines();
            isPaused = true;
            rotationSpeed = 0;

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
        }
    }
}