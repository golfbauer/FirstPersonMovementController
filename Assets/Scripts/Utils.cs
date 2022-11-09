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
        GameObject playerCamera,
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
        float jumpForce,
        int countAllowedJumps,
        KeyCode slideKey,
        float slideSpeed,
        float timeSlide,
        bool canCancelSlide,
        float slideControll,
        float wallRunSpeed,
        float wallRunMaxAngle,
        int wallRunLayer,
        float maxTimeOnWall,
        float wallRunGravityMultiplier,
        float wallRunMinimumHeight,
        KeyCode wallRunKey,
        Vector2 wallJumpForce,
        float wallPushForce,
        bool canChangeWallJumpDirect
        )
    {
        PlayerMovement movement = target.AddComponent<PlayerMovement>();
        movement.PlayerCamera = playerCamera;
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
        movement.CountAllowedJumps = countAllowedJumps;
        movement.SlideKey = slideKey;
        movement.SlideSpeed = slideSpeed;
        movement.TimeSlide = timeSlide;
        movement.CanCancelSlide = canCancelSlide;
        movement.SlideControl = slideControll;

        movement.WallRunSpeed = wallRunSpeed;
        movement.WallRunMaxAngle = wallRunMaxAngle;
        movement.WallRunLayer = wallRunLayer;
        movement.MaxTimeOnWall = maxTimeOnWall;
        movement.WallRunGravityMultiplier = wallRunGravityMultiplier;
        movement.WallRunMinimumHeight = wallRunMinimumHeight;
        movement.WallRunKey = wallRunKey;   

        movement.WallJumpForce = wallJumpForce;
        movement.WallPushForce = wallPushForce;
        movement.CanChangeWallJumpDirect = canChangeWallJumpDirect;

        return movement;
    }

    public static KinematicCharacterController CreateKinemeticCharacterController(
        GameObject target, 
        float slopeLimit, 
        Vector3 center,
        float height, 
        float radius,
        float anglePower,
        float maxBounces
        )
    {
        KinematicCharacterController controller = target.AddComponent<KinematicCharacterController>();
        controller.SlopeLimit = slopeLimit;
        controller.Center = center;
        controller.Height = height;
        controller.Radius = radius;
        controller.AnglePower = anglePower;
        controller.MaxBounces = maxBounces;
        return controller;
    }

    public enum WallRunDirect
    {
        Right,
        Left,
        Up,
        Down,
        Stop
    }
}

