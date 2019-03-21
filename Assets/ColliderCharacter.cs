using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class controls the collision between the player and objects
public class ColliderCharacter : MonoBehaviour {

    public float pushPower = 2.0F;      // The strength at which the player pushes the object

    // Whenever the player collides with an object (when walking)
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Get the collided object
        Rigidbody body = hit.collider.attachedRigidbody;

        // Check if the object can be affected by physics
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

        // Push the object in the direction of the movement
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }
}
