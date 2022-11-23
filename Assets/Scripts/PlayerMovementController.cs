using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool configDuringRuntime = false;

    [Header("Kinematic Character Controller")]

    [SerializeField] [OnChangedCall("OnVariableChange")] private float slopeLimit;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float stairOffset;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float stairSnapdownDistance;
    [SerializeField] [OnChangedCall("OnVariableChange")] private Vector3 center;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float height;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float radius;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float anglePower;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float maxBounces;

    [Header("Player Components")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private Transform playerTransform;
    [SerializeField] [OnChangedCall("OnVariableChange")] private GameObject playerCamera;

    [Header("Mouse Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private float mouseSensivity = 100f;

    [Header("Movement Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private float moveSpeed = 20f;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float sprintSpeed = 40f;

    [Header("Jumping Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private float jumpForce = 1.0f;
    [SerializeField] [OnChangedCall("OnVariableChange")] private int countAllowedJumps = 1;

    [Header("WallRun Configurations")]
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunSpeed = 20f;
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunMaxAngle = 100f;
    [SerializeField][OnChangedCall("OnVariableChange")] private int wallRunLayer = 1 << 7;
    [SerializeField][OnChangedCall("OnVariableChange")] private float maxTimeOnWall = 500f;
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunGravityMultiplier = 0f;
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunMinimumHeight = 1f;
    [SerializeField][OnChangedCall("OnVariableChange")] private float timeToTiltCameraWallRun = 0.5f;
    [SerializeField][OnChangedCall("OnVariableChange")] private float maxCameraTilt = 20f;

    [Header("WallJump Configurations")]
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector2 wallJumpForce = new Vector2(10f, 2f);
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallPushForce = 2f;
    [SerializeField][OnChangedCall("OnVariableChange")] private bool canChangeWallJumpDirect = true;

    [Header("Grapple Configurations")]
    [SerializeField][OnChangedCall("OnVariableChange")] private int grappleLayer = 1 << 7;
    [SerializeField][OnChangedCall("OnVariableChange")] private float maxGrappleDistance;
    [SerializeField][OnChangedCall("OnVariableChange")] private float grappleCoolDown;
    [SerializeField][OnChangedCall("OnVariableChange")] private float grappleSpeed;

    [Header("Crouch Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private float crouchHeight;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float standingHeight;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float timeToCrouch;
    [SerializeField] [OnChangedCall("OnVariableChange")] private Vector3 crouchingCenter;
    [SerializeField] [OnChangedCall("OnVariableChange")] private Vector3 standingCenter;

    [Header("Slide Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private float slideSpeed;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float timeSlide;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float slideControll;
    [SerializeField] [OnChangedCall("OnVariableChange")] private bool canCancelSlide;

    [Header("Environmental Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private Vector3 gravity = new Vector3(0, -9.81f, 0);

    [Header("Key Binding Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] [OnChangedCall("OnVariableChange")] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] [OnChangedCall("OnVariableChange")] private KeyCode crouchKey = KeyCode.C;
    [SerializeField] [OnChangedCall("OnVariableChange")] private KeyCode slideKey = KeyCode.C;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode wallRunKey = KeyCode.Space;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode wallJumpKey;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode grappleKey;

    [Header("Head Bob Configurations")]
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector2 crouchHeadBobWalk;
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector2 crouchHeadBobSprint;
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector2 crouchHeadBobDefault;
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector2 sprintHeadBob;
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector2 walkHeadBob;
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector2 defaultHeadBob;

    private PlayerCameraLook mouseLookCamera;
    private PlayerMovement playerMovement;
    private KinematicCharacterController controller;

    public PlayerCameraLook MouseLookCamera
    {
        get
        {
            return mouseLookCamera;
        }
    }

    public PlayerMovement PlayerMovement
    {
        get
        {
            return playerMovement;
        }
    }

    public KinematicCharacterController Controller
    {
        get
        {
            return controller;
        }
    }


    private void Awake()
    {
        controller = Utils.CreateKinemeticCharacterController(
            this.gameObject, 
            slopeLimit,
            stairOffset, 
            stairSnapdownDistance,
            center, 
            height, 
            radius,
            anglePower,
            maxBounces
        );

        mouseLookCamera = Utils.CreatePlayerCameraLook(playerCamera, playerTransform, mouseSensivity, timeToTiltCameraWallRun, maxCameraTilt);

        playerMovement = Utils.CreateMovement(
            this.gameObject,
            playerCamera,
            moveSpeed,
            sprintSpeed,
            sprintKey,
            jumpKey,
            crouchHeight,
            standingHeight,
            timeToCrouch,
            crouchingCenter,
            standingCenter,
            crouchKey,
            gravity,
            jumpForce,
            countAllowedJumps,
            slideKey,
            slideSpeed,
            timeSlide,
            canCancelSlide,
            slideControll,
            wallRunSpeed,
            wallRunMaxAngle,
            wallRunLayer,
            maxTimeOnWall,
            wallRunGravityMultiplier,
            wallRunMinimumHeight,
            wallRunKey,
            wallJumpForce,
            wallPushForce,
            canChangeWallJumpDirect,
            wallJumpKey,
            crouchHeadBobWalk,
            crouchHeadBobSprint,
            crouchHeadBobDefault,
            sprintHeadBob,
            walkHeadBob,
            defaultHeadBob,
            grappleLayer,
            grappleCoolDown,
            grappleSpeed,
            maxGrappleDistance,
            grappleKey
            );

    }

    public void OnVariableChange()
    {
        if (configDuringRuntime)
        {
            if(controller != null)
            {
                controller.SlopeLimit = slopeLimit;
                controller.StairOffset = stairOffset;
                controller.StairSnapdownDistance = stairSnapdownDistance;
                controller.Center = center;
                controller.Height = height;
                controller.Radius = radius;
                controller.AnglePower = anglePower;
                controller.MaxBounces = maxBounces;
            }

            if (mouseLookCamera != null)
            {
                mouseLookCamera.PlayerTransform = playerTransform;
                mouseLookCamera.MouseSensitivity = mouseSensivity;
                mouseLookCamera.MaxCameraTilt = maxCameraTilt;
                mouseLookCamera.TimeToTiltCameraWallRun = timeToTiltCameraWallRun;
            }

            if (playerMovement != null)
            {
                playerMovement.PlayerCamera = playerCamera;
                playerMovement.MoveSpeed = moveSpeed;
                playerMovement.SprintSpeed = sprintSpeed;
                playerMovement.SprintKey = sprintKey;
                playerMovement.JumpKey = jumpKey;
                playerMovement.CrouchHeight = crouchHeight;
                playerMovement.StandingHeight = standingHeight;
                playerMovement.TimeToCrouch = timeToCrouch;
                playerMovement.CrouchingCenter = crouchingCenter;
                playerMovement.StandingCenter = standingCenter;
                playerMovement.CrouchKey = crouchKey;
                playerMovement.Gravity = gravity;
                playerMovement.JumpForce = jumpForce;
                playerMovement.CountAllowedJumps = countAllowedJumps;

                playerMovement.SlideKey = slideKey;
                playerMovement.SlideSpeed = slideSpeed;
                playerMovement.TimeSlide = timeSlide;
                playerMovement.CanCancelSlide = canCancelSlide;
                playerMovement.SlideControl = slideControll;

                playerMovement.WallRunSpeed = wallRunSpeed;
                playerMovement.WallRunMaxAngle = wallRunMaxAngle;
                playerMovement.WallRunLayer = 1 << wallRunLayer;
                playerMovement.MaxTimeOnWall = maxTimeOnWall;
                playerMovement.WallRunGravityMultiplier = wallRunGravityMultiplier;
                playerMovement.WallRunMinimumHeight = wallRunMinimumHeight;
                playerMovement.WallRunKey = wallRunKey;

                playerMovement.WallJumpForce = wallJumpForce;
                playerMovement.WallPushForce = wallPushForce;
                playerMovement.CanChangeWallJumpDirect = canChangeWallJumpDirect;
                playerMovement.WallJumpKey = wallJumpKey;

                playerMovement.CrouchHeadBobDefault = crouchHeadBobDefault;
                playerMovement.CrouchHeadBobWalk = crouchHeadBobWalk;
                playerMovement.CrouchHeadBobSprint = crouchHeadBobSprint;
                playerMovement.DefaultHeadBob = defaultHeadBob;
                playerMovement.WalkHeadBob = walkHeadBob;
                playerMovement.SprintHeadBob = sprintHeadBob;

                playerMovement.GrappleLayer = grappleLayer;
                playerMovement.GrappleCoolDown = grappleCoolDown;
                playerMovement.GrappleSpeed = grappleSpeed;
                playerMovement.MaxGrappleDistance = maxGrappleDistance;
                playerMovement.GrappleKey = grappleKey;
            }
        }
    }
}
