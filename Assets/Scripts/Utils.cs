using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour
{
	public static MouseLook CreateMouseLook(GameObject target, Transform playerTransform, float mouseSensitivity)
    {
        if(playerTransform == null || target == null)
        {
            throw new System.Exception("Assign Camera and/or playerBody to FirstPersonMovementController");
        }

        MouseLook mouseLook = target.AddComponent<MouseLook>();
        mouseLook.SetPlayerTransform(playerTransform);
        mouseLook.SetMouseSensitivity(mouseSensitivity);

        return mouseLook;
    }

    public static Movement CreateMovement(GameObject target, float moveSpeed)
    {
        if (target == null)
        {
            throw new System.Exception("Assign PlayerBody to FirstPersonMovementController");
        }

        Movement movement = target.AddComponent<Movement>();
        movement.SetMoveSpeed(moveSpeed);
        return movement;
    }
}

