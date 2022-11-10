using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour
{
    public const float Epsilon = 0.001f;
    public const float MaxAngleShoveRadians = 90f;

	public static PlayerCameraLook CreatePlayerCameraLook(GameObject target, Transform playerTransform, float mouseSensitivity)
    {
        PlayerCameraLook mouseLook = target.AddComponent<PlayerCameraLook>();
        mouseLook.MouseSensitivity = mouseSensitivity;
        mouseLook.PlayerTransform = playerTransform;

        return mouseLook;
    }

    public static PlayerMovement CreateMovement(
        GameObject target,
        float moveSpeed, 
        float sprintSpeed, 
        KeyCode sprintKey, 
        KeyCode jumpKey,
        float crouchHeight, 
        float standingHeight, 
        float timeToCrouch, 
        Vector3 crouchingCenter, 
        Vector3 standingCenter,
        KeyCode crouchKey,
        Vector3 gravity,
        float jumpForce
        )
    {
        PlayerMovement movement = target.AddComponent<PlayerMovement>();
        movement.MoveSpeed = moveSpeed;
        movement.SprintSpeed = sprintSpeed;
        movement.SprintKey = sprintKey;
        movement.JumpKey = jumpKey;
        movement.CrouchHeight = crouchHeight;
        movement.StandingHeight = standingHeight;
        movement.TimeToCrouch = timeToCrouch;
        movement.CrouchingCenter = crouchingCenter;
        movement.StandingCenter = standingCenter;
        movement.CrouchKey = crouchKey;
        movement.Gravity = gravity;
        movement.JumpForce = jumpForce;

        return movement;
    }

    public static KinematicCharacterController CreateKinemeticCharacterController(
        GameObject target, 
        float slopeLimit, 
        float stairOffset,
        float stairSnapdownDistance,
        Vector3 center,
        float height, 
        float radius,
        float anglePower,
        float maxBounces
        )
    {
        KinematicCharacterController controller = target.AddComponent<KinematicCharacterController>();
        controller.SlopeLimit = slopeLimit;
        controller.StairOffset = stairOffset;
        controller.StairSnapdownDistance = stairSnapdownDistance;
        controller.Center = center;
        controller.Height = height;
        controller.Radius = radius;
        controller.AnglePower = anglePower;
        controller.MaxBounces = maxBounces;
        return controller;
    }
}

