using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovementController : MonoBehaviour
{

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private GameObject playerCamera;

    [SerializeField]
    private float mouseSensivity = 100f;

    [SerializeField]
    private float moveSpeed = 20f;

    [SerializeField]
    private float sprintSpeed = 40f;

    [SerializeField]
    private KeyCode sprintKey = KeyCode.LeftShift;

    [SerializeField]
    private float gravity = -9.81f;


    private CharacterController controller;

    private MouseLook mouseLookCamera;
    private Movement playerMovement;
    private PlayerPhysics playerPhysics;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        mouseLookCamera = Utils.CreateMouseLook(playerCamera, playerTransform, mouseSensivity);
        playerMovement = Utils.CreateMovement(this.gameObject, controller, moveSpeed, sprintSpeed, sprintKey, gravity);
        playerPhysics = Utils.CreatePhysics(this.gameObject, playerTransform, controller, gravity);
    }

    void Update()
    {
        
    }
}
