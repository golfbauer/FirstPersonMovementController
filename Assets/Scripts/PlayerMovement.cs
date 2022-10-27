using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour
{

    // Public variables that can be set via Unity Editor
    public float MoveSpeed { get; set; }
	public float SprintSpeed { get; set; }
	public float CrouchHeight { get; set; }
	public float StandingHeight { get; set; }
	public float TimeToCrouch { get; set; }
    public float JumpForce { get; set; }
    public int CountAllowedJumps { get; set; }

    public KeyCode SprintKey { get; set; }
    public KeyCode JumpKey { get; set; }
    public KeyCode CrouchKey { get; set; }

    public Vector3 CrouchingCenter { get; set; }
    public Vector3 StandingCenter { get; set; }
    public Vector3 Gravity { get; set; }

    // Public variables that can be set via Scripts
    public bool Jump { get; set; } = false;
    public bool Sprint { get; set; } = false;

	private bool isCrouching;
	private bool duringCrouchAnimation;
    private bool onGround;

    private bool canCrouch => onGround;
    private bool canJump =>
        (!(currentJumpCount == 0 && !onGround) &&
        currentJumpCount < CountAllowedJumps &&
        !duringCrouchAnimation) || Jump;
    private bool canSprint => onGround || Sprint;

    private float elapsedSinceJump;
    private float elapsedSinceNotOnGround;
    private int currentJumpCount;
    private Vector3 velocity;

	private KinematicCharacterController controller;

    private void Start()
    {
		controller  = GetComponent<KinematicCharacterController>();
    }

    void Update()
	{
		onGround = controller.CheckGrounded(out RaycastHit groundHit);
        if (!onGround)
        {
            velocity += Gravity * Time.deltaTime;
            elapsedSinceNotOnGround += Time.deltaTime;
        } else
        {
            if (!(velocity.y > 0))
            {
                velocity = Vector3.zero;
                elapsedSinceNotOnGround = 0;
                currentJumpCount = 0;
            }
        }

        Vector3 movement;

        if((Input.GetKey(SprintKey) && canSprint) || Sprint)
        {
            movement = MoveDirect(SprintSpeed);
        } else {
            movement = MoveDirect(MoveSpeed);
        }


        if (onGround)
        {
            movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
        }

        PlayerJump();

        PlayerCrouch();

        // Attempt to move the player based on player movement
        transform.position = controller.MovePlayer(movement);

        // Move player based on falling speed
        transform.position = controller.MovePlayer(velocity * Time.deltaTime);
	} 
    
	private Vector3 MoveDirect(float moveSpeed)
    {
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

        return moveDirect * moveSpeed * Time.deltaTime;
	}

	void PlayerJump()
    {
		if(canJump && Input.GetKeyDown(JumpKey))
        { 
			velocity.y = Mathf.Sqrt(JumpForce * -3.0f * Gravity.y);
            currentJumpCount++;
            elapsedSinceJump = 0;
            Jump = false;
        }

        elapsedSinceJump += Time.deltaTime;
    }

    void PlayerCrouch()
    {
        if (canCrouch && Input.GetKeyDown(CrouchKey))
        {
            StartCoroutine(CrouchStand());
        }
    }

    private IEnumerator CrouchStand()
    {
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? StandingHeight : CrouchHeight;
        float currentHeight = controller.Height;
        Vector3 targetCenter = isCrouching ? StandingCenter : CrouchingCenter;
        Vector3 currentCenter = controller.Center;

        while (timeElapsed < TimeToCrouch)
        {
            float controllerHeight = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / TimeToCrouch);

            if (isCrouching)
            {
                float ColliderHeightDifference = controllerHeight - controller.Height;

                transform.position += Vector3.up * ColliderHeightDifference;
            }

            controller.Height = controllerHeight;
            controller.Center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / TimeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.Height = targetHeight;
        controller.Center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }
}

