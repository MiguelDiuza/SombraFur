using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMoveP : MonoBehaviour
{

    //personaje

    Transform playerTr;
    Rigidbody playerRb;
    Animator playerAnim;

    public float playerSpeed = 0f;
    private Vector2 newDirection;

    //Camara

    public Transform cameraAxis;
    public Transform cameraTrack;
    private Transform theCamera;

    private float rotY = 0f;
    private float rotX = 0f;

    public float camRotSpeed = 200f;
    public float minAngle = -45f;
    public float maxAngle = 45f;
    public float cameraSpeed = 200f;

    void Start()
    {
        playerTr = this.transform;
        playerRb = this.GetComponent<Rigidbody>();

        theCamera = Camera.main.transform;
        playerAnim = this.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        MoveLogic();
        CameraLogic();
        AnimLogic();
    }

    public void MoveLogic()
    {
        // Obtener la dirección actual del Rigidbody
        Vector3 direction = playerRb.velocity;

        // Capturar entrada del jugador
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float theTime = Time.deltaTime;

        newDirection = new Vector2 (moveX, moveZ);

        // Calcular movimiento en cada dirección
        Vector3 side = playerSpeed * moveX * theTime * playerTr.right;
        Vector3 forward = playerSpeed * moveZ * theTime * playerTr.forward;

        // Direccion final
        Vector3 endDirection = side + forward;

        // Aplicar movimiento al Rigidbody
        playerRb.velocity = endDirection;
    }

    public void CameraLogic()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float theTime = Time.deltaTime;

        // Ajustar rotación en Y y X con sensibilidad
        rotY += mouseY * theTime * camRotSpeed;
        rotX = mouseX * theTime * camRotSpeed;

        // Aplicar rotación horizontal al jugador
        playerTr.Rotate(0, rotX, 0);

        // Limitar la rotación en Y para evitar giros excesivos
        rotY = Mathf.Clamp(rotY, minAngle, maxAngle);

        // Aplicar la rotación a la cámara
        Quaternion localRotation = Quaternion.Euler(-rotY, 0, 0);
        cameraAxis.localRotation = localRotation;

        // Interpolación de posición y rotación para suavizar el movimiento de la cámara
        theCamera.position = Vector3.Lerp(theCamera.position, cameraTrack.position, cameraSpeed * theTime);
        theCamera.rotation = Quaternion.Lerp(theCamera.rotation, cameraTrack.rotation, cameraSpeed * theTime);
    }

    public void AnimLogic()
    {
        playerAnim.SetFloat("X", newDirection.x);
        playerAnim.SetFloat("Y", newDirection.y);
    }

}
