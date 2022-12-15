using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using static Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using System.Net;

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
    public float WallRunSpeed { get; set; }
    public float WallRunMaxAngle { get; set; }
    public float WallPushForce { get; set; }
    public float MaxTimeOnWall { get; set; }
    public float WallRunGravityMultiplier { get; set; }
    public float WallRunMinimumHeight { get; set; }
    public float MaxGrappleDistance { get; set; }
    public float GrappleCoolDown { get; set; }
    public float GrappleSpeed { get; set; }
    public float MaxDashTime { get; set; }
    public float DashControll { get; set; }
    public float DashSpeed { get; set; }

    public int CountAllowedJumps { get; set; }
    public int WallRunLayer { get; set; }
    public int GrappleLayer { get; set; }
    public int MaxDashCount { get; set; }

    public Vector2 WallJumpForce { get; set; }
    public Vector2 CrouchHeadBobWalk { get; set; }
    public Vector2 CrouchHeadBobSprint { get; set; }
    public Vector2 CrouchHeadBobDefault { get; set; }
    public Vector2 SprintHeadBob { get; set; }
    public Vector2 WalkHeadBob { get; set; }
    public Vector2 DefaultHeadBob { get; set; }

    public bool CanCancelSlide { get; set; }
    public bool CanCancelGrapple { get; set; }
    public bool CanChangeWallJumpDirect { get; set; }

    public KeyCode SprintKey { get; set; }
    public KeyCode JumpKey { get; set; }
    public KeyCode CrouchKey { get; set; }
    public KeyCode SlideKey { get; set; }
    public KeyCode WallRunKey { get; set; }
    public KeyCode WallJumpKey { get; set; }
    public KeyCode GrappleKey { get; set; }
    public KeyCode DashKey { get; set; }

    public Vector3 CrouchingCenter { get; set; }
    public Vector3 StandingCenter { get; set; }
    public Vector3 Gravity { get; set; }

    public GameObject PlayerCamera { get; set; }

    // Public variables that can be set via Scripts
    public bool Jump { get; set; } = false;
    public bool Slide { get; set; } = false;
    public bool Crouch { get; set; } = false;
    public bool WallRun { get; set; } = false;
    public bool WallJump { get; set; } = false;

    //Set to true if player is doing the action
    private bool isCrouching;
    private bool isSliding;
    private bool isSprinting;
    private bool isWalking;
    private bool isJumping;
    private bool isWallRunning;
    private bool isWallJumping;
    public bool isGrappling;
    private bool isDashing;

    private bool onGround;
    private bool crouched;
    private bool projectOnPlane => onGround && !isGrappling;
    public bool grapplingAnimation;

    //Check whether player is allowed to perform action
    private bool canCrouch => onGround && !isSliding && Input.GetKeyDown(CrouchKey);
    private bool canJump =>
        (!(currentJumpCount == 0 && !onGround) &&
        currentJumpCount < CountAllowedJumps &&
        !isCrouching && !isSliding) && Input.GetKeyDown(JumpKey);
    private bool canSprint => onGround && Input.GetKey(SprintKey);
    private bool canSlide => isSprinting && !crouched && !isSliding && Input.GetKeyDown(SlideKey);
    private bool cancelSlide => CanCancelSlide && Input.GetKeyDown(SlideKey);
    private bool canWallRun => PlayerCanWallRun();
    private bool canWallJump => PlayerCanWallJump();
    private bool canHeadbob => onGround && !isSliding;
    private bool canGrapple => PlayerCanGrapple();
    private bool canDash => (isJumping || isWallJumping) && !onGround && !isDashing && Input.GetKeyDown(DashKey) && currentDashCount < MaxDashCount;

    private float elapsedSinceJump;
    private float elapsedSinceNotOnGround;
    private float elapsedSinceGrapple;

    private float grappleTime;

    private int currentJumpCount;

    private float slideX;
    private float slideZ;
    private float slideTimeElapsed;
    private Vector3 slideDirect;

    private bool isWallRight;
    private RaycastHit hitWallRight;
    private bool isWallLeft;
    private RaycastHit hitWallLeft;
    private bool isWallFront;
    private RaycastHit hitWallFront;
    private bool isWallBack;
    private RaycastHit hitWallBack;

    private float hitWallAngle;
    private Vector3 wallRunMoveDirect;
    private WallRunDirect prevWallDirect;
    private Vector3 prevGravity;
    private float timeOnWall;

    private float headBobTimer;
    private float prevBobSpeed;
    private float prevBobAmount;
    private bool prevIsCameraTop;

    public RaycastHit grappleHit;
    public Vector3 localGrappleHitPoint;
    private Vector3 grappleMoveDirect;

    private int currentDashCount;
    private float currentDashTime;
    private float dashX;
    private float dashZ;
    private Vector3 dashDirect;

    private Vector3 velocity;
    private Vector3 movement;

    private EnvironmentController dynamicGroundController;

    private KinematicCharacterController controller;
    private PlayerCameraLook playerCamera;

    private void Start()
    {
        controller = GetComponent<KinematicCharacterController>();
        playerCamera = PlayerCamera.GetComponent<PlayerCameraLook>();

        prevGravity = Gravity;
        WallRunLayer = 1 << WallRunLayer;
        GrappleLayer = 1 << GrappleLayer;
    }

    void Update()
    {
        Debug.Log(transform.position);
        if (transform.position.y <= - 100)
        {
            transform.position = new Vector3(138.5f, 1.5f, 0f);
        }

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

        PlayerMove();

        PlayerJump();

        PlayerWallRun();

        PlayerWallJump();

        PlayerCrouch();

        PlayerSlide();

        PlayerDash();

        PlayerGrapple();

        PlayerHeadBob();

        if (projectOnPlane)
        {
            movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
        }

        // Check if Player is standing on a moving plattform and add the plattform movement to player movement if so
        if (groundHit.collider)
        {
            dynamicGroundController = groundHit.collider.gameObject.GetComponent<EnvironmentController>();
            if (dynamicGroundController != null)
            {
                if (dynamicGroundController.isMoving)
                {
                    movement += dynamicGroundController.MovementDelta;
                }
                if (dynamicGroundController.isRotating)
                {
                    transform.RotateAround(dynamicGroundController.RotationPoint, Vector3.up, dynamicGroundController.DegreesPerSecond * Time.deltaTime);
                }
            }

        }

        transform.position = controller.MovePlayer(movement);

        // If player was not falling before movement but is falling after movement see if he should be snapped down
        if (onGround && !controller.CheckGrounded(out RaycastHit groundHitAfterMove)) 
        {
            if (controller.SnapDown(transform.position, transform.rotation))
            {
                velocity += Gravity;
            }
        }

        // Move player based on falling speed
        transform.position = controller.MovePlayer(velocity * Time.deltaTime);
    }

    void PlayerMove()
    {

        if (canSprint)
        {
            movement = MoveDirect(SprintSpeed);
            isSprinting = true;
            isWalking = false;
        } else
        {
            movement = MoveDirect(MoveSpeed);
            isWalking = true;
            isSprinting = false;
        }

        if(movement == Vector3.zero)
        {
            isSprinting = false;
            isWalking = false;
        }
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

    void PlayerWallRun()
    {
        if (canWallRun || WallRun)
        {
            isWallRunning = true;
            timeOnWall += Time.deltaTime;
            Gravity = prevGravity * WallRunGravityMultiplier;

            TiltCameraOnWallRun();
            movement = wallRunMoveDirect * WallRunSpeed * Time.deltaTime;

        } else
        {
            isWallRunning = false;
            WallRun = false;
            prevWallDirect = WallRunDirect.Stop;
            UndoChangeGravity();
            if (playerCamera.CameraTiltedRight || playerCamera.CameraTiltedLeft) 
            { 
                playerCamera.TiltCamera(); 
            }
            if(onGround || isWallJumping) timeOnWall = 0;
        }
    }

    void TiltCameraOnWallRun()
    {
        if (isWallRight)
        {
            playerCamera.TiltLeft();
            return;
        }
        if (isWallLeft)
        {
            playerCamera.TiltRight();
            return;
        }
    }

    bool PlayerCanWallRun()
    {
        bool checkControls = WallRunKey == KeyCode.None ? !Input.GetKeyDown(WallJumpKey) : Input.GetKey(WallRunKey);

        bool canWallRun = (isJumping || isWallJumping || isWallRunning) && checkControls && timeOnWall < MaxTimeOnWall;
        if (!canWallRun) return false;

        PlayerUpdateWallHit();

        hitWallAngle = 0;

        if (isWallFront)
        {
            hitWallAngle = Vector3.Angle(hitWallFront.normal, Vector3.up);
            wallRunMoveDirect = transform.right * (prevWallDirect == WallRunDirect.Right ? -1f : 1f) + transform.forward;
            if (prevWallDirect == WallRunDirect.Stop) wallRunMoveDirect = Vector3.zero;
        }
        if (isWallBack)
        {
            hitWallAngle = Vector3.Angle(hitWallBack.normal, Vector3.up);
            if (prevWallDirect == WallRunDirect.Stop) return false;
            wallRunMoveDirect = transform.right * (prevWallDirect == WallRunDirect.Right ? 1f : -1f) + -transform.forward;
        }
        if (isWallRight)
        {
            hitWallAngle = Vector3.Angle(hitWallRight.normal, Vector3.up);
            prevWallDirect = WallRunDirect.Right;
            wallRunMoveDirect = transform.right + transform.forward;
        }
        if (isWallLeft)
        {
            hitWallAngle = Vector3.Angle(hitWallLeft.normal, Vector3.up);
            prevWallDirect = WallRunDirect.Left;
            wallRunMoveDirect = -transform.right + transform.forward;
        }
        canWallRun = hitWallAngle > 0 ? hitWallAngle < WallRunMaxAngle : false;

        if(canWallRun && !isWallRunning)
        {
            canWallRun = InitializeWallRun();
        }

        return canWallRun;
    }

    bool InitializeWallRun()
    {
        currentJumpCount = 0;
        TempChangeGravity(Gravity * WallRunGravityMultiplier);
        velocity = Vector3.zero;

        bool isToCloseToGround = Physics.Raycast(transform.position, -transform.up, WallRunMinimumHeight + controller.Height);

        if (isToCloseToGround && !isWallJumping) return false;

        return true;
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
                velocity = PlayerCamera.transform.forward * WallJumpForce.x;
            } else
            {
                Vector3 hitNormal = isWallRight ? hitWallRight.normal 
                    : isWallLeft ? hitWallLeft.normal 
                    : isWallFront ? hitWallFront.normal 
                    : isWallBack ? hitWallBack.normal 
                    : transform.forward;
                velocity = hitNormal * WallJumpForce.x;
            }

            velocity.y = Mathf.Sqrt(WallJumpForce.y * -3.0f * Gravity.y);
            currentJumpCount++;
            elapsedSinceJump = 0;
            isWallJumping = true;
            WallJump = false;
        }
    }

    bool PlayerCanWallJump()
    {
        if(timeOnWall > MaxTimeOnWall)
        {
            if (isWallRunning) PushOffWall();
            return false;
        }
        bool canWallJump = WallRunKey == KeyCode.None ? Input.GetKeyDown(WallJumpKey) : Input.GetKeyUp(WallRunKey);
        if (!canWallJump) return false;

        PlayerUpdateWallHit();

        return isWallRight || isWallLeft || isWallFront || isWallBack;
    }

    void PushOffWall()
    {
        Vector3 hitNormal = isWallRight ? hitWallRight.normal
                    : isWallLeft ? hitWallLeft.normal
                    : isWallFront ? hitWallFront.normal
                    : isWallBack ? hitWallBack.normal
                    : transform.forward;
        velocity = hitNormal * WallPushForce;
    }

    void PlayerUpdateWallHit()
    {
        isWallRight = Physics.Raycast(transform.position, transform.right, out hitWallRight, 1f, WallRunLayer);
        isWallLeft = Physics.Raycast(transform.position, -transform.right, out hitWallLeft, 1f, WallRunLayer);
        isWallFront = Physics.Raycast(transform.position, transform.forward, out hitWallFront, 1f, WallRunLayer);
        isWallBack = Physics.Raycast(transform.position, -transform.forward, out hitWallBack, 1f, WallRunLayer);
    }

    void PlayerGrapple()
    {
        if (canGrapple || isGrappling)
        {
            isGrappling = true;
            grappleTime += Time.deltaTime;
            if (grapplingAnimation)
            {
                movement = Vector3.zero;
                return;
            }
            if (controller.CheckObjectHit(grappleMoveDirect))
            {
                isJumping = true;
                isGrappling = false;
                return;
            }
            grappleMoveDirect = (grappleHit.collider.transform.position + localGrappleHitPoint - transform.position).normalized;
            movement = grappleMoveDirect * GrappleForceFunction () * GrappleSpeed * Time.deltaTime;
            return;
        }

        UndoChangeGravity();
        isGrappling = false;
        ResetGrappleTime();
        elapsedSinceGrapple += Time.deltaTime;
    }

    bool PlayerCanGrapple()
    {
        if(CanCancelGrapple && isGrappling && !grapplingAnimation && Input.GetKeyDown(GrappleKey))
        {
            isGrappling = false;
            currentJumpCount = 1;
            return false;
        }
        if (Input.GetKeyDown(GrappleKey) && elapsedSinceGrapple > GrappleCoolDown)
        {
            Transform camera = playerCamera.transform;
            if (Physics.Raycast(camera.position, camera.forward, out grappleHit, MaxGrappleDistance, GrappleLayer))
            {
                InitializeGrapple();
                return true;
            }
        }

        return false;
    }

    void InitializeGrapple()
    {
        localGrappleHitPoint = grappleHit.point - grappleHit.collider.transform.position;
        grapplingAnimation = true;
        TempChangeGravity(Vector3.zero);
        velocity = Vector3.zero;
    }

    void ResetGrappleTime()
    {
        grappleTime = 1f;
    }

    float GrappleForceFunction()
    {
        return grappleTime * grappleTime * grappleTime;
    }

    void TempChangeGravity(Vector3 gravity)
    {
        if(Gravity == prevGravity)
        {
            prevGravity = Gravity;
            Gravity = gravity;
        }
    }

    void UndoChangeGravity()
    {
        if(!isGrappling && !isWallRunning && !isDashing) Gravity = prevGravity;
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
            slideTimeElapsed = 0;

            isSliding = true;
            Slide = false;
        }

        if (isSliding)
        {
            if (cancelSlide && slideTimeElapsed > 0)
            {
                isSliding = false;
                Crouch = true;
                return;
            }

            if (slideTimeElapsed < TimeSlide)
            {
                Vector3 currentSlideDirect =
                    slideDirect * (1f - SlideControl) +
                    (SlideControl * (slideX * transform.right + transform.forward * slideZ));
                movement = currentSlideDirect * SlideSpeed * Time.deltaTime;

                slideTimeElapsed += Time.deltaTime;
                return;
            }
            isSliding = false;
            Crouch = true;
        }
    }

    void PlayerDash()
    {
        if (canDash)
        {
            dashX = Input.GetAxis("Horizontal");
            dashZ = Input.GetAxis("Vertical");
            dashDirect = transform.right * dashX + transform.forward * dashZ;
            if(dashDirect == Vector3.zero)
            {
                dashDirect = transform.forward;
            }
            currentDashTime = 0;
            TempChangeGravity(Vector3.zero);
            velocity = Vector3.zero;

            isDashing = true;
            isJumping = false;
        }

        if (isDashing)
        {
            if (currentDashTime < MaxDashTime)
            {
                Vector3 currentDashDirect =
                    dashDirect * (1f - DashControll) +
                    (DashControll * (dashX * transform.right + transform.forward * dashZ));
                movement = currentDashDirect * DashSpeed * Time.deltaTime;

                currentDashTime += Time.deltaTime;
                return;
            }
            isDashing = false;
            isJumping = true;
            currentDashCount++;
            UndoChangeGravity();
        }
        ResetDash();
    }

    void ResetDash()
    {
        if(onGround || isWallRunning || isGrappling)
        {
            currentDashCount = 0;
        }
    }

    void PlayerHeadBob()
    {
        if (canHeadbob)
        {
            Vector2 headBobFactors = GetHeadBobFactors();

            float bobSpeed = headBobFactors.x;
            float bobAmount = headBobFactors.y;

            if (headBobFactors == Vector2.zero) 
            { 
                FinishPlayerHeadBob();
            } else
            {
                prevBobAmount = bobAmount;
                prevBobSpeed = bobSpeed;
                prevIsCameraTop = playerCamera.transform.localPosition.y > playerCamera.defaultPositionY;

                headBobTimer += Time.deltaTime * bobSpeed;
                playerCamera.HeadBobCamera(headBobTimer, bobAmount);
            }
            return;
        }
        headBobTimer = 0f;
    }

    void FinishPlayerHeadBob()
    {
        if(
            (prevIsCameraTop && playerCamera.transform.localPosition.y <= playerCamera.defaultPositionY) || 
            (!prevIsCameraTop && playerCamera.transform.localPosition.y >= playerCamera.defaultPositionY))
        {
            headBobTimer = 0f;
            return;
        }
        if (headBobTimer / Mathf.PI > (int)Math.Round(headBobTimer / Mathf.PI))
        {
            float interfaceWithx = (int)Math.Round(headBobTimer / Mathf.PI) * Mathf.PI + Mathf.PI;
            headBobTimer = interfaceWithx - ((headBobTimer / Mathf.PI) % 1) * Mathf.PI;
        }
        headBobTimer += Time.deltaTime * prevBobSpeed;
        playerCamera.HeadBobCamera(headBobTimer, prevBobAmount);
    }

    public Vector2 GetHeadBobFactors()
    {
        if (crouched)
            {
                return isWalking ? CrouchHeadBobWalk : isSprinting ? CrouchHeadBobSprint : CrouchHeadBobDefault;
            }
            return isWalking ? WalkHeadBob : isSprinting ? SprintHeadBob : DefaultHeadBob;
        }
}

