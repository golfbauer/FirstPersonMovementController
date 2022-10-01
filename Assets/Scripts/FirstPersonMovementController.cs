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


    private MouseLook mouseLookCamera;
    private Movement playerMovement;

    private void Awake()
    {
        mouseLookCamera = Utils.CreateMouseLook(playerCamera, playerTransform, mouseSensivity);
        playerMovement = Utils.CreateMovement(this.gameObject, moveSpeed);
    }

    void Update()
    {
        
    }
}
