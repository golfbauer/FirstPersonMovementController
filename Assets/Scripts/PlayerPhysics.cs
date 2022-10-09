using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour
{

	private Transform playerTransform;
	private CharacterController controller;

	private float gravity;
	private Vector3 playerVelocity;
    private bool isGrounded;


    void Update()
    {
        CheckIsGrounded();
        Debug.Log(isGrounded);
        applyGravity();
    }

    void applyGravity()
    {

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void CheckIsGrounded()
    {
        return Physics.Raycast(playerTransform.position, Vector3.down, controller.height/2 + 0.1f);
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

