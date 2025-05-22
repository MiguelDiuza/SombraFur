using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CameraInventory : MonoBehaviour
{
    public static CameraInventory Instance { get; private set; }

    [Header("Referencias UI")]
    public TextMeshProUGUI cameraCountText;

    [Header("Cámaras en Escena")]
    [Tooltip("Lista de todas las cámaras en la escena. Se llenará automáticamente.")]
    public List<GameObject> activeCameras = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Considera si quieres que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // En Start, encuentra todas las cámaras y solo añade las que no estén "desactivadas" inicialmente.
        CamaraMove[] existingCameras = FindObjectsOfType<CamaraMove>();
        foreach (CamaraMove cam in existingCameras)
        {
            // Solo agrega la cámara si NO ha colisionado previamente (es decir, está "activa")
            if (!cam.GetComponent<CamaraMove>().hasCollided)
            {
                AddCamera(cam.gameObject);
            }
        }

        UpdateCameraCountUI();
    }

    public void AddCamera(GameObject cameraObject)
    {
        // Asegúrate de que la cámara no esté ya en la lista y no esté en estado "colisionado"
        if (!activeCameras.Contains(cameraObject) && cameraObject.GetComponent<CamaraMove>().hasCollided == false)
        {
            activeCameras.Add(cameraObject);
            UpdateCameraCountUI();
            
        }
    }

    public void RemoveCamera(GameObject cameraObject)
    {
        if (activeCameras.Contains(cameraObject))
        {
            activeCameras.Remove(cameraObject);
            UpdateCameraCountUI();
            
        }
    }

    private void UpdateCameraCountUI()
    {
        if (cameraCountText != null)
        {
            cameraCountText.text = "" + activeCameras.Count;
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI 'cameraCountText' no asignado en CameraInventory.");
        }
    }
}