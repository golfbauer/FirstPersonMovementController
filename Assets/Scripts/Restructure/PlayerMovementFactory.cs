using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    }

    void InitializeWalking()
    {
        Walking walking = gameObject.AddComponent<Walking>();
        walking.MoveSpeed = 40f;
        walking.MoveCap = 5f;
        walking.ActionKeys = new KeyCode[] { };
        walking.SupportedFeatures = new List<string>();
        walking.Identifier = "Walking";
        manager.AddFeature(walking.Identifier, walking);
    }

    void InitializeSprinting()
    {
        Sprinting sprinting = this.AddComponent<Sprinting>();
        sprinting.MoveSpeed = 40f;
        sprinting.MoveCap = 10f;
        sprinting.ActionKeys = new KeyCode[] { KeyCode.LeftShift };
        sprinting.SupportedFeatures = new List<string>();
        sprinting.Identifier = "Sprinting";
        manager.AddFeature(sprinting.Identifier, sprinting);
    }

    void InitializeJumping()
    {
        Jumping jumping = this.AddComponent<Jumping>();
        jumping.MaxJumpCount = 2;
        jumping.JumpForce = new Vector3(0, 10f, 0);
        jumping.JumpCap = 30f;
        jumping.ActionKeys = new KeyCode[] { KeyCode.Space };
        jumping.BreakingFeatures = new List<string>();
        jumping.SupportedFeatures = new List<string>();
        jumping.Identifier = "Jumping";
        manager.AddFeature(jumping.Identifier, jumping);
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
        CameraController cameraController = playerCamera.AddComponent<CameraController>();
        cameraController.PlayerTransform = transform;
        cameraController.MouseSensitivity = 100f;
    }
}
