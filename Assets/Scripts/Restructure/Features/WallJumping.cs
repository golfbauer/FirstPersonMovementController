using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumping : PlayerFeature
{
    public Vector2 WallJumpForce { get; set; }
    public CameraController CameraController { get; set; }

    public override void CheckAction()
    {
        if(DisableFeature || !CanExecute())
        {
            IsExecutingAction = !CheckIsExecuting();
        } else
        {
            if (!IsExecutingAction) Init();
            Velocity = ExecuteAction();
            manager.AddRawVelocity(Velocity);
            IsExecutingAction = true;
        }

        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        if (!CheckKeyInput()) return false;
        if (!CheckRequiredFeatures()) return false;

        return true;
    }

    protected override Vector3 ExecuteAction()
    {
        Vector3 vel = CameraController.transform.forward;
        vel = new Vector3(vel.x * WallJumpForce.x, vel.y * WallJumpForce.y, vel.z * WallJumpForce.x);

        return vel;
    }

    protected override void Init()
    {
        return;
    }

    private bool CheckIsExecuting()
    {
        List<string> requiredFeatures = new List<string>
        {
            "isJumping",
            "isWallRunning"
        };

        return IsExecutingAction && (!manager.IsGrounded() || CheckIfFeaturesActive(requiredFeatures));
    }

    private bool CheckKeyInput()
    {
        return CheckInputGetKeysUp();
    }

    private bool CheckRequiredFeatures()
    {
        List<string> requiredFeatures = new List<string>
        {
            "isWallRunning"
        };

        return CheckIfFeaturesActive(requiredFeatures);
    }
}
