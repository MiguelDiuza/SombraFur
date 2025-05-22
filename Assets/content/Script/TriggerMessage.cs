using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
    [Tooltip("El mensaje que se mostrará en la consola cuando el jugador colisione.")]
    [TextArea(3, 10)] // Makes the text area larger in the Inspector
    public string messageOnTrigger = "¡Jugador o Sombra ha colisionado!";

    [Tooltip("Permite detectar también objetos con el tag 'Sombra'.")]
    public bool detectShadowTag = false;

    private bool hasTriggered = false; // This flag ensures the message is sent only once

    private void OnTriggerEnter(Collider other)
    {
        // Only proceed if the message hasn't been triggered yet
        if (!hasTriggered)
        {
            // Check if the colliding object has the "Player" tag
            // OR if detectShadowTag is true and the object has the "Sombra" tag
            if (other.CompareTag("Player") || (detectShadowTag && other.CompareTag("Sombra")))
            {
                Debug.Log(messageOnTrigger); // Show the message in the console
                hasTriggered = true; // Set the flag to true so it won't trigger again
            }
        }
    }
}