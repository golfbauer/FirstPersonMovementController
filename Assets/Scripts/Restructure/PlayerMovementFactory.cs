using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Utils;

public class PlayerMovementFactory : MonoBehaviour
{

    [Header("Debugging")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool _debug = false;
    [SerializeField][OnChangedCall("OnVariableChange")] private bool printDebugInfo = false;

    [Header("Manager")]
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector3 gravity;
    [SerializeField][OnChangedCall("OnVariableChange")] private float groundDrag;
    [SerializeField][OnChangedCall("OnVariableChange")] private float airDrag;

    [Header("Camera")]
    [SerializeField][OnChangedCall("OnVariableChange")] private GameObject playerCamera;
    [SerializeField][OnChangedCall("OnVariableChange")] private float cameraSensitivity;

    [Header("Kinematic Character Controller")]
    [SerializeField][OnChangedCall("OnVariableChange")] private float slopeLimit;
    [SerializeField][OnChangedCall("OnVariableChange")] private float stairOffset;
    [SerializeField][OnChangedCall("OnVariableChange")] private float stairSnapdownDistance;
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector3 center;
    [SerializeField][OnChangedCall("OnVariableChange")] private float height;
    [SerializeField][OnChangedCall("OnVariableChange")] private float radius;
    [SerializeField][OnChangedCall("OnVariableChange")] private float anglePower;
    [SerializeField][OnChangedCall("OnVariableChange")] private float maxBounces;

    [Header("Walking")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableWalk;
    [SerializeField][OnChangedCall("OnVariableChange")] private float walkSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float walkCap;

    [Header("Sprinting")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableSprint;
    [SerializeField][OnChangedCall("OnVariableChange")] private float sprintSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float sprintCap;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] sprintKeys;

    [Header("Jumping")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableJumping;
    [SerializeField][OnChangedCall("OnVariableChange")] private float jumpHeight;
    [SerializeField][OnChangedCall("OnVariableChange")] private int maxJumpCount;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] jumpKeys;

    [Header("Crouching")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableCrouching;
    [SerializeField][OnChangedCall("OnVariableChange")] private float timeToCrouch;
    [SerializeField][OnChangedCall("OnVariableChange")] private float heightDifference;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] crouchKeys;

    [Header("Sliding")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableSliding;
    [SerializeField][OnChangedCall("OnVariableChange")] private float slideSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float slideCap;
    [SerializeField][OnChangedCall("OnVariableChange")] private float slideControl;
    [SerializeField][OnChangedCall("OnVariableChange")] private float slideTime;
    [SerializeField][OnChangedCall("OnVariableChange")] private bool canCancelSlide;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] slideKeys;

    [Header("Dashing")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableDashing;
    [SerializeField][OnChangedCall("OnVariableChange")] private float dashSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float dashCap;
    [SerializeField][OnChangedCall("OnVariableChange")] private float dashControl;
    [SerializeField][OnChangedCall("OnVariableChange")] private float dashTime;
    [SerializeField][OnChangedCall("OnVariableChange")] private int maxDashCount;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] dashKeys;

    [Header("WallRunning")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableWallRunning;
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunGravityMultiplier;
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunCap;
    [SerializeField][OnChangedCall("OnVariableChange")] private float maxTimeOnWall;
    [SerializeField][OnChangedCall("OnVariableChange")] private float minWallRunAngle;
    [SerializeField][OnChangedCall("OnVariableChange")] private float maxWallRunAngle;
    [SerializeField][OnChangedCall("OnVariableChange")] private string[] wallRunLayers;
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunDistanceToGround;
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunTimeToTiltCamera;
    [SerializeField][OnChangedCall("OnVariableChange")] private float wallRunCameraTiltAngle;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] wallRunKeys;

    [Header("WallJumping")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableWallJumping;
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector2 wallJumpForce;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] wallJumpKeys;

    [Header("Grappling")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disbaleGrappling;
    [SerializeField][OnChangedCall("OnVariableChange")] private float grappleSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float grappleCap;
    [SerializeField][OnChangedCall("OnVariableChange")] private float maxGrappleDistance;
    [SerializeField][OnChangedCall("OnVariableChange")] private float grappleCooldown;
    [SerializeField][OnChangedCall("OnVariableChange")] private string[] grappleLayers;
    [SerializeField][OnChangedCall("OnVariableChange")] private bool canCancelGrapple;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] grappleKeys;

    [Header("Jetpack")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableJetpack;
    [SerializeField][OnChangedCall("OnVariableChange")] private float jetpackSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float jetpackCap;
    [SerializeField][OnChangedCall("OnVariableChange")] private float timeToRechargeJetpack;
    [SerializeField][OnChangedCall("OnVariableChange")] private float timeToDepletJetpack;
    [SerializeField][OnChangedCall("OnVariableChange")] private float timeToStartRecharge;
    [SerializeField][OnChangedCall("OnVariableChange")] private float fallReductionFactor;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] jetpackKeys;

    [Header("Headbob")]
    [SerializeField][OnChangedCall("OnVariableChange")] private bool disableHeadbob;

    private PlayerMovementManager manager;
    private KinematicCharacterController controller;
    private CameraController cameraController;
    public KinematicCharacterController Controller
    {
        get
        {
            return controller;
        }
    }

    private Walking walking;
    private Sprinting sprinting;
    private Jumping jumping;
    private Crouching crouching;
    private Sliding sliding;
    private Dashing dashing;
    private WallRunning wallRunning;
    private WallJumping wallJumping;
    private Grappling grappling;
    private Jetpack jetpacking;
    private Headbob headbob;
    private bool isRunning;

    private void Awake()
    {
        InitializeKinematicCharacterController();
        isRunning = true;
    }

    void Start()
    {
        InitializeCameraController();
        InitializeManager();

        InitializeWalking();
        InitializeSprinting();
        InitializeJumping();
        InitializeCrouching();
        InitializeSliding();
        InitializeDashing();
        InitializeWallJump();
        InitializeWallRun();
        InitializeGrapple();
        InitializeJetpack();
        InitializeHeadbob();
    }

    public void OnVariableChange()
    {
        if (_debug && isRunning)
        {
            UpdateWalking();
            UpdateSprinting();
            UpdateJumping();
            UpdateCrouching();
            UpdateSliding();
            UpdateDashing();
            UpdateWallRun();
            UpdateWallJump();
            UpdateGrapple();
            UpdateJetpack();
            UpdateHeadbob();
            UpdateManager();
            UpdateCameraController();
            UpdateKinematicCharacterController();
        }
    }

    void InitializeWalking()
    {
        walking = gameObject.AddComponent<Walking>();
        UpdateWalking();
        manager.AddFeature(walking.Identifier, walking);
    }

    void UpdateWalking()
    {
        walking.Disabled = disableWalk;
        walking.MoveSpeed = walkSpeed;
        walking.MoveCap = walkCap;
        walking.Identifier = Features.Walking;
    }

    void InitializeSprinting()
    {
        sprinting = this.AddComponent<Sprinting>();
        UpdateSprinting();
        manager.AddFeature(sprinting.Identifier, sprinting);
    }

    void UpdateSprinting()
    {
        sprinting.Disabled = disableSprint;
        sprinting.MoveSpeed = sprintSpeed;
        sprinting.MoveCap = sprintCap;
        sprinting.ActionKeys = sprintKeys;
        sprinting.Identifier = Features.Sprinting;
    }

    void InitializeJumping()
    {
        jumping = this.AddComponent<Jumping>();
        UpdateJumping();
        manager.AddFeature(jumping.Identifier, jumping);
    }

    void UpdateJumping()
    {
        jumping.Disabled = disableJumping;
        jumping.MaxJumpCount = maxJumpCount;
        jumping.JumpHeight = jumpHeight;
        jumping.ActionKeys = jumpKeys;
        jumping.Identifier = Features.Jumping;
        jumping.ExcludingFeatures = new List<string>
        {
            Features.Sliding,
            Features.Crouching
        };
    }

    void InitializeCrouching()
    {
        crouching = this.AddComponent<Crouching>();
        UpdateCrouching();
        manager.AddFeature(crouching.Identifier, crouching);
    }

    void UpdateCrouching()
    {
        crouching.Disabled = disableCrouching;
        crouching.ActionKeys = crouchKeys;
        crouching.Identifier = Features.Crouching;
        crouching.ExcludingFeatures = new List<string>
        {
            Features.Sprinting,
            Features.Sliding
        };

        crouching.TimeToCrouch = timeToCrouch;
        crouching.HeightDifference = heightDifference;
        crouching.CameraController = cameraController;
    }

    void InitializeSliding()
    {
        sliding = this.AddComponent<Sliding>();
        UpdateSliding();
        manager.AddFeature(sliding.Identifier, sliding);
    }

    void UpdateSliding()
    {
        sliding.Disabled = disableSliding;
        sliding.ActionKeys = slideKeys;
        sliding.Identifier = Features.Sliding;
        sliding.RequiredFeatures = new List<string>
        {
            Features.Sprinting
        };
        sliding.DisableFeatures = new List<string>
        {
            Features.Walking,
            Features.Sprinting,
            Features.Jumping
        };

        sliding.MoveCap = slideCap;
        sliding.MoveSpeed = slideSpeed;
        sliding.MoveControl = slideControl;
        sliding.MoveTime = slideTime;
        sliding.CanCancelSlide = canCancelSlide;
        sliding.CameraController = cameraController;
    }

    void InitializeDashing()
    {
        dashing = this.AddComponent<Dashing>();
        UpdateDashing();
        manager.AddFeature(dashing.Identifier, dashing);
    }

    void UpdateDashing()
    {
        dashing.Disabled = disableDashing;
        dashing.ActionKeys = dashKeys;
        dashing.Identifier = Features.Dashing;
        dashing.RequiredFeatures = new List<string>
        {
            Features.Jumping,
            Features.WallJumping
        };
        dashing.DisableFeatures = new List<string>
        {
            Features.Walking,
            Features.Sprinting,
            Features.Jumping
        };
        dashing.CameraController = cameraController;

        dashing.MoveCap = dashCap;
        dashing.MoveSpeed = dashSpeed;
        dashing.MoveControl = dashControl;
        dashing.MoveTime = dashTime;
        dashing.MaxDashCount = maxDashCount;
        dashing.GravityMultiplier = 0;
    }

    void InitializeWallRun()
    {
        wallRunning = this.AddComponent<WallRunning>();
        UpdateWallRun();
        manager.AddFeature(wallRunning.Identifier, wallRunning);
    }

    void UpdateWallRun()
    {
        wallRunning.Disabled = disableWallRunning;
        wallRunning.ActionKeys = wallRunKeys;
        wallRunning.Identifier = Features.WallRunning;
        wallRunning.RequiredFeatures = new List<string>
        {
            Features.Jumping
        };
        wallRunning.DisableFeatures = new List<string>
        {
            Features.Walking,
            Features.Sprinting,
            Features.Jumping
        };

        wallRunning.GravityMultiplier = wallRunGravityMultiplier;
        wallRunning.MoveCap = wallRunCap;
        wallRunning.MoveSpeed = wallRunSpeed;
        wallRunning.MaxTimeOnWall = maxTimeOnWall;
        wallRunning.MinWallRunAngle = minWallRunAngle;
        wallRunning.MaxWallRunAngle = maxWallRunAngle;
        wallRunning.WallRunLayers = wallRunLayers;
        wallRunning.DistanceToGround = wallRunDistanceToGround;
        wallRunning.TimeToTiltCamera = wallRunTimeToTiltCamera;
        wallRunning.CameraTiltAngle = wallRunCameraTiltAngle;

        wallRunning.CameraController = cameraController;
    }

    void InitializeWallJump()
    {
        wallJumping = this.AddComponent<WallJumping>();
        UpdateWallJump();
        manager.AddFeature(wallJumping.Identifier, wallJumping);
    }

    void UpdateWallJump()
    {
        wallJumping.Disabled = disableWallJumping;
        wallJumping.ActionKeys = wallJumpKeys;
        wallJumping.Identifier = Features.WallJumping;
        wallJumping.RequiredFeatures = new List<string>
        {
            Features.WallRunning
        };

        wallJumping.MoveForce = wallJumpForce;

        wallJumping.CameraController = cameraController;
    }

    void InitializeGrapple()
    {
        grappling = this.AddComponent<Grappling>();
        UpdateGrapple();
        manager.AddFeature(grappling.Identifier, grappling);
    }

    void UpdateGrapple()
    {
        grappling.Disabled = disbaleGrappling;
        grappling.ActionKeys = grappleKeys;
        grappling.Identifier = Features.Grappling;
        grappling.DisableFeatures = new List<string>
        {
            Features.Walking,
            Features.Sprinting,
            Features.Dashing,
            Features.WallRunning,
        };

        grappling.MoveCap = grappleCap;
        grappling.CoolDown = grappleCooldown;
        grappling.MaxGrappleDistance = maxGrappleDistance;
        grappling.MoveSpeed = grappleSpeed;
        grappling.GrappleLayers = grappleLayers;
        grappling.CanCancelExecution = canCancelGrapple;

        grappling.CameraController = cameraController;
    }

    void InitializeJetpack()
    {
        jetpacking = this.AddComponent<Jetpack>();
        UpdateJetpack();
        manager.AddFeature(jetpacking.Identifier, jetpacking);
    }

    void UpdateJetpack()
    {
        jetpacking.Disabled = disableJetpack;
        jetpacking.ActionKeys = jetpackKeys;
        jetpacking.Identifier = Features.Jetpack;
        jetpacking.DisableFeatures = new List<string>
        {
            Features.Sprinting,
            Features.Jumping,
            Features.Dashing,
            Features.WallRunning,
            Features.Grappling
        };

        jetpacking.ExcludingFeatures = new List<string>
        {
            Features.WallRunning,
            Features.Dashing,
            Features.Grappling,
            Features.Crouching,
            Features.Sliding
        };

        jetpacking.MoveCap = jetpackCap;
        jetpacking.MoveSpeed = jetpackSpeed;
        jetpacking.TimeToRechargeJetpackFuel = timeToRechargeJetpack;
        jetpacking.TimeToDepletJetpackFuel = timeToDepletJetpack;
        jetpacking.TimeToStartRecharge = timeToStartRecharge;
        jetpacking.FallReductionFactor = fallReductionFactor;
    }

    void InitializeHeadbob()
    {
        headbob = this.AddComponent<Headbob>();
        UpdateHeadbob();
        manager.AddFeature(headbob.Identifier, headbob);
    }

    void UpdateHeadbob()
    {
        headbob.Disabled = disableHeadbob;
        headbob.Identifier = Features.Headbob;
        headbob.ExcludingFeatures = new List<string>
        {
            Features.Sliding,
            Features.Crouching
        };
        headbob.HeadbobFeatures = new Dictionary<string, Vector2>
        {
            { Features.Sprinting, Vector2.zero },
            { Features.Walking, new Vector2(10, 0.1f) }
        };

        headbob.CameraController = cameraController;
    }

    void InitializeManager()
    {
        manager = this.AddComponent<PlayerMovementManager>();
        UpdateManager();
    }

    void UpdateManager()
    {
        manager.BaseGravity = gravity;
        manager.GroundedVelocityDeclineRate = groundDrag;
        manager.AirborneVelocityDeclineRate = airDrag;
        manager.PrintDebugInfo = printDebugInfo;
    }

    void InitializeKinematicCharacterController()
    {
        controller = this.AddComponent<KinematicCharacterController>();
        UpdateKinematicCharacterController();
    }

    void UpdateKinematicCharacterController()
    {
        controller.SlopeLimit = slopeLimit;
        controller.StairOffset = stairOffset;
        controller.StairSnapdownDistance = stairSnapdownDistance;
        controller.Center = center;
        controller.Height = height;
        controller.Radius = radius;
        controller.AnglePower = anglePower;
        controller.MaxBounces = maxBounces;
    }

    void InitializeCameraController()
    {
        cameraController = playerCamera.AddComponent<CameraController>();
        UpdateCameraController();
    }

    void UpdateCameraController()
    {
        cameraController.PlayerTransform = transform;
        cameraController.MouseSensitivity = cameraSensitivity;
    }
}
