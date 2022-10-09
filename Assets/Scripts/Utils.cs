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

    public static Movement CreateMovement(
        GameObject target, 
        CharacterController controller, 
        float moveSpeed, 
        float sprintSpeed, 
        KeyCode sprintKey, 
        KeyCode jumpKey, 
        float crouchHeight, 
        float standingHeight, 
        float timeToCrouch, 
        Vector3 crouchingCenter, 
        Vector3 standingCenter,
        KeyCode crouchKey
        )
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
        movement.SetJumpKey(jumpKey);
        movement.SetCrouchHeight(crouchHeight);
        movement.SetStandingHeight(standingHeight);
        movement.SetTimeToCrouch(timeToCrouch);
        movement.SetCrouchingCenter(crouchingCenter);
        movement.SetStandingCenter(standingCenter);
        movement.SetCrouchKey(crouchKey);

        return movement;
    }

    public static PlayerPhysics CreatePhysics(GameObject target, Transform playerTransform, CharacterController controller, float gravity, float jumpForce)
    {
        if(target == null || playerTransform == null)
        {
            throw new System.Exception("Assign PlayerBody to FirstPersonMovementController");
        }

        PlayerPhysics physics = target.AddComponent<PlayerPhysics>();
        physics.SetGravity(gravity);
        physics.SetPlayerTransform(playerTransform);
        physics.SetController(controller);
        physics.SetJumpForce(jumpForce);

        return physics;
    }
}

