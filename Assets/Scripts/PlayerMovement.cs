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
    public float SlideControl { get; set; }
    public float WallRunSpeed { get; set; } = 20f;
    public float WallRunMaxAngle { get; set; } = 100f;
    public float WallJumpForce { get; set; } = 10f;
    public int WallRunLayer { get; set; } = 1 << 7;
    public int CountAllowedJumps { get; set; }

    public bool CanCancelSlide { get; set; }

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
    private bool isJumping;
    private bool isWallRunning;
    private bool isWallJumping;

    private bool onGround;
    private bool crouched;

    //Check whether player is allowed to perform action
    private bool canCrouch => onGround && !isSliding && Input.GetKeyDown(CrouchKey);
    private bool canJump =>
        (!(currentJumpCount == 0 && !onGround) &&
        currentJumpCount < CountAllowedJumps &&
        !isCrouching && !isSliding) && Input.GetKeyDown(JumpKey);
    private bool canSprint => onGround && Input.GetKey(SprintKey);
    private bool canSlide => isSprinting && !crouched && !isSliding && Input.GetKeyDown(SlideKey);
    private bool cancelSlide => CanCancelSlide && Input.GetKeyDown(SlideKey);
    private bool canWallRun => (isJumping || isWallRunning) && !isWallJumping;
    private bool canWallJump => (isWallRight || isWallLeft) && !onGround &&
        currentJumpCount <= CountAllowedJumps && Input.GetKeyDown(JumpKey);

    private float elapsedSinceJump;
    private float elapsedSinceNotOnGround;

    private int currentJumpCount;

    private float slideX;
    private float slideZ;
    private float timeElapsed;
    private Vector3 slideDirect;

    private bool isWallRight;
    private bool isWallLeft;

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

        PlayerWallRun();

        PlayerWallJump();

        PlayerCrouch();

        PlayerSlide();

        if (onGround)
        {
            movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
        }

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
        if (isJumping && (onGround || isWallRunning))
        {
            isJumping = false;
        }

        if (canJump || Jump)
        {
            velocity.y = Mathf.Sqrt(JumpForce * -3.0f * Gravity.y);
            currentJumpCount++;
            elapsedSinceJump = 0;
            Jump = false;
            isJumping = true;
        }

        elapsedSinceJump += Time.deltaTime;
    }

    // Todo: implement wall run
    // 1. Wall run is only possible when jumping & press a or d key with resptective dir & tag on wall | angle -> Done but all applied at the same  time
    // 2. Move with wall run speed in along wall -> Done
    // 3. reset jump count & player can jump from wall (not up but to the side) -> Done, but player will not be able to wallrun until hit ground!!! (add timer???)
    // 4. tilt camera to left and right angle can be set
    // 5. Make wall run possible without a and d key (needs jump to get loose)
    // Gravity is hardcoded
    // WalJump doesnt use normal but  transform.right???


    void PlayerWallRun()
    {
        if (canWallRun)
        {
            isWallRunning = false;
            isWallRight = Physics.Raycast(transform.position, transform.right, out RaycastHit hitWallRight, 1f, WallRunLayer);
            isWallLeft = Physics.Raycast(transform.position, -transform.right, out RaycastHit hitWallLeft, 1f, WallRunLayer);

            if (Input.GetKey(KeyCode.D) && isWallRight)
            {
                float wallAngle = Vector3.Angle(hitWallRight.normal, Vector3.up);
                isWallRunning = wallAngle < WallRunMaxAngle;
            }
            if (Input.GetKey(KeyCode.A) && isWallLeft)
            {
                float wallAngle = Vector3.Angle(hitWallLeft.normal, Vector3.up);
                isWallRunning = wallAngle < WallRunMaxAngle;
            }
        }

        if (isWallRunning)
        {
            Gravity = Vector3.zero;
            velocity = Vector3.zero;
            currentJumpCount = 0;

            if (PlayerWallJump()) return;

            Vector3 moveDirect = transform.right * (isWallRight ? 1f : -1f) + transform.forward;

            movement =  moveDirect * WallRunSpeed * Time.deltaTime;
            return;
        }

        Gravity = new Vector3(0, -19.62f, 0);
    }

    private bool PlayerWallJump()
    {
        isWallRunning = false;
        Gravity = new Vector3(0, -19.62f, 0);
        isWallRight = Physics.Raycast(transform.position, transform.right, out RaycastHit hitWallRight, 1f, WallRunLayer);
        isWallLeft = Physics.Raycast(transform.position, -transform.right, out RaycastHit hitWallLeft, 1f, WallRunLayer);

        if (isWallJumping && onGround)
        {
            isWallJumping = false;
        }

        if (canWallJump)
        {
            velocity = transform.right * (isWallRight ? -1f : 1f) * WallJumpForce;
            velocity.y = Mathf.Sqrt(JumpForce * -3.0f * Gravity.y);
            currentJumpCount++;
            elapsedSinceJump = 0;
            isWallJumping = true;
            return true;
        }
        return false;
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
            slideX = Input.GetAxis("Horizontal");
            slideZ = Input.GetAxis("Vertical");
            slideDirect = transform.right * slideX + transform.forward * slideZ;
            timeElapsed = 0;

            isSliding = true;
            Slide = false;
        }

        if (isSliding)
        {
            if (cancelSlide && timeElapsed > 0)
            {
                isSliding = false;
                Crouch = true;
                return;
            }

            if (timeElapsed < TimeSlide)
            {
                Vector3 currentSlideDirect =
                    slideDirect * (1f - SlideControl) +
                    (SlideControl * (slideX * transform.right + transform.forward * slideZ));
                movement = currentSlideDirect * SlideSpeed * Time.deltaTime;

                timeElapsed += Time.deltaTime;
                return;
            }
            isSliding = false;
            Crouch = true;
        }
    }
}

