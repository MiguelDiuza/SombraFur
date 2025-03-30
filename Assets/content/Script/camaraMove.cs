using System.Collections;
using UnityEngine;

public class CamaraMove : MonoBehaviour
{
    public float rotationSpeed = 30f; // Velocidad de giro
    public float maxRotation = 45f; // Límite del giro en cada dirección
    public float pauseDuration = 2f; // Tiempo de pausa en cada extremo

    private float currentRotation = 0f;
    private int direction = 1;
    private bool isPaused = false;

    void Update()
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

    IEnumerator PauseRotation()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        direction *= -1; // Cambia de dirección
        isPaused = false;
    }
}
