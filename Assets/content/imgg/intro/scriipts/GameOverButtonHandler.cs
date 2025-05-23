using UnityEngine;
using UnityEngine.UI; // Necesario para Button
using UnityEngine.EventSystems; // Necesario para EventSystem

public class GameOverButtonHandler : MonoBehaviour
{
    [Header("Boton de Game Over")]
    [Tooltip("Arrastra aquí el botón que quieres que se active con Enter.")]
    public Button myButton;

    void Start()
    {
        // Asegúrate de que el botón esté asignado
        if (myButton == null)
        {
            Debug.LogError("No se ha asignado el botón en el Inspector para GameOverButtonHandler.");
            return;
        }

        // Si hay un EventSystem activo, selecciona el botón al inicio
        // Esto es útil si el mouse está deshabilitado
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(myButton.gameObject);
        }
        else
        {
            Debug.LogWarning("No se encontró un EventSystem activo en la escena. El botón no se seleccionará automáticamente.");
        }
    }

    void Update()
    {
        // Detecta la tecla Enter (o Return)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Verifica si el botón está activo en la jerarquía y es interactuable
            if (myButton != null && myButton.gameObject.activeInHierarchy && myButton.interactable)
            {
                // Simula un clic en el botón
                myButton.onClick.Invoke();
            }
        }
    }
}