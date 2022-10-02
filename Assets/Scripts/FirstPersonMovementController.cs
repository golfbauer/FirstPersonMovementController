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
    private float mouseSensivity;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float gravity;


    private CharacterController controller;

    private MouseLook mouseLookCamera;
    private Movement playerMovement;
    private Physics physics;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        mouseLookCamera = Utils.CreateMouseLook(playerCamera, playerTransform, mouseSensivity);
        playerMovement = Utils.CreateMovement(this.gameObject, controller, moveSpeed);
        physics = Utils.CreatePhysics(this.gameObject, playerTransform, controller, gravity);
    }

    void Update()
    {
        
    }
}
