using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumping : PlayerFeature
{
    public Vector2 WallJumpForce { get; set; }
    public CameraController CameraController { get; set; }

    public override void CheckAction()
    {
        if(Disabled || !CanExecute())
        {
            IsExecutingAction = !CheckIsExecuting();
        } else
        {
            if (!IsExecutingAction) Init();
            ExecuteAction();
            manager.AddRawVelocity(velocity);
            IsExecutingAction = true;
        }

        UpdateElapsedSince();
    }

    new protected bool CanExecute()
    {
        if (!CheckKeyInput()) return false;
        if (!CheckRequiredFeatures()) return false;

        return true;
    }

    new protected void ExecuteAction()
    {
        Vector3 vel = CameraController.transform.forward;
        vel = new Vector3(vel.x * WallJumpForce.x, vel.y * WallJumpForce.y, vel.z * WallJumpForce.x);

        velocity = vel;
    }

    private bool CheckIsExecuting()
    {
        List<string> requiredFeatures = new List<string>
        {
            "isJumping",
            "isWallRunning"
        };

        return IsExecutingAction && (!manager.IsGrounded() || CheckIfFeatureActive(requiredFeatures));
    }

    private bool CheckKeyInput()
    {
        return CheckAllInputGetKeysUp();
    }

    new private bool CheckRequiredFeatures()
    {
        List<string> requiredFeatures = new List<string>
        {
            "isWallRunning"
        };

        return CheckIfFeatureActive(requiredFeatures);
    }
}
