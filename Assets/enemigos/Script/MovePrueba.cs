using UnityEngine;
using UnityEngine.AI;

public class MovePrueba : MonoBehaviour
{
    public Transform targetPosition; // Arrastra aquí el Transform del objeto destino desde el Inspector
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        // Obtén el componente NavMeshAgent adjunto a este objeto
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Verifica si el NavMeshAgent existe
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent no encontrado en este GameObject.");
            enabled = false; // Desactiva el script si no hay NavMeshAgent
        }
        else if (targetPosition == null)
        {
            Debug.LogError("No se ha asignado un Target Position en el Inspector.");
            enabled = false; // Desactiva el script si no hay objetivo
        }
        else
        {
            // Establece la posición de destino del NavMeshAgent
            navMeshAgent.destination = targetPosition.position;
        }
    }

    void Update()
    {
        // Puedes agregar lógica adicional aquí, por ejemplo, para verificar si ha llegado al destino.
        if (navMeshAgent != null && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            // El agente ha llegado a su destino (o está muy cerca).
            Debug.Log("¡He llegado al destino!");
            // Puedes realizar otras acciones aquí al llegar.
        }
    }
}