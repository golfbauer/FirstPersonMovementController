using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour
{

	private Transform playerTransform;
	private CharacterController controller;

	private float gravity;
	private Vector3 playerVelocity;
    private bool isGrounded;

    private float jumpForce;
    private bool applyJumpForce = false;


    void Update()
    {
        isGrounded = CheckIsGrounded();
        applyGravity();
    }

    void applyGravity()
    {

        if (isGrounded && playerVelocity.y < -0.1f)
        {
            playerVelocity.y = -0.1f;
        }

        if (applyJumpForce)
        {
            applyJumpForce = false;
            playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private bool CheckIsGrounded()
    {
        RaycastHit hit;
        return Physics.SphereCast(playerTransform.position, controller.radius , Vector3.down, out hit, controller.height/2);
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }

	public void SetGravity(float gravity)
    {
		this.gravity = gravity;
    }

    public void SetApplyJumpForce(bool applyJumpForce)
    {
        this.applyJumpForce = applyJumpForce;
    }

    public void SetJumpForce(float jumpForce)
    {
        this.jumpForce = jumpForce;
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

