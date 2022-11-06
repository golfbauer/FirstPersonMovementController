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
    public bool CanChangeWallJumpDirect { get; set; } = true;

    public KeyCode SprintKey { get; set; }
    public KeyCode JumpKey { get; set; }
    public KeyCode CrouchKey { get; set; }
    public KeyCode SlideKey { get; set; }
    public KeyCode WallRunKey { get; set; } = KeyCode.Space;

    public Vector3 CrouchingCenter { get; set; }
    public Vector3 StandingCenter { get; set; }
    public Vector3 Gravity { get; set; }

    public GameObject PlayerCamera { get; set; }

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
    private bool canWallRun => (isJumping || isWallRunning) && Input.GetKey(WallRunKey);
    private bool canWallJump =>isWallRunning &&
        currentJumpCount <= CountAllowedJumps && Input.GetKeyUp(WallRunKey);

    private float elapsedSinceJump;
    private float elapsedSinceNotOnGround;

    private int currentJumpCount;

    private float slideX;
    private float slideZ;
    private float timeElapsed;
    private Vector3 slideDirect;

    private bool isWallRight;
    private bool isWallLeft;
    private bool isWallFront;
    private bool isWallBack;

    private Vector3 velocity;
    private Vector3 movement;

    private KinematicCharacterController controller;
    private PlayerCameraLook playerCamera;

    private void Start()
    {
        controller = GetComponent<KinematicCharacterController>();
        playerCamera = PlayerCamera.GetComponent<PlayerCameraLook>();
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


    // 5. Make wall run possible without a and d key (needs jump to get loose)
    // Gravity is hardcoded
    // WalJump doesnt use normal but  transform.right???

    private string prevWallDirect;
    void PlayerWallRun()
    {
        if (canWallRun)
        {
            isWallRunning = false;
            Vector3 wallRunDirect = Vector3.zero;
            isWallRight = Physics.Raycast(transform.position, transform.right, out RaycastHit hitWallRight, 1f, WallRunLayer);
            isWallLeft = Physics.Raycast(transform.position, -transform.right, out RaycastHit hitWallLeft, 1f, WallRunLayer);
            isWallFront = Physics.Raycast(transform.position, transform.forward, out RaycastHit hitWallFront, 1f, WallRunLayer);
            isWallBack = Physics.Raycast(transform.position, -transform.forward, out RaycastHit hitWallBack, 1f, WallRunLayer);

            if (isWallRight)
            {
                float wallAngle = Vector3.Angle(hitWallRight.normal, Vector3.up);
                isWallRunning = wallAngle < WallRunMaxAngle;
                wallRunDirect = transform.right + transform.forward;
                prevWallDirect = "right";
                if (isWallRunning && !playerCamera.CameraTiltedLeft)
                {
                    playerCamera.TiltCameraRight = false;
                    playerCamera.TiltCamera();
                }
            }

            if (isWallLeft)
            {
                float wallAngle = Vector3.Angle(hitWallLeft.normal, Vector3.up);
                isWallRunning = wallAngle < WallRunMaxAngle;
                wallRunDirect = -transform.right + transform.forward;
                prevWallDirect = "left";
                if (isWallRunning && !playerCamera.CameraTiltedRight)
                {
                    playerCamera.TiltCameraRight = true;
                    playerCamera.TiltCamera();
                }
            }
            if (isWallFront)
            {
                float wallAngle = Vector3.Angle(hitWallFront.normal, Vector3.up);
                isWallRunning = wallAngle < WallRunMaxAngle;
                wallRunDirect = transform.right * (prevWallDirect == "right" ? -1f : 1f) + transform.forward;
            }
            if (isWallBack)
            {
                float wallAngle = Vector3.Angle(hitWallBack.normal, Vector3.up);
                isWallRunning = wallAngle < WallRunMaxAngle;
                wallRunDirect = transform.right * (prevWallDirect == "right" ? 1f : -1f) + -transform.forward;
            }

            if (isWallRunning)
            {
                Gravity = Vector3.zero;
                velocity = Vector3.zero;
                currentJumpCount = 0;

                movement = wallRunDirect * WallRunSpeed * Time.deltaTime;
                return;
            }
        }
        if(playerCamera.CameraTiltedRight || playerCamera.CameraTiltedLeft) playerCamera.TiltCamera();
        Gravity = new Vector3(0, -19.62f, 0);
    }

    private bool PlayerWallJump()
    {
        if (isWallJumping && (onGround || isWallRunning || isJumping))
        {
            isWallJumping = false;
        }

        if (canWallJump)
        {
            Gravity = new Vector3(0, -19.62f, 0);
            if (CanChangeWallJumpDirect)
            {
                velocity = PlayerCamera.transform.forward * WallJumpForce;
            } else
            {
                velocity = transform.right * (isWallRight ? -1f : 1f) * WallJumpForce;
            }

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

