using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalCameraController : MonoBehaviour
{
    public Transform player;  // Referencia al personaje
    public Transform cameraPivot; // Punto de pivote para la cámara (útil en TPS)

    public float mouseSensitivity = 200f;
    public float minYAngle = -60f;
    public float maxYAngle = 60f;
    public float cameraSmoothSpeed = 10f; // Suavidad del seguimiento

    private float rotX = 0f; // Rotación en X (izquierda-derecha)
    private float rotY = 0f; // Rotación en Y (arriba-abajo)

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor en el centro
    }

    void Update()
    {
        CameraControl();
    }

    private void CameraControl()
    {
        // Captura la entrada del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotación horizontal (personaje gira en Y)
        rotX += mouseX;
        player.rotation = Quaternion.Euler(0, rotX, 0);

        // Rotación vertical (cámara gira en X)
        rotY -= mouseY;
        rotY = Mathf.Clamp(rotY, minYAngle, maxYAngle); // Limita la inclinación

        // Aplicar rotación al pivote de la cámara
        cameraPivot.localRotation = Quaternion.Euler(rotY, 0, 0);
    }
}
