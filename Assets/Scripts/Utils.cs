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
        CapsuleCharacterController controller,
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
        if (target == null)
        {
            throw new System.Exception("Assign PlayerBody to FirstPersonMovementController");
        }

        Movement movement = target.AddComponent<Movement>();
        movement.SetController(controller);
        movement.SetMoveSpeed(moveSpeed);
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

    public static PlayerPhysics CreatePhysics(GameObject target, CapsuleCharacterController controller, Transform playerTransform, float gravity, float jumpForce)
    {
        if(target == null || playerTransform == null)
        {
            throw new System.Exception("Assign PlayerBody to FirstPersonMovementController");
        }

        PlayerPhysics physics = target.AddComponent<PlayerPhysics>();
        physics.SetGravity(gravity);
        physics.SetPlayerTransform(playerTransform);
        physics.SetJumpForce(jumpForce);
        physics.SetController(controller);

        return physics;
    }

    public static CapsuleCharacterController CreateCapsuleCharacterController(GameObject target, float slopeLimit, float stepOffset,float jumpingStepOffset, float skinWidth, Vector3 center,float height, float radius)
    {
        CapsuleCharacterController controller = target.AddComponent<CapsuleCharacterController>();
        controller.SlopeLimit = slopeLimit;
        controller.StepOffset = stepOffset;
        controller.JumpingStepOffset = jumpingStepOffset;
        controller.SkinWidth = skinWidth;
        controller.Center = center;
        controller.Height = height;
        controller.Radius = radius;

        return controller;
    }
}

