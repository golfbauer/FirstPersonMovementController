using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovementController : MonoBehaviour
{
    [Header("Capsule Collider")]
    [SerializeField] private float slopeLimit;
    [SerializeField] private float stepOffset;
    [SerializeField] private float jumpingStepOffset = 0.1f;
    [SerializeField] private float skinWidth;
    [SerializeField] private Vector3 center;
    [SerializeField] private float height;
    [SerializeField] private float radius;

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
    [SerializeField] private float gravity = -9.81f;

    [Header("Key Bindings")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    private MouseLook mouseLookCamera;
    private Movement playerMovement;
    private PlayerPhysics playerPhysics;
    private CapsuleCharacterController controller;


    private void Awake()
    {
        controller = Utils.CreateCapsuleCharacterController(this.gameObject, slopeLimit, stepOffset, jumpingStepOffset, skinWidth, center, height, radius);
        mouseLookCamera = Utils.CreateMouseLook(playerCamera, playerTransform, mouseSensivity);
        playerMovement = Utils.CreateMovement(
            this.gameObject,
            controller,
            moveSpeed, 
            sprintSpeed, 
            sprintKey, 
            jumpKey,
            crouchHeight,
            standingHeight,
            timeToCrouch,
            crouchingCenter,
            standingCenter,
            crouchKey
            );
        playerPhysics = Utils.CreatePhysics(this.gameObject, controller, playerTransform, gravity, jumpForce);
    }
}
