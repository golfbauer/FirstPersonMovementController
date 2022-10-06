using UnityEngine;
using System.Collections;

public class Physics : MonoBehaviour
{

	private Transform playerTransform;
	private CharacterController controller;

	private float gravity;
	private Vector3 playerVelocity;


    void Update()
    {
        applyGravity();
    }

    void applyGravity()
    {

        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

	public void SetGravity(float gravity)
    {
		this.gravity = gravity;
    }

	public void SetPlayerTransform(Transform playerTransform)
    {
		this.playerTransform = playerTransform;
    }

	public void SetController(CharacterController controller)
    {
		this.controller = controller;
    }
}

