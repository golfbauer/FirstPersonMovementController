using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

	private bool shouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && isGrounded;

	private KinematicCharacterController controller;
	private PlayerPhysics playerPhysics;
	
	private float moveSpeed;
	private float sprintSpeed;
	private float groundedStepOffset;

    private KeyCode sprintKey;
	private KeyCode jumpKey;
	private KeyCode crouchKey;

	private float crouchHeight;
	private float standingHeight;
	private float timeToCrouch;
	private Vector3 crouchingCenter;
	private Vector3 standingCenter;
	private bool isCrouching;
	private bool duringCrouchAnimation;

	private bool isGrounded;



    private void Start()
    {
		playerPhysics = GetComponent<PlayerPhysics>();

		groundedStepOffset = controller.StepOffset;
    }

    void Update()
	{
		isGrounded = playerPhysics.GetIsGrounded();

		if (Input.GetKey(sprintKey))
			CharacterMovement(sprintSpeed);
		else
			CharacterMovement(moveSpeed);

		CharacterJump();
		CharacterCrouch();
	}

	void CharacterMovement(float moveSpeed)
    {
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

		controller.Move(moveDirect * moveSpeed * Time.deltaTime);
	}

	void CharacterJump()
    {
		if(!isGrounded)
		{
			controller.StepOffset = controller.JumpingStepOffset;
        }
		else
		{
			controller.StepOffset = groundedStepOffset;
		}

		if(isGrounded && Input.GetKeyDown(jumpKey))
        {
			playerPhysics.SetApplyJumpForce(true);
        }
    }

	void CharacterCrouch()
    {
        if (shouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private IEnumerator CrouchStand()
    {
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = controller.Height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = controller.Center;

        while (timeElapsed < timeToCrouch)
        {
            controller.Height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            controller.Center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.Height = targetHeight;
        controller.Center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    public void SetCrouchHeight(float crouchHeight)
    {
		this.crouchHeight = crouchHeight;
    }

	public void SetStandingHeight(float standingHeight)
	{
		this.standingHeight = standingHeight;
	}

	public void SetTimeToCrouch(float timeToCrouch)
	{
		this.timeToCrouch = timeToCrouch;
	}

	public void SetCrouchingCenter(Vector3 crouchingCenter)
	{
		this.crouchingCenter = crouchingCenter;
	}

	public void SetStandingCenter(Vector3 standingCenter)
	{
		this.standingCenter = standingCenter;
	}

	public void SetCrouchKey(KeyCode crouchKey)
    {
		this.crouchKey = crouchKey;
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

	public void SetJumpKey(KeyCode jumpKey)
    {
		this.jumpKey = jumpKey;
    }

	public void SetController(KinematicCharacterController controller)
    {
		this.controller = controller;
    }
}

