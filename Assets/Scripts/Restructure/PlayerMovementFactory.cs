using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static Utils;

public class PlayerMovementFactory : MonoBehaviour
{

    [Header("Debugging")]
    [SerializeField] private bool _debug = false;

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
    [SerializeField][OnChangedCall("OnVariableChange")] private float walkSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float walkCap;

    [Header("Sprinting")]
    [SerializeField][OnChangedCall("OnVariableChange")] private float sprintSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float sprintCap;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] sprintKeys;

    [Header("Jumping")]
    [SerializeField][OnChangedCall("OnVariableChange")] private float jumpHeight;
    [SerializeField][OnChangedCall("OnVariableChange")] private int maxJumpCount;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] jumpKeys;

    [Header("Crouching")]
    [SerializeField][OnChangedCall("OnVariableChange")] private float timeToCrouch;
    [SerializeField][OnChangedCall("OnVariableChange")] private float heightDifference;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] crouchKeys;

    [Header("Sliding")]
    [SerializeField][OnChangedCall("OnVariableChange")] private float slideSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float slideCap;
    [SerializeField][OnChangedCall("OnVariableChange")] private float slideControl;
    [SerializeField][OnChangedCall("OnVariableChange")] private float slideTime;
    [SerializeField][OnChangedCall("OnVariableChange")] private bool canCancelSlide;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] slideKeys;

    [Header("Dashing")]
    [SerializeField][OnChangedCall("OnVariableChange")] private float dashSpeed;
    [SerializeField][OnChangedCall("OnVariableChange")] private float dashCap;
    [SerializeField][OnChangedCall("OnVariableChange")] private float dashControl;
    [SerializeField][OnChangedCall("OnVariableChange")] private float dashTime;
    [SerializeField][OnChangedCall("OnVariableChange")] private int maxDashCount;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] dashKeys;

    [Header("WallRunning")]
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
    [SerializeField][OnChangedCall("OnVariableChange")] private Vector2 wallJumpForce;
    [SerializeField][OnChangedCall("OnVariableChange")] private KeyCode[] wallJumpKeys;

    [Header("Components")]
    [SerializeField] private GameObject playerCamera;

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

    private void Awake()
    {
        InitializeKinematicCharacterController();
    }

    // Start is called before the first frame update
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
    }

    public void OnVariableChange()
    {
        if (_debug)
        {
            UpdateWalking();
            UpdateSprinting();
            UpdateJumping();
            UpdateCrouching();
            UpdateSliding();
            UpdateDashing();
            UpdateWallRun();
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
    }

    void InitializeWallRun()
    {
        wallRunning = this.AddComponent<WallRunning>();
        UpdateWallRun();
        manager.AddFeature(wallRunning.Identifier, wallRunning);
    }

    void UpdateWallRun()
    {
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
        Grappling grappling = this.AddComponent<Grappling>();
        grappling.ActionKeys = new KeyCode[] { KeyCode.F };
        grappling.Identifier = "Grappling";
        grappling.DisableFeatures = new List<string>();
        grappling.DisableFeatures.Add("Walking");
        grappling.DisableFeatures.Add("Jumping");
        grappling.DisableFeatures.Add("Dashing");
        grappling.DisableFeatures.Add("WallRunning");

        grappling.MoveCap = 30f;
        grappling.CoolDown = 1f;
        grappling.MaxGrappleDistance = 50f;
        grappling.MoveSpeed = 50f;
        grappling.GrappleLayers = new string[] { "WallRun" };
        grappling.CanCancelExecution = true;

        grappling.CameraController = cameraController;
        manager.AddFeature(grappling.Identifier, grappling);
    }

    void InitializeJetpack()
    {
        Jetpack jetpacking = this.AddComponent<Jetpack>();
        jetpacking.ActionKeys = new KeyCode[] { KeyCode.E };
        jetpacking.Identifier = "Jetpack";
        jetpacking.DisableFeatures = new List<string>();
        jetpacking.DisableFeatures.Add("Sprinting");
        jetpacking.DisableFeatures.Add("Jumping");
        jetpacking.DisableFeatures.Add("Dashing");
        jetpacking.DisableFeatures.Add("WallRunning");
        jetpacking.DisableFeatures.Add("Grappling");

        jetpacking.ExcludingFeatures = new List<string>();
        jetpacking.ExcludingFeatures.Add("WallRunning");
        jetpacking.ExcludingFeatures.Add("Dashing");
        jetpacking.ExcludingFeatures.Add("Grappling");
        jetpacking.ExcludingFeatures.Add("Crouching");
        jetpacking.ExcludingFeatures.Add("Sliding");

        jetpacking.MoveCap = 30f;
        jetpacking.MoveSpeed = 15f;
        jetpacking.TimeToRechargeJetpackFuel = 2f;
        jetpacking.TimeToDepletJetpackFuel = 3f;
        jetpacking.TimeToStartRecharge = 0.3f;
        jetpacking.FallReductionFactor = 10f;

        manager.AddFeature(jetpacking.Identifier, jetpacking);
    }

    void InitializeHeadbob()
    {
        Headbob headbob = this.AddComponent<Headbob>();
        headbob.Identifier = "Headbob";
        headbob.ExcludingFeatures = new List<string>();
        headbob.ExcludingFeatures.Add("Sliding");
        headbob.ExcludingFeatures.Add("Crouching");
        headbob.HeadbobFeatures = new Dictionary<string, Vector2>();
        headbob.HeadbobFeatures.Add("Sprinting", Vector2.zero);
        headbob.HeadbobFeatures.Add("Walking", new Vector2(10, 0.1f));

        headbob.CameraController = cameraController;
        manager.AddFeature(headbob.Identifier, headbob);
    }

    void InitializeManager()
    {
        manager = this.AddComponent<PlayerMovementManager>();
        manager.BaseGravity = new Vector3(0, -9.81f, 0);
        manager.GroundedVelocityDeclineRate = 20f;
        manager.AirborneVelocityDeclineRate = 0;
    }

    void InitializeKinematicCharacterController()
    {
        controller = Utils.CreateKinemeticCharacterController(
            this.gameObject,
            slopeLimit,
            stairOffset,
            stairSnapdownDistance,
            center,
            height,
            radius,
            anglePower,
            maxBounces
        );
    }

    void InitializeCameraController()
    {
        cameraController = playerCamera.AddComponent<CameraController>();
        cameraController.PlayerTransform = transform;
        cameraController.MouseSensitivity = 100f;
    }
}
