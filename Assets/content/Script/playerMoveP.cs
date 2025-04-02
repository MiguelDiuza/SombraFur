using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMoveP : MonoBehaviour
{
    // Personaje
    Transform playerTr;
    Rigidbody playerRb;
    Animator playerAnim;

    public float playerSpeed = 0f;
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

    void Start()
    {
        playerTr = this.transform;
        playerRb = this.GetComponent<Rigidbody>();
        theCamera = Camera.main.transform;
        playerAnim = this.GetComponentInChildren<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        cameraTrack.gameObject.SetActive(true);
        cameraWeaponTrack.gameObject.SetActive(false);
    }

    void Update()
    {
        MoveLogic();
        CameraLogic();
        AnimLogic();
        ItemLogic();

        if (hasPistol)
        {
            if (Input.GetMouseButtonDown(1)) // Si presiona clic derecho
            {
                playerAnim.SetBool("holdPistol", true); // Activar animación de apuntar
            }
            else if (Input.GetMouseButtonUp(1)) // Si suelta el clic derecho
            {
                playerAnim.SetBool("holdPistol", false); // Volver a animación normal
            }
        }

    }

    public void MoveLogic()
    {
        Vector3 direction = playerRb.velocity;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float theTime = Time.deltaTime;

        newDirection = new Vector2(moveX, moveZ);
        Vector3 side = playerSpeed * moveX * theTime * playerTr.right;
        Vector3 forward = playerSpeed * moveZ * theTime * playerTr.forward;
        Vector3 endDirection = side + forward;
        playerRb.velocity = endDirection;
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            Debug.Log("Ya no hay un item cerca!");
            nearItem = null;
        }
    }
}