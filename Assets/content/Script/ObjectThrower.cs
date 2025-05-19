using UnityEngine;

public class ObjectThrower : MonoBehaviour
{
    [Header("Configuración del Lanzamiento")]
    public Transform puntoLanzamiento;
    public GameObject prefabLata;
    public float velocidadLanzamiento = 25f;
    [Range(0f, 90f)]
    public float anguloMaximoVertical = 80f; // Límite superior del ángulo de lanzamiento
    [Range(0f, 90f)]
    public float anguloMinimoVertical = -80f; // Límite inferior del ángulo de lanzamiento
    public int puntosTrayectoria = 30; // Número de puntos para la trayectoria
    public float tiempoEntrePuntos = 0.1f; // Tiempo entre cada punto de la trayectoria
    [Range(0f, 90f)]
    public float anguloMaximoVisionVertical = 85f; // Ángulo máximo de la cámara hacia arriba para permitir lanzamiento

    [Header("Referencias")]
    public Animator animator;
    public Inventario inventario;
    public LineRenderer lineaTrayectoria;
    public Camera camaraPrincipal; // Referencia a la cámara principal

    private bool modoLanzamiento = false;

    void Start()
    {
        // Asegurarse de que el LineRenderer esté presente
        if (lineaTrayectoria == null)
        {
            Debug.LogError("¡LineRenderer no asignado al ObjectThrower!");
            enabled = false; // Desactivar el script si falta el LineRenderer
        }
        lineaTrayectoria.enabled = false; // Inicialmente la trayectoria no se muestra

        // Asegurarse de que la cámara principal esté asignada
        if (camaraPrincipal == null)
        {
            camaraPrincipal = Camera.main;
            if (camaraPrincipal == null)
            {
                Debug.LogError("¡Cámara principal no encontrada! Asigna una cámara al ObjectThrower.");
                enabled = false; // Desactivar si no se encuentra la cámara
            }
        }
    }

    void Update()
    {
        // Activar modo lanzamiento con la tecla 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            modoLanzamiento = true;
            Debug.Log("Modo lanzamiento ACTIVADO");
        }

        // Desactivar modo lanzamiento con la tecla 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            modoLanzamiento = false;
            lineaTrayectoria.enabled = false; // Ocultar la trayectoria al desactivar el modo
            Debug.Log("Modo lanzamiento DESACTIVADO");
        }

        // Si está activado el modo de lanzamiento
        if (modoLanzamiento)
        {
            // Verificar el ángulo de la cámara
            float anguloCamaraVertical = Vector3.Dot(camaraPrincipal.transform.forward, Vector3.up);

            // Si la cámara está mirando demasiado hacia arriba, ocultar la trayectoria y no permitir el lanzamiento
            if (anguloCamaraVertical > Mathf.Sin(anguloMaximoVisionVertical * Mathf.Deg2Rad))
            {
                lineaTrayectoria.enabled = false;
                return; // Salir del Update para no permitir el lanzamiento
            }
            else
            {
                lineaTrayectoria.enabled = true; // Mostrar la trayectoria si la cámara está dentro del ángulo permitido
            }

            // Calcular dirección hacia donde apunta el mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 destino;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                destino = hit.point;
            }
            else
            {
                destino = ray.GetPoint(100f); // punto lejano
            }

            // Calcular la dirección de lanzamiento
            Vector3 direccion = (destino - puntoLanzamiento.position).normalized;

            // Limitar el ángulo vertical del lanzamiento
            Vector3 planoHorizontal = Vector3.ProjectOnPlane(direccion, Vector3.up).normalized;
            float anguloVertical = Vector3.Angle(planoHorizontal, direccion);

            // Ajustar la dirección si el ángulo vertical excede los límites
            if (Vector3.Dot(Vector3.up, direccion) > 0) // Si la dirección es hacia arriba
            {
                if (anguloVertical > anguloMaximoVertical)
                {
                    direccion = Quaternion.AngleAxis(anguloMaximoVertical - anguloVertical, Vector3.Cross(planoHorizontal, direccion)) * direccion;
                }
            }
            else // Si la dirección es hacia abajo
            {
                if (anguloVertical > -anguloMinimoVertical)
                {
                    direccion = Quaternion.AngleAxis(-anguloMinimoVertical - anguloVertical, Vector3.Cross(planoHorizontal, direccion)) * direccion;
                }
            }

            // Actualizar la visualización de la trayectoria
            ActualizarTrayectoria(direccion);

            // Al hacer clic izquierdo, intenta lanzar si hay latas
            if (Input.GetMouseButtonDown(0))
            {
                if (inventario.GetLatas() > 0)
                {
                    StartCoroutine(LanzarLata(direccion)); // Pasar la dirección limitada al lanzamiento
                }
                else
                {
                    Debug.Log("No tienes latas disponibles.");
                }
            }
        }
    }

    void ActualizarTrayectoria(Vector3 direccion)
    {
        lineaTrayectoria.positionCount = puntosTrayectoria;
        Vector3 puntoInicial = puntoLanzamiento.position;
        Vector3 velocidadInicial = direccion * velocidadLanzamiento;

        for (int i = 0; i < puntosTrayectoria; i++)
        {
            float tiempo = i * tiempoEntrePuntos;
            Vector3 punto = CalcularPuntoTrayectoria(puntoInicial, velocidadInicial, tiempo);
            lineaTrayectoria.SetPosition(i, punto);
        }
    }

    Vector3 CalcularPuntoTrayectoria(Vector3 posicionInicial, Vector3 velocidadInicial, float tiempo)
    {
        // Ecuación de movimiento parabólico: posición final = posición inicial + velocidad inicial * tiempo + 0.5 * gravedad * tiempo^2
        return posicionInicial + velocidadInicial * tiempo + 0.5f * Physics.gravity * tiempo * tiempo;
    }

    System.Collections.IEnumerator LanzarLata(Vector3 direccionLanzamiento)
    {
        // Ejecutar animación de lanzar
        animator.SetTrigger("throw");
        animator.SetBool("lanzarSi", true);

        // Esperar que inicie la animación
        yield return new WaitForSeconds(0.2f);

        // Instanciar y lanzar la lata usando la dirección limitada
        GameObject lata = Instantiate(prefabLata, puntoLanzamiento.position, Quaternion.identity);
        Rigidbody rb = lata.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = direccionLanzamiento * velocidadLanzamiento;
        }

        // Restar una lata del inventario
        inventario.AgregarLatas(-1);

        // Finalizar animación
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("lanzarSi", false);
    }
}