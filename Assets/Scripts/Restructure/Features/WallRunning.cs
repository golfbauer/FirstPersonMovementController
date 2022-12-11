using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class WallRunning : PlayerFeature
{
    public float GravityMultiplier { get; set; }
    public float MoveCap { get; set; }
    public float WallRunSpeed { get; set; }
    public float PushOfWallForce { get; set; }
    public float MaxTimeOnWall { get; set; }
    public float MaxWallRunAngle { get; set; }
    public float MinWallRunAngle { get; set; }
    public string[] WallRunLayers { get; set; }
    public float WallRunMinimumHeight { get; set; }
    public float TimeToTiltCamera { get; set; }
    public float CameraTiltAngle { get; set; }
    public CameraController CameraController { get; set; }

    private float prevGravityMultiplier;
    private Vector3 wallRunMoveDirect;
    private WallPosition prevWallPosition;

    private bool isWallRight;
    private bool isWallLeft;
    private bool isWallFront;
    private bool isWallBack;

    new void Start()
    {
        base.Start();
        prevGravityMultiplier = manager.GravityMultiplier;
    }


    public override void CheckAction()
    {
        if (DisableFeature || !CanExecute())
        {
            IsExecutingAction = false;
            prevWallPosition = WallPosition.None;
            ChangeGravityMultiplier(true);
            TiltCamera(true);
        } else
        {
            if (!IsExecutingAction) Init();
            isWallRight = isWallLeft = isWallFront = isWallBack = false;

            IsExecutingAction = true;
            Velocity = ExecuteAction();

            TiltCamera(false);

            manager.AddVelocity(Velocity, MoveCap);
        }

        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        if (!CheckKeyInput()) return false;

        if (!CheckRequiredFeatures()) return false;

        if (!CheckWallHit(out RaycastHit hit)) return false;

        if (!CheckTimeOnWall(hit)) return false;

        // Only Check on first WallContact
        if (!IsExecutingAction)
        {
            if (!CheckDistanceToGround()) return false;
        }

        return true;
    }

    protected override Vector3 ExecuteAction()
    {
        return wallRunMoveDirect * WallRunSpeed;
    }

    protected override void Init()
    {
        ChangeGravityMultiplier(false);
    }

    private void ChangeGravityMultiplier(bool undoGravity)
    {
        if (undoGravity)
        {
            manager.GravityMultiplier = prevGravityMultiplier;
            return;
        }

        prevGravityMultiplier = manager.GravityMultiplier;
        manager.GravityMultiplier = GravityMultiplier;
    }

    private bool CheckKeyInput()
    {
        return CheckInputGetKeys();
    }

    private bool CheckRequiredFeatures()
    {
        List<string> requiredFeatures = new List<string>
        {
            "isJumping"
        };

        return CheckIfFeaturesActive(requiredFeatures);
    }

    private bool CheckTimeOnWall(RaycastHit hit)
    {
        if(ElapsedSinceStartExecution >= MaxTimeOnWall)
        {
            PushOffWall(hit);
            return false;
        }
        return true;
    }

    private bool CheckWallHit(out RaycastHit hit)
    {
        bool isHit = PlayerUpdateWallHit(out hit);

        if (!isHit)
        {
            return false;
        }

        float hitWallAngle = Vector3.Angle(hit.normal, Vector3.up);

        if (hitWallAngle > MaxWallRunAngle || hitWallAngle < MinWallRunAngle)
        {
            return false;
        }

        if (isWallFront)
        {
            wallRunMoveDirect = transform.right * (prevWallPosition == WallPosition.Right ? -1f : 1f) + transform.forward;
            if (prevWallPosition == WallPosition.None) wallRunMoveDirect = Vector3.zero;
        }
        if (isWallBack)
        {
            if (prevWallPosition == WallPosition.None) return false;
            wallRunMoveDirect = transform.right * (prevWallPosition == WallPosition.Right ? 1f : -1f) + -transform.forward;
        }
        if (isWallRight)
        {
            prevWallPosition = WallPosition.Right;
            wallRunMoveDirect = transform.right + transform.forward;
        }
        if (isWallLeft)
        {
            prevWallPosition = WallPosition.Left;
            wallRunMoveDirect = -transform.right + transform.forward;
        }

        return true;
    }

    private bool CheckDistanceToGround()
    {
        return Physics.Raycast(transform.position, -transform.up, WallRunMinimumHeight);
    }

    private bool PlayerUpdateWallHit(out RaycastHit hit)
    {
        isWallRight = Physics.Raycast(transform.position, transform.right, out hit, 1f, LayerMask.GetMask(WallRunLayers));
        if (isWallRight) return true;

        isWallLeft = Physics.Raycast(transform.position, -transform.right, out hit, 1f, LayerMask.GetMask(WallRunLayers));
        if (isWallLeft) return true;

        isWallFront = Physics.Raycast(transform.position, transform.forward, out hit, 1f, LayerMask.GetMask(WallRunLayers));
        if (isWallFront) return true;

        isWallBack = Physics.Raycast(transform.position, -transform.forward, out hit, 1f, LayerMask.GetMask(WallRunLayers));
        if (isWallBack) return true;

        return false;
    }

    private void TiltCamera(bool skip)
    {
        if(skip)
        {
            CameraController.TiltCamera(0, TimeToTiltCamera);
            return;
        }

        if(isWallFront || isWallBack)
        {
            CameraController.TiltCamera(0, TimeToTiltCamera);
            return;
        }
        
        if(isWallRight)
        {
            CameraController.TiltCamera(CameraTiltAngle, TimeToTiltCamera);
            return;
        }

        if(isWallLeft)
        {
            CameraController.TiltCamera(-CameraTiltAngle, TimeToTiltCamera);
            return;
        }
    }

    private void PushOffWall(RaycastHit hit)
    {
        manager.AddVelocity(hit.normal * PushOfWallForce, PushOfWallForce);
    }

    public enum WallPosition
    {
        Right,
        Left,
        Front,
        Back,
        None
    }
}
