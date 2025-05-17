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
            agent.SetDestination(player.position);
            animator.SetBool("walk", true);
            animator.SetBool("atrapado", false);
            animator.SetBool("descanso", false);

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= attackRange)
            {
                AttackPlayer();
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
            return;
        }

        agent.SetDestination(transform.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        animator.SetBool("walk", false);
        animator.SetBool("atrapado", true);
        animator.SetBool("descanso", false);

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(projectile, puntoDisparo.position, puntoDisparo.rotation).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        SendMessage("PlayClip", sonidoDisparo, SendMessageOptions.DontRequireReceiver);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (estaMuerto) return;

        if (other.CompareTag("Player"))
        {
            chasingPlayer = true;
            investigatingNoise = false;
            isSearchingNoise = false;
        }

        if (other.CompareTag("ruido") && !chasingPlayer && !investigatingNoise)
        {
            noisePosition = other.transform.position;
            investigatingNoise = true;
            isSearchingNoise = false;
            agent.isStopped = false;
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