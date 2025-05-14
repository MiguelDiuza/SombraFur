using UnityEngine;

public class ArmaSeguirMano : MonoBehaviour
{
    public Transform manoPivot; // Este es el GameObject vacío en la mano del NPC

    void LateUpdate()
    {
        if (manoPivot != null)
        {
            transform.position = manoPivot.position;
            transform.rotation = manoPivot.rotation;
        }
    }
}
