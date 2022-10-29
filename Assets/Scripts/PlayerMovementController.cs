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

    [Header("Crouch Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private float crouchHeight;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float standingHeight;
    [SerializeField] [OnChangedCall("OnVariableChange")] private float timeToCrouch;
    [SerializeField] [OnChangedCall("OnVariableChange")] private Vector3 crouchingCenter;
    [SerializeField] [OnChangedCall("OnVariableChange")] private Vector3 standingCenter;

    [Header("Slide Configurations")]
    [SerializeField][OnChangedCall("OnVariableChange")] private float slideSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float timeSlide;

    [Header("Environmental Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private Vector3 gravity = new Vector3(0, -9.81f, 0);

    [Header("Key Binding Configurations")]
    [SerializeField] [OnChangedCall("OnVariableChange")] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] [OnChangedCall("OnVariableChange")] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] [OnChangedCall("OnVariableChange")] private KeyCode crouchKey = KeyCode.C;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode slideKey = KeyCode.C;

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
            center, 
            height, 
            radius,
            anglePower,
            maxBounces
        );

        mouseLookCamera = Utils.CreatePlayerCameraLook(playerCamera, playerTransform, mouseSensivity);

        playerMovement = Utils.CreateMovement(
            this.gameObject,
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
            timeSlide
            );

    }

    public void OnVariableChange()
    {
        if (configDuringRuntime)
        {
            if(controller != null)
            {
                controller.SlopeLimit = slopeLimit;
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
            }

            if (playerMovement != null)
            {
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
            }
        }
    }
}
