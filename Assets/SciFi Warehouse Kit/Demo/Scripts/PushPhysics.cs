using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPhysics : MonoBehaviour
{
    // This script pushes all rigidbodies that the character touches
    float pushPower = 2.0f;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // No rigidbody or is kinematic
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We don't want to push objects below us
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        // Calculate push direction from move direction
        // Only push objects to the sides, never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
}
