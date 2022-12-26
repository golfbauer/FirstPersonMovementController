using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class WallRunning : PlayerFeatureExecuteOverTime
{
    public float PushOfWallForce { get; set; }
    public float MaxTimeOnWall { get; set; }
    public float MaxWallRunAngle { get; set; }
    public float MinWallRunAngle { get; set; }
    public string[] WallRunLayers { get; set; }
    public float TimeToTiltCamera { get; set; }
    public float CameraTiltAngle { get; set; }
    public float DistanceToGround { get; set; }

    private float savedGravityMultiplier;
    private Vector3 wallRunMoveDirect;
    private WallPosition prevWallPosition;

    private bool isWallRight;
    private bool isWallLeft;
    private bool isWallFront;
    private bool isWallBack;

    new void Start()
    {
        base.Start();
        savedGravityMultiplier = manager.GravityMultiplier;
    }

    public override void CheckAction()
    {
        if (!Disabled && CanExecute())
        {
            Init();
        }

        if (IsExecutingAction)
        {
            isWallRight = isWallLeft = isWallFront = isWallBack = false;
            ExecuteAction();
            manager.AddVelocity(velocity, MoveCap);
            TiltCamera(false);
        } else
        {
            TiltCamera(true);
        }

        UpdateElapsedSince();
    }

    new protected bool CanExecute()
    {
        if(IsExecutingAction)
        {
            return false;
        }
        if (!CheckKeys()) return false;
        if (!CheckRequiredFeatures()) return false;
        if (!CheckWallHit(out RaycastHit hit)) return false;
        if (CheckDistanceToGround()) return false;

        return true;
    }

    protected override void Init()
    {
        Vector3 managerVelocity = manager.GetVelocity();
        managerVelocity.y = 0f;
        manager.SetVelocity(managerVelocity);

        DisableGivenFeatures();

        ChangeGravityMultiplier();

        IsExecutingAction = true;
    }

    protected bool CheckStillExecuting()
    {
        if (!CheckKeys()) return false;
        if (!CheckWallHit(out RaycastHit hit)) return false;
        if (!CheckTimeOnWall(hit)) return false;

        return true;
    }

    protected override bool CheckKeys()
    {
        return CheckAllInputGetKeys();
    }

    private bool CheckTimeOnWall(RaycastHit hit)
    {
        if (elapsedSinceStartExecution >= MaxTimeOnWall)
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
        return Physics.Raycast(transform.position, -transform.up, DistanceToGround);
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

    protected override void ExecuteAction()
    {
        if(CheckStillExecuting())
        {
            velocity = wallRunMoveDirect * MoveSpeed;
            return;
        }

        FinishExecution();
    }

    protected override void FinishExecution()
    {
        IsExecutingAction = false;
        EnableFeatures();
        prevWallPosition = WallPosition.None;
        UndoChangeGravityMultiplier();
    }

    private void ChangeGravityMultiplier()
    {
        savedGravityMultiplier = manager.GravityMultiplier;
        manager.GravityMultiplier = 0f;
    }

    private void UndoChangeGravityMultiplier()
    {
        manager.GravityMultiplier = savedGravityMultiplier;
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
        manager.AddRawVelocity(hit.normal * PushOfWallForce);
    }

    public enum WallPosition
    {
        Right,
        Left,
        None
    }
}
