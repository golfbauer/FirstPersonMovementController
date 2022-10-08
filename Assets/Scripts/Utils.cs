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

    public static Movement CreateMovement(GameObject target, CharacterController controller, float moveSpeed, float sprintSpeed, KeyCode sprintKey)
    {
        if (target == null || controller == null)
        {
            throw new System.Exception("Assign PlayerBody to FirstPersonMovementController");
        }

        Movement movement = target.AddComponent<Movement>();
        movement.SetMoveSpeed(moveSpeed);
        movement.SetController(controller);
        movement.SetSprintSpeed(sprintSpeed);
        movement.SetSprintKey(sprintKey);

        return movement;
    }

    public static Physics CreatePhysics(GameObject target, Transform playerTransform, CharacterController controller, float gravity)
    {
        if(target == null || playerTransform == null)
        {
            throw new System.Exception("Assign PlayerBody to FirstPersonMovementController");
        }

        Physics physics = target.AddComponent<Physics>();
        physics.SetGravity(gravity);
        physics.SetPlayerTransform(playerTransform);
        physics.SetController(controller);

        return physics;
    }
}

