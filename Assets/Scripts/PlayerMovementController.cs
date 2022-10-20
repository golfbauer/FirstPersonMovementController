using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Kinematic Character Controller")]
    [SerializeField] private float slopeLimit;
    [SerializeField] private Vector3 center;
    [SerializeField] private float height;
    [SerializeField] private float radius;
    [SerializeField] private float anglePower;
    [SerializeField] private float maxBounces;

    [Header("Player Components")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject playerCamera;

    [Header("Mouse Configurations")]
    [SerializeField] private float mouseSensivity = 100f;

    [Header("Movement Configurations")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float sprintSpeed = 40f;

    [Header("Jumping Configurations")]
    [SerializeField] private float jumpForce = 1.0f;

    [Header("Crouch Configurations")]
    [SerializeField] private float crouchHeight;
    [SerializeField] private float standingHeight;
    [SerializeField] private float timeToCrouch;
    [SerializeField] private Vector3 crouchingCenter;
    [SerializeField] private Vector3 standingCenter;

    [Header("Environmental Configurations")]
    [SerializeField] private Vector3 gravity = new Vector3(0, -9.81f, 0);

    [Header("Key Binding Configurations")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    private PlayerCameraLook mouseLookCamera;
    private PlayerMovement playerMovement;
    private KinematicCharacterController controller;


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
            jumpForce
            );

    }
}
