using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardiaAI : MonoBehaviour
{
    public AudioClip sonidoMuerte;
    public AudioClip sonidoDisparo;


    public NavMeshAgent agent;
    public Animator animator;
    public float health;

    [Header("Jugador")]
    public Transform player;
    public Collider visionCollider;

    [Header("Patrullaje")]
    public List<Transform> patrolPoints;
    private int patrolIndex = 0;
    private bool goingForward = true;

    [Header("Ruido")]
    public float noiseDetectionRange = 5f; // Rango dentro del cual el guardia reacciona al ruido
    private bool investigatingNoise = false;
    private Vector3 noisePosition;
    private bool isSearchingNoise = false;
    private float totalRotation = 0f;
    private float rotationSpeedForSearch = 360f; // Velocidad de rotación para la búsqueda (grados/segundo)

    [Header("Rangos")]
    public float sightRange = 15f;
    public float attackRange = 10f;

    [Header("Ataque")]
    public GameObject projectile;
    public Transform puntoDisparo;
    public float timeBetweenAttacks = 2f;
    private bool alreadyAttacked;
    public float initialAttackDelay = 2f; // Nuevo: Retraso inicial antes del primer ataque
    private Coroutine attackCoroutine; // Variable para almacenar la corrutina de ataque

    [Header("Estados")]
    private bool chasingPlayer = false;
    private bool estaMuerto = false;
    private bool muriendo = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (estaMuerto) return;

        if (!muriendo && health <= 0)
        {
            muriendo = true;
            Morir();
            return;
        }

        // ¡Prioridad al jugador! Si está persiguiendo, hacer eso.
        if (chasingPlayer && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= attackRange)
            {
                agent.isStopped = true; // Detener al agente al entrar en rango
                animator.SetBool("walk", false); // Detener la animación de caminar
                animator.SetBool("atrapado", true); // Activar animación de ataque

                // Iniciar la corrutina de ataque solo si no está ya en ejecución
                if (attackCoroutine == null)
                {
                    attackCoroutine = StartCoroutine(DelayedAttack());
                }
            }
            else
            {
                agent.isStopped = false; // Continuar persiguiendo si está fuera de rango
                agent.SetDestination(player.position);
                animator.SetBool("walk", true);
                animator.SetBool("atrapado", false);
            }
            return; // Importante: Salir del Update para no ejecutar otras lógicas
        }
        // Si no está persiguiendo al jugador, verificar si está investigando un ruido
        else if (investigatingNoise)
        {
            if (!isSearchingNoise)
            {
                agent.SetDestination(noisePosition);
                animator.SetBool("walk", true);
                animator.SetBool("atrapado", false);
                animator.SetBool("descanso", false);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
                {
                    agent.isStopped = true;
                    animator.SetBool("walk", false);
                    animator.SetBool("descanso", true);
                    isSearchingNoise = true;
                    totalRotation = 0f;
                }
            }
            else
            {
                if (totalRotation < 360f)
                {
                    float rotationThisFrame = rotationSpeedForSearch * Time.deltaTime;
                    transform.Rotate(Vector3.up, rotationThisFrame);
                    totalRotation += rotationThisFrame;
                }
                else
                {
                    investigatingNoise = false;
                    isSearchingNoise = false;
                    animator.SetBool("descanso", false);
                }
            }
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Count == 0) return;

        agent.isStopped = false;
        animator.SetBool("walk", true);
        animator.SetBool("atrapado", false);
        animator.SetBool("descanso", false);

        agent.SetDestination(patrolPoints[patrolIndex].position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            UpdatePatrolIndex();
        }
    }

    void UpdatePatrolIndex()
    {
        if (goingForward)
        {
            patrolIndex++;
            if (patrolIndex >= patrolPoints.Count)
            {
                patrolIndex = patrolPoints.Count - 2;
                goingForward = false;
            }
        }
        else
        {
            patrolIndex--;
            if (patrolIndex < 0)
            {
                patrolIndex = 1;
                goingForward = true;
            }
        }
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange)
        {
            chasingPlayer = false;
            agent.isStopped = false; // Reactivar el movimiento al salir del rango
            animator.SetBool("walk", true);
            animator.SetBool("atrapado", false);
            // Detener la corrutina si el jugador sale del rango
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            return;
        }

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!alreadyAttacked)
        {
            // Calcular dirección hacia el jugador (con diferencia de altura)
            Vector3 direccion = (player.position + Vector3.up * 1.5f - puntoDisparo.position).normalized;

            // Instanciar el proyectil
            Rigidbody rb = Instantiate(projectile, puntoDisparo.position, Quaternion.LookRotation(direccion)).GetComponent<Rigidbody>();

            // Aplicar fuerza en la dirección calculada
            rb.AddForce(direccion * 40f, ForceMode.Impulse); // Puedes ajustar la potencia aquí

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        SendMessage("PlayClip", sonidoDisparo, SendMessageOptions.DontRequireReceiver);
    }

    IEnumerator DelayedAttack()
    {
        // Esperar el tiempo de retraso inicial
        yield return new WaitForSeconds(initialAttackDelay);

        // Después del retraso, atacar mientras el jugador esté en rango y no esté muerto
        while (chasingPlayer && player != null && !estaMuerto)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
                yield return new WaitForSeconds(timeBetweenAttacks); // Esperar entre ataques
            }
            else
            {
                // Si el jugador sale del rango durante la corrutina, detenerla
                attackCoroutine = null;
                yield break;
            }
        }
        // Asegurarse de que la corrutina se reinicie si termina (por ejemplo, si chasingPlayer se vuelve falso)
        attackCoroutine = null;
    }

    void ResetAttack() => alreadyAttacked = false;

    public void TakeDamage(int damage)
    {
        if (estaMuerto) return;
        health -= damage;
        if (health <= 0 && !muriendo)
        {
            muriendo = true;
            Morir();
        }
    }

    void Morir()
    {
        estaMuerto = true;
        agent.isStopped = true;
        animator.SetTrigger("muertoB");
        animator.SetBool("walk", false);
        animator.SetBool("atrapado", false);
        animator.SetBool("descanso", false);
        Destroy(agent);
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Collider>(), 1f);
        this.enabled = false;
        SendMessage("PlayClip", sonidoMuerte, SendMessageOptions.DontRequireReceiver);
        Debug.Log("Neutralizaste un guardia");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (estaMuerto) return;

        if (other.CompareTag("Player"))
        {
            chasingPlayer = true;
            investigatingNoise = false;
            isSearchingNoise = false;
            Debug.Log("Te descubrieron");
        }

        if (other.CompareTag("ruido") && !chasingPlayer && !investigatingNoise)
        {
            noisePosition = other.transform.position;
            investigatingNoise = true;
            isSearchingNoise = false;
            agent.isStopped = false;
            Debug.Log("El guardia esta investigando el ruido");
        }

        if (other.CompareTag("Bala"))
        {
            TakeDamage(100);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, noiseDetectionRange);
    }
}