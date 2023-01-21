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
    public float DistanceToWall { get; set; } = 1f;

    protected WallPosition wallPosition;
    protected WallPosition prevWallPosition;
    protected RaycastHit wallHitInfo;
    protected WallJumping wallJumping;

    protected override void Start()
    {
        base.Start();
        wallJumping = GetComponent<WallJumping>();
    }


    protected override bool CanExecute()
    {
        if(!base.CanExecute()) return false;
        if (!CheckWallHit()) return false;
        if(!CheckWallAngle()) return false;
        if (CheckDistanceToGround()) return false;

        return true;
    }

    protected virtual bool CheckDuringExecution()
    {
        if (!CheckKeys()) return false;
        if (!CheckWallHit()) return false;
        if(!CheckWallAngle()) return false;
        if (!CheckTimeOnWall()) return false;

        return true;
    }

    protected override bool CheckKeys()
    {
        return CheckAllInputGetKeys();
    }

    protected virtual bool CheckWallHit()
    {
        if(wallPosition != WallPosition.None) prevWallPosition = wallPosition;

        if (Physics.Raycast(transform.position, transform.forward, out wallHitInfo, DistanceToWall, LayerMask.GetMask(WallRunLayers)))
        {
            wallPosition = WallPosition.None;
            moveDirect = transform.right * (prevWallPosition == WallPosition.Right ? -1f : 1f) + transform.forward;
            if (prevWallPosition == WallPosition.None) moveDirect = Vector3.zero;
            return true;
        }
        if (Physics.Raycast(transform.position, -transform.forward, out wallHitInfo, DistanceToWall, LayerMask.GetMask(WallRunLayers)))
        {
            wallPosition = WallPosition.None;
            moveDirect = transform.right * (prevWallPosition == WallPosition.Right ? 1f : -1f) + -transform.forward;
            return prevWallPosition != WallPosition.None;
        }
        if (Physics.Raycast(transform.position, transform.right, out wallHitInfo, DistanceToWall, LayerMask.GetMask(WallRunLayers)))
        {
            wallPosition = WallPosition.Right;
            moveDirect = transform.right + transform.forward;
            return true;
        }
        if (Physics.Raycast(transform.position, -transform.right, out wallHitInfo, DistanceToWall, LayerMask.GetMask(WallRunLayers)))
        {
            wallPosition = WallPosition.Left;
            moveDirect = -transform.right + transform.forward;
            return true;
        }

        return false;
    }

    protected virtual bool CheckWallAngle()
    {
        float hitWallAngle = Vector3.Angle(wallHitInfo.normal, Vector3.up);
        return hitWallAngle < MaxWallRunAngle && hitWallAngle > MinWallRunAngle;
    }

    protected virtual bool CheckDistanceToGround()
    {
        return Physics.Raycast(transform.position, -transform.up, DistanceToGround);
    }

    protected virtual bool CheckTimeOnWall()
    {
        if (elapsedSinceStartExecution >= MaxTimeOnWall)
        {
            PushOffWall(wallHitInfo);
            return false;
        }
        return true;
    }

    protected virtual void PushOffWall(RaycastHit hit)
    {
        manager.AddRawVelocity(hit.normal * PushOfWallForce);
    }

    protected override void Init()
    {
        base.Init();
        SetVelocityAtStart();
        ChangeGravityMultiplier(GravityMultiplier);
        wallPosition = prevWallPosition = WallPosition.None;
    }

    protected virtual void SetVelocityAtStart()
    {
        initVelocity.y = 0f;
        manager.SetVelocity(initVelocity);
    }

    protected override void ExecuteAction()
    {
        if(CheckDuringExecution())
        {
            velocity = moveDirect * MoveSpeed;
        } else {
            FinishExecution();
        }

        TiltCamera();
    }

    protected override void FinishExecution()
    {
        IsExecutingAction = false;
        EnableFeatures();
        if(!manager.IsFeatureActive(Features.WallJumping)){
            Vector3 velocity = manager.GetVelocity();
            manager.SetVelocity(velocity.magnitude * transform.forward);
        }
        wallPosition = WallPosition.None;
        UndoChangeGravityMultiplier();
    }

    protected virtual void TiltCamera()
    {
        if(wallPosition == prevWallPosition) return;

        switch(wallPosition){
            case WallPosition.None:
                CameraController.TiltCamera(0, TimeToTiltCamera);
                break;
            case WallPosition.Right:
                CameraController.TiltCamera(CameraTiltAngle, TimeToTiltCamera);
                break;
            case WallPosition.Left:
                CameraController.TiltCamera(-CameraTiltAngle, TimeToTiltCamera);
                break;
        }
    }
}
