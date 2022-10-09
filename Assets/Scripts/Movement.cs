using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
	private CharacterController controller;

	private float moveSpeed;
	private float sprintSpeed;
	private KeyCode sprintKey;
	private float gravity;

	private Vector3 playerVelocity;

    void Update()
	{
		if(Input.GetKey(sprintKey))
			CharacterMovement(sprintSpeed);
		else
			CharacterMovement(moveSpeed);

	}

	void CharacterMovement(float moveSpeed)
    {
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

		controller.Move(moveDirect * moveSpeed * Time.deltaTime);
	}

	void ApplyGravity()
    {

        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

	public void SetMoveSpeed(float moveSpeed)
    {
		this.moveSpeed = moveSpeed;
    }

	public void SetSprintSpeed(float sprintSpeed)
    {
		this.sprintSpeed = sprintSpeed;
    }

	public void SetSprintKey(KeyCode sprintKey)
    {
		this.sprintKey = sprintKey;
    }

	public void SetGravity(float gravity)
	{
		this.gravity = gravity;
	}

	public void SetController(CharacterController controller)
    {
		this.controller = controller;
    }
}

