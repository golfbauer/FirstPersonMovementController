using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;

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
    public float MaxTimeOnWall { get; set; } = 1f;
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
    public bool WallJump { get; set; } = false;

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
    private bool canWallRun => (isJumping || isWallRunning) && Input.GetKey(WallRunKey) && timeOnWall < MaxTimeOnWall;
    private bool canWallJump =>isWallRunning && Input.GetKeyUp(WallRunKey);

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
    private string prevWallDirect;
    private Vector3 tempGravity;
    private float timeOnWall;

    private Vector3 velocity;
    private Vector3 movement;

    private KinematicCharacterController controller;
    private PlayerCameraLook playerCamera;

    private void Start()
    {
        controller = GetComponent<KinematicCharacterController>();
        playerCamera = PlayerCamera.GetComponent<PlayerCameraLook>();

        tempGravity = Gravity;
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


    // TODO: 
    // 1. WallRun done by hitting wall, getting loose by pressing jump key
    // 2. Front and Back shouldnt go left or right when hit wall first. Back to wall -> shouldnt work,  Front to  wall -> Stays on wall wihtout moving
    // 3. WallRun needs max timer. Drops after time expired. Can be set to no timer. Should timer be reset when walljump or jump??? : yes it should and push away from wall -> Solution for now
    // 4. WallRun should have value from -1 to 1 to slowly rise or drop while wallrunning. Just multiply Gravity by said value.
    // 5. Add minium hight for wall run -> raycast down

    // 1. WallJump should use hit.normal or tranform.forward && transform.right
    // 2. WallJump facing wall should push backwards a bit
    // 3. Can I combine WallJump and Jump to both apply force? Does this make sense? Or make WallJumpForce into vector to be able to alter y and z.
    // 4. WallJump when hitting end of wall. make it an option


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
                timeOnWall += Time.deltaTime;
                if(timeOnWall > MaxTimeOnWall)
                {
                    WallJump = true;
                }
                if (Gravity != Vector3.zero) tempGravity = Gravity;
                Gravity = Vector3.zero;
                velocity = Vector3.zero;
                currentJumpCount = 0;

                movement = wallRunDirect * WallRunSpeed * Time.deltaTime;
                return;
            }
        }
        if(playerCamera.CameraTiltedRight || playerCamera.CameraTiltedLeft) playerCamera.TiltCamera();
        Gravity = tempGravity;
        if (onGround)
        {
            timeOnWall = 0;
        }
    }

    void PlayerWallJump()
    {
        if (isWallJumping && (onGround || isWallRunning || isJumping))
        {
            isWallJumping = false;
        }

        if (canWallJump || WallJump)
        {
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
            WallJump = false;
        }
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

