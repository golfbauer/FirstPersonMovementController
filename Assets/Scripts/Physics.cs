using UnityEngine;
using System.Collections;

public class Physics : MonoBehaviour
{

	private Transform playerTransform;
	private CharacterController controller;

	private float gravity;
	private float velocityY = 0.0f;


	void Update()
	{
		applyGravity();
	}

	void applyGravity()
    {
        velocityY += gravity * Time.deltaTime;

        if (controller.isGrounded)
			velocityY = -0.2f;

        controller.Move(Vector3.up * velocityY);
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

