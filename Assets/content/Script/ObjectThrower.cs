using UnityEngine;

using System.Collections;



public class ObjectThrower : MonoBehaviour

{

    public Transform throwOrigin; // El punto desde donde se lanza la lata (ej: la mano del personaje)

    public GameObject canPrefab; // Prefab de la lata

    public float throwSpeed = 10f;

    public float arcHeight = 2f; // Altura máxima de la parábola

    public Animator animator; // Asigna el Animator del jugador

    public LineRenderer trajectoryRenderer; // Para visualizar la parábola

    public int trajectoryPoints = 30; // Número de puntos para la parábola

    public float trajectoryTimeStep = 0.1f; // Intervalo de tiempo entre puntos



    private bool isThrowingEnabled = false;

    private bool isAiming = false;



    void Update()

    {

        // Activar/Desactivar el lanzamiento con la tecla 2

        if (Input.GetKeyDown(KeyCode.Alpha2))

        {

            isThrowingEnabled = !isThrowingEnabled;

            Debug.Log("puedes lanzar latas: " + isThrowingEnabled);

            // Opcional: Aquí podrías activar/desactivar algún indicador visual

        }



        // Apuntar (opcional, puedes lanzar directamente al hacer clic si lo prefieres)

        if (isThrowingEnabled)

        {

            if (Input.GetMouseButton(1))

            {

                isAiming = true;

                UpdateTrajectory(); // Mostrar la trayectoria mientras se apunta

            }

            else

            {

                isAiming = false;

                trajectoryRenderer.positionCount = 0; // Ocultar la trayectoria

            }



            // Lanzar con clic izquierdo

            if (Input.GetMouseButtonDown(0) && isAiming && !IsInvoking("PerformThrow"))

            {

                RaycastHit hit;

                Vector3 targetPoint;

                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))

                {

                    targetPoint = hit.point;

                }

                else

                {

                    targetPoint = Camera.main.transform.position + Camera.main.transform.forward * 10f; // Punto lejano por defecto

                }



                animator.SetBool("lanzarSi", true); // Activa animación

                Invoke("PerformThrow", 0.2f); // Llama a PerformThrow después de un pequeño retardo para sincronizar con la animación

                StartCoroutine(ResetThrowAnimation());

                StartCoroutine(ThrowCan(targetPoint));

            }

        }



        // Desactivar el lanzamiento con la tecla 1

        if (Input.GetKeyDown(KeyCode.Alpha1))

        {

            isThrowingEnabled = false;

            isAiming = false;

            trajectoryRenderer.positionCount = 0;

            Debug.Log("Lanzamiento desactivado.");

            // Opcional: Aquí podrías desactivar el indicador visual

        }

    }



    void PerformThrow()

    {

        // Esta función se llama mediante Invoke para sincronizar con la animación

    }



    System.Collections.IEnumerator ResetThrowAnimation()

    {

        yield return new WaitForSeconds(1f); // Ajusta la duración según tu animación

        animator.SetBool("lanzarSi", false);

    }



    System.Collections.IEnumerator ThrowCan(Vector3 target)

    {

        // Espera pequeña para que coincida con el lanzamiento en la animación (ajusta si es necesario)

        yield return new WaitForSeconds(0.3f); // Ajusta este valor para sincronizar con el frame de lanzamiento



        GameObject can = Instantiate(canPrefab, throwOrigin.position, Quaternion.identity);

        Rigidbody rb = can.GetComponent<Rigidbody>();



        if (rb != null)

        {

            Vector3 velocity = CalculateParabolicVelocity(throwOrigin.position, target, arcHeight);

            rb.velocity = velocity;

        }

    }



    /// <summary>

    /// Calcula la velocidad inicial necesaria para lanzar un objeto de A a B con una parábola que alcanza cierta altura.

    /// </summary>

    Vector3 CalculateParabolicVelocity(Vector3 start, Vector3 end, float height)

    {

        float gravity = Mathf.Abs(Physics.gravity.y);



        // Dirección horizontal (XZ)

        Vector3 horizontal = new Vector3(end.x - start.x, 0, end.z - start.z);

        float horizontalDistance = horizontal.magnitude;



        // Altura entre puntos

        float verticalDistance = end.y - start.y;



        float timeUp = Mathf.Sqrt(2 * height / gravity);

        float timeDown = Mathf.Sqrt(2 * (height - verticalDistance) / gravity);

        float totalTime = timeUp + timeDown;



        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * height);

        Vector3 velocityXZ = horizontal / totalTime;



        return velocityXZ + velocityY;

    }



    void UpdateTrajectory()

    {

        if (!isThrowingEnabled || !isAiming)

        {

            trajectoryRenderer.positionCount = 0;

            return;

        }



        trajectoryRenderer.positionCount = trajectoryPoints;

        Vector3 targetPoint;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))

        {

            targetPoint = hit.point;

        }

        else

        {

            targetPoint = Camera.main.transform.position + Camera.main.transform.forward * 10f;

        }



        for (int i = 0; i < trajectoryPoints; i++)

        {

            float time = i * trajectoryTimeStep;

            Vector3 position = CalculateParabolicPoint(throwOrigin.position, targetPoint, arcHeight, time);

            trajectoryRenderer.SetPosition(i, position);

        }

    }



    Vector3 CalculateParabolicPoint(Vector3 start, Vector3 end, float height, float time)

    {

        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 horizontal = new Vector3(end.x - start.x, 0, end.z - start.z);

        float distance = horizontal.magnitude;

        float totalTime = Mathf.Sqrt(2 * height / gravity) + Mathf.Sqrt(2 * Mathf.Abs(height - (end.y - start.y)) / gravity);



        if (totalTime <= 0) return start; // Evitar división por cero



        float normalizedTime = time / totalTime;

        Vector3 point = Vector3.Lerp(start, end, normalizedTime);

        point.y = ParabolaEquation(start.y, end.y, height, normalizedTime);

        return point;

    }



    float ParabolaEquation(float y0, float y1, float h, float t)

    {

        float a = -4 * h / 1;

        float b = 4 * h;

        float c = y0;

        return a * t * t + b * t + c;

    }

}