using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // Importar el espacio de nombres

public class playerMoveP : MonoBehaviour
{
    // Personaje
    Transform playerTr;
    Rigidbody playerRb;
    Animator playerAnim;

    public float playerSpeed = 5f; // Ajusta la velocidad base
    public bool hasPistol = false;
    private Vector2 newDirection;

    // Cámara
    public Transform cameraAxis;
    public Transform cameraTrack;
    public Transform cameraWeaponTrack;
    private Transform theCamera;

    private float rotY = 0f;
    private float rotX = 0f;

    public float camRotSpeed = 200f;
    public float minAngle = -45f;
    public float maxAngle = 45f;
    public float cameraSpeed = 200f;

    // Items
    public GameObject nearItem;
    public GameObject itemPrefab;
    public Transform itemSlot;
    public GameObject crosshair;
    public GameObject panel;

    private bool isAiming = false;

    // Post-Process para Apuntar
    public PostProcessVolume aimPostProcessVolume;
    public float aimPostProcessFadeDuration = 0.2f; // Tiempo para la transición del efecto al apuntar

    [Header("Subir Escaleras")]
    public float climbSpeed = 2f; // Velocidad de ascenso en la escalera
    public float stepHeightThreshold = 0.5f; // Altura máxima del escalón que se puede subir
    public string stairsTag = "Stairs"; // Etiqueta del objeto que representa la escalera
    private bool isOnStairs = false;

    void Start()
    {
        playerTr = this.transform;
        playerRb = this.GetComponent<Rigidbody>();
        theCamera = Camera.main.transform;
        playerAnim = this.GetComponentInChildren<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        cameraTrack.gameObject.SetActive(true);
        cameraWeaponTrack.gameObject.SetActive(false);

        // Inicializar el PostProcessVolume de apuntar
        if (aimPostProcessVolume != null)
        {
            aimPostProcessVolume.weight = 0f;
        }
        else
        {
            Debug.LogWarning("El PostProcessVolume para apuntar no ha sido asignado en el Inspector.");
        }
    }

    void Update()
    {
        CameraLogic();
        AnimLogic();
        ItemLogic();

        if (hasPistol)
        {
            if (Input.GetMouseButtonDown(1)) // Si presiona clic derecho
            {
                playerAnim.SetBool("holdPistol", true); // Activar animación de apuntar
                StartCoroutine(FadeAimPostProcess(0f, 1f)); // Activar efecto de la cámara al apuntar
            }
            else if (Input.GetMouseButtonUp(1)) // Si suelta el clic derecho
            {
                playerAnim.SetBool("holdPistol", false); // Volver a animación normal
                StartCoroutine(FadeAimPostProcess(1f, 0f)); // Desactivar efecto de la cámara al dejar de apuntar
            }
        }
    }

    void FixedUpdate()
    {
        MoveLogic(); // El movimiento basado en la física debe ir en FixedUpdate
    }

    public void MoveLogic()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float theTime = Time.fixedDeltaTime; // Usar Time.fixedDeltaTime para la física

        newDirection = new Vector2(moveX, moveZ);
        Vector3 side = playerSpeed * moveX * theTime * playerTr.right;
        Vector3 forward = playerSpeed * moveZ * theTime * playerTr.forward;
        Vector3 horizontalMovement = side + forward;
        Vector3 verticalMovement = Vector3.zero;

        if (isOnStairs)
        {
            // Anular la gravedad mientras está en las escaleras
            playerRb.useGravity = false;
            if (moveZ > 0) // Mover hacia adelante activa el ascenso
            {
                verticalMovement = Vector3.up * climbSpeed * theTime;
                // Opcional: Añadir un pequeño movimiento hacia adelante para seguir la inclinación
                // horizontalMovement += playerTr.forward * climbSpeed * 0.2f * theTime;
            }
            else if (moveZ < 0) // Mover hacia atrás para descender (opcional)
            {
                verticalMovement = Vector3.down * climbSpeed * theTime;
                // horizontalMovement -= playerTr.forward * climbSpeed * 0.2f * theTime;
            }
            playerRb.velocity = horizontalMovement + verticalMovement;
        }
        else
        {
            playerRb.useGravity = true;
            playerRb.velocity = horizontalMovement + Vector3.up * playerRb.velocity.y; // Mantener la velocidad vertical
        }

        // Intento de subir escalones más altos automáticamente
        if (!isOnStairs && moveZ > 0 && playerRb.velocity.y < 0.1f) // Si se mueve hacia adelante y está cerca del suelo
        {
            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 0.1f; // Raycast desde un poco arriba del pie
            if (Physics.Raycast(origin, transform.forward, out hit, 0.6f)) // Detecta si hay algo enfrente
            {
                Vector3 stepCheckOrigin = transform.position + Vector3.up * 0.2f + transform.forward * 0.4f;
                RaycastHit stepHit;
                if (!Physics.Raycast(stepCheckOrigin, Vector3.down, out stepHit, stepHeightThreshold))
                {
                    // No hay suelo directamente enfrente a una altura baja, podría ser un escalón
                    Vector3 liftVelocity = Vector3.up * 2f; // Pequeño impulso hacia arriba
                    playerRb.velocity += liftVelocity;
                }
            }
        }
    }

    public void CameraLogic()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float theTime = Time.deltaTime;

        rotY += mouseY * theTime * camRotSpeed;
        rotX = mouseX * theTime * camRotSpeed;
        playerTr.Rotate(0, rotX, 0);
        rotY = Mathf.Clamp(rotY, minAngle, maxAngle);
        cameraAxis.localRotation = Quaternion.Euler(-rotY, 0, 0);

        if (hasPistol && Input.GetMouseButton(1))
        {
            isAiming = true;
            cameraTrack.gameObject.SetActive(false);
            cameraWeaponTrack.gameObject.SetActive(true);
            crosshair.gameObject.SetActive(true);
            theCamera.position = Vector3.Lerp(theCamera.position, cameraWeaponTrack.position, cameraSpeed * theTime);
            theCamera.rotation = Quaternion.Lerp(theCamera.rotation, cameraWeaponTrack.rotation, cameraSpeed * theTime);
        }
        else
        {
            isAiming = false;
            cameraTrack.gameObject.SetActive(true);
            cameraWeaponTrack.gameObject.SetActive(false);
            crosshair.gameObject.SetActive(false);
            theCamera.position = Vector3.Lerp(theCamera.position, cameraTrack.position, cameraSpeed * theTime);
            theCamera.rotation = Quaternion.Lerp(theCamera.rotation, cameraTrack.rotation, cameraSpeed * theTime);
        }
    }

    public void AnimLogic()
    {
        playerAnim.SetFloat("X", newDirection.x);
        playerAnim.SetFloat("Y", newDirection.y);

        if (hasPistol)
        {
            playerAnim.SetLayerWeight(1, isAiming ? 1 : 0);
        }
    }

    public void ItemLogic()
    {
        if (nearItem != null && Input.GetKeyDown(KeyCode.E))
        {
            GameObject instantiateItem = Instantiate(itemPrefab, itemSlot.position, itemSlot.rotation);
            Destroy(nearItem.gameObject);
            instantiateItem.transform.parent = itemSlot;
            hasPistol = true;
            panel.SetActive(true);
            nearItem = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            Debug.Log("Hay un item cerca! tomalo con la tecla (E)");
            nearItem = other.gameObject;
        }
        else if (other.CompareTag(stairsTag) && !isOnStairs)
        {
            isOnStairs = true;
            playerRb.velocity = Vector3.zero; // Detener cualquier movimiento al entrar a la escalera
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            nearItem = null;
        }
        else if (other.CompareTag(stairsTag))
        {
            isOnStairs = false;
            playerRb.useGravity = true;
        }
    }


    IEnumerator FadeAimPostProcess(float startWeight, float endWeight)
    {
        float elapsedTime = 0f;
        float currentWeight = startWeight;

        while (elapsedTime < aimPostProcessFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newWeight = Mathf.Lerp(startWeight, endWeight, elapsedTime / aimPostProcessFadeDuration);

            if (aimPostProcessVolume != null)
            {
                aimPostProcessVolume.weight = newWeight;
            }

            yield return null;
        }

        // Asegurar que el peso final sea exactamente el objetivo
        if (aimPostProcessVolume != null)
        {
            aimPostProcessVolume.weight = endWeight;
        }
    }
}