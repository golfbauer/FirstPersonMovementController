using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerMovementFactory : MonoBehaviour
{

    [Header("Debugging")]
    [Header("Kinematic Character Controller")]

    [SerializeField] private float slopeLimit;
    [SerializeField] private float stairOffset;
    [SerializeField] private float stairSnapdownDistance;
    [SerializeField] private Vector3 center;
    [SerializeField] private float height;
    [SerializeField] private float radius;
    [SerializeField] private float anglePower;
    [SerializeField] private float maxBounces;

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
    }

    void InitializeWalking()
    {
        Walking walking = gameObject.AddComponent<Walking>();
        walking.MoveSpeed = 40f;
        walking.MoveCap = 5f;
        walking.ActionKeys = new KeyCode[] { };
        walking.Identifier = "Walking";

        manager.AddFeature(walking.Identifier, walking);
    }

    void InitializeSprinting()
    {
        Sprinting sprinting = this.AddComponent<Sprinting>();
        sprinting.MoveSpeed = 40f;
        sprinting.MoveCap = 10f;
        sprinting.ActionKeys = new KeyCode[] { KeyCode.LeftShift };
        sprinting.Identifier = "Sprinting";

        manager.AddFeature(sprinting.Identifier, sprinting);
    }

    void InitializeJumping()
    {
        Jumping jumping = this.AddComponent<Jumping>();
        jumping.MaxJumpCount = 2;
        jumping.JumpHeight = 5f;
        jumping.ActionKeys = new KeyCode[] { KeyCode.Space };
        jumping.Identifier = "Jumping";
        jumping.ExcludingFeatures = new List<string>();
        jumping.ExcludingFeatures.Add("Sliding");
        jumping.ExcludingFeatures.Add("Crouching");

        manager.AddFeature(jumping.Identifier, jumping);
    }

    void InitializeCrouching()
    {
        Crouching crouching = this.AddComponent<Crouching>();
        crouching.ActionKeys = new KeyCode[] { KeyCode.C };
        crouching.Identifier = "Crouching";
        crouching.ExcludingFeatures = new List<string>();
        crouching.ExcludingFeatures.Add("Sprinting");
        crouching.ExcludingFeatures.Add("Sliding");

        crouching.TimeToCrouch = 0.5f;
        crouching.HeightDifference = 0.5f;
        crouching.CameraController = cameraController;

        manager.AddFeature(crouching.Identifier, crouching);
    }

    void InitializeSliding()
    {
        Sliding sliding = this.AddComponent<Sliding>();
        sliding.ActionKeys = new KeyCode[] { KeyCode.C };
        sliding.Identifier = "Sliding";
        sliding.RequiredFeatures = new List<string>();
        sliding.RequiredFeatures.Add("Sprinting");
        sliding.DisableFeatures = new List<string>();
        sliding.DisableFeatures.Add("Walking");
        sliding.DisableFeatures.Add("Sprinting");
        sliding.DisableFeatures.Add("Jumping");
        sliding.CameraController = cameraController;

        sliding.MoveCap = 20f;
        sliding.MoveSpeed = 100f;
        sliding.MoveControl = 0f;
        sliding.MoveTime = 0.75f;
        sliding.CanCancelSlide = true;

        manager.AddFeature(sliding.Identifier, sliding);
    }

    void InitializeDashing()
    {
        Dashing dashing = this.AddComponent<Dashing>();
        dashing.ActionKeys = new KeyCode[] { KeyCode.LeftShift };
        dashing.Identifier = "Dashing";
        dashing.RequiredFeatures = new List<string>();
        dashing.RequiredFeatures.Add("Jumping");
        dashing.RequiredFeatures.Add("WallJumping");
        dashing.DisableFeatures = new List<string>();
        dashing.DisableFeatures.Add("Walking");
        dashing.DisableFeatures.Add("Sprinting");
        dashing.DisableFeatures.Add("Jumping");
        dashing.CameraController = cameraController;

        dashing.MoveCap = 30f;
        dashing.MoveSpeed = 100f;
        dashing.MoveControl = 0f;
        dashing.MoveTime = 0.5f;
        dashing.MaxDashCount = 3;

        manager.AddFeature(dashing.Identifier, dashing);
    }
    void InitializeWallRun()
    {
        WallRunning wallRunning = this.AddComponent<WallRunning>();
        wallRunning.ActionKeys = new KeyCode[] { KeyCode.Space };
        wallRunning.Identifier = "WallRunning";
        wallRunning.RequiredFeatures = new List<string>();
        wallRunning.RequiredFeatures.Add("Jumping");
        wallRunning.DisableFeatures = new List<string>();
        wallRunning.DisableFeatures.Add("Walking");
        wallRunning.DisableFeatures.Add("Sprinting");
        wallRunning.DisableFeatures.Add("Jumping");

        wallRunning.GravityMultiplier = 0;
        wallRunning.MoveCap = 20f;
        wallRunning.MoveSpeed = 30f;
        wallRunning.MaxTimeOnWall = 500;
        wallRunning.MinWallRunAngle = 80;
        wallRunning.MaxWallRunAngle = 100;
        wallRunning.WallRunLayers = new string[] { "WallRun" };
        wallRunning.DistanceToGround = 0;
        wallRunning.TimeToTiltCamera = 0.3f;
        wallRunning.CameraTiltAngle = 15f;

        wallRunning.CameraController = cameraController;
        manager.AddFeature(wallRunning.Identifier, wallRunning);
    }

    void InitializeWallJump()
    {
        WallJumping wallJumping = this.AddComponent<WallJumping>();
        wallJumping.ActionKeys = new KeyCode[] { KeyCode.Space };
        wallJumping.Identifier = "WallJumping";
        wallJumping.RequiredFeatures = new List<string>();
        wallJumping.RequiredFeatures.Add("WallRunning");

        wallJumping.MoveForce = new Vector3(10f, 10f);

        wallJumping.CameraController = cameraController;
        manager.AddFeature(wallJumping.Identifier, wallJumping);
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
