using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float distance = 5f;
    public float speed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingForward = true;
    private bool isTurning = false;
    public Transform pivot;

    private Animator animator;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + transform.forward * distance;
        animator = GetComponent<Animator>(); // Asegúrate de que el Animator esté en el mismo GameObject
    }

    void Update()
    {
        if (!isTurning)
        {
            Move();
        }
    }

    void Move()
    {
        Vector3 target = movingForward ? targetPos : startPos;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        float distanceToTarget = Vector3.Distance(transform.position, target);
        animator.SetBool("isMoving", distanceToTarget > 0.01f);

        if (distanceToTarget < 0.01f)
        {
            StartCoroutine(TurnAround());
        }
    }

    System.Collections.IEnumerator TurnAround()
    {
        isTurning = true;
        animator.SetBool("isMoving", false);

        float rotationTime = 1f;
        float elapsed = 0f;
        float angle = 180f;

        Vector3 pivotPoint = pivot.position;
        Vector3 rotationAxis = Vector3.up; // eje Y (vertical)

        while (elapsed < rotationTime)
        {
            float step = (Time.deltaTime / rotationTime) * angle;
            transform.RotateAround(pivotPoint, rotationAxis, step);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ajustamos dirección hacia el nuevo target (por si lo necesitas)
        movingForward = !movingForward;
        isTurning = false;
        animator.SetBool("isMoving", true);
    }

}
