using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour
{

    // Public variables that can be set via Unity Editor
    public float MoveSpeed { get; set; }
    public float SprintSpeed { get; set; }
    public float SlideSpeed { get; set; }
    public float CrouchHeight { get; set; }
    public float StandingHeight { get; set; }
    public float TimeToCrouch { get; set; }
    public float TimeSlide { get; set; }
    public float JumpForce { get; set; }
    public int CountAllowedJumps { get; set; }

    public KeyCode SprintKey { get; set; }
    public KeyCode JumpKey { get; set; }
    public KeyCode CrouchKey { get; set; }
    public KeyCode SlideKey { get; set; }

    public Vector3 CrouchingCenter { get; set; }
    public Vector3 StandingCenter { get; set; }
    public Vector3 Gravity { get; set; }

    // Public variables that can be set via Scripts
    public bool Jump { get; set; } = false;
    public bool Sprint { get; set; } = false;
    public bool Slide { get; set; } = false;
    public bool Crouch { get; set; } = false;

    //Set to true if player is doing the action
    private bool isCrouching;
    private bool isSliding;
    private bool isSprinting;

    private bool onGround;
    private bool crouched;

    //Check whether player is allowed to perform action
    private bool canCrouch => onGround && Input.GetKeyDown(CrouchKey);
    private bool canJump =>
        (!(currentJumpCount == 0 && !onGround) &&
        currentJumpCount < CountAllowedJumps &&
        !isCrouching) && Input.GetKeyDown(JumpKey);
    private bool canSprint => onGround && Input.GetKey(SprintKey);
    private bool canSlide => (isSprinting && Input.GetKeyDown(SlideKey) && !crouched);

    private float elapsedSinceJump;
    private float elapsedSinceNotOnGround;
    private int currentJumpCount;
    private Vector3 velocity;
    private Vector3 movement;

    private KinematicCharacterController controller;

    private void Start()
    {
        controller = GetComponent<KinematicCharacterController>();
    }

    void Update()
    {
        onGround = controller.CheckGrounded(out RaycastHit groundHit);
        if (!onGround)
        {
            velocity += Gravity * Time.deltaTime;
            elapsedSinceNotOnGround += Time.deltaTime;
        }
        else
        {
            if (!(velocity.y > 0))
            {
                velocity = Vector3.zero;
                elapsedSinceNotOnGround = 0;
                currentJumpCount = 0;
            }
        }

        if (canSprint || Sprint)
        {
            movement = MoveDirect(SprintSpeed);
            isSprinting = true;
        }
        else
        {
            movement = MoveDirect(MoveSpeed);
            isSprinting = false;
        }

        PlayerJump();

        PlayerCrouch();

        PlayerSlide();

        if (onGround)
        {
            movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
        }

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
        if (canJump || Jump)
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
        if (canCrouch || Crouch)
        {
            StartCoroutine(CrouchStand());
            Crouch = false;
        }
    }

    private IEnumerator CrouchStand()
    {
        isCrouching = true;

        float timeElapsed = 0;
        float targetHeight = crouched ? StandingHeight : CrouchHeight;
        float currentHeight = controller.Height;
        Vector3 targetCenter = crouched ? StandingCenter : CrouchingCenter;
        Vector3 currentCenter = controller.Center;
        Vector3 initialPosition = transform.position;

        while (timeElapsed < TimeToCrouch)
        {
            float controllerHeight = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / TimeToCrouch);

            if (crouched)
            {
                float ColliderHeightDifference = controllerHeight - controller.Height;

                transform.position += Vector3.up * (ColliderHeightDifference + Utils.Epsilon);
            }

            controller.Height = controllerHeight;
            controller.Center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / TimeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        
        controller.Height = targetHeight;
        controller.Center = targetCenter;

        crouched = !crouched;
        isCrouching = false;
    }

    void PlayerSlide()
    {
        if (canSlide || Slide)
        {
            StartCoroutine(SlideStand());
            Slide = false;
        }
    }

    private IEnumerator SlideStand()
    {
        isSliding = true;

        float slideX = Input.GetAxis("Horizontal");
        float slideZ = Input.GetAxis("Vertical");
        float timeElapsed = 0;

        while(timeElapsed < TimeSlide)
        {
            MoveDirectSlide(slideX, slideZ);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        Crouch = true;
        isSliding = false;
    }

    void MoveDirectSlide(float slideX, float slideZ)
    {
        onGround = controller.CheckGrounded(out RaycastHit groundHit);

        Vector3 moveDirect = transform.right * slideX + transform.forward * slideZ;
        moveDirect = moveDirect * Time.deltaTime * SlideSpeed;

        if (onGround)
        {
            moveDirect = Vector3.ProjectOnPlane(moveDirect, groundHit.normal);
        }

        transform.position = controller.MovePlayer(moveDirect);
    }
}

