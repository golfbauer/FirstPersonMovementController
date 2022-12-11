using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : PlayerFeature
{

    public float MoveCap { get; set; }
    public float DashSpeed { get; set; }
    public float DashControll { get; set; }
    public float MaxDashTime { get; set; }
    public int MaxDashCount { get; set; }

    private float prevGravityMultiplier;
    private int currentDashCount;
    private float currentDashTime;
    private float dashX;
    private float dashZ;
    private Vector3 dashDirect;

    public override void CheckAction()
    {
        if(!DisableFeature && CanExecute())
        {
            Init();
        } 
        
        if(IsExecutingAction) {
            IsExecutingAction = true;
            Velocity = ExecuteAction();
            manager.AddVelocity(Velocity, MoveCap);
        }

        ResetDash();
    }

    protected override bool CanExecute()
    {
        if (IsExecutingAction) return false;
        if (manager.IsGrounded()) return false;
        if (CheckFeatures()) return false;
        if (!CheckInput()) return false;
        if(!CheckDashCount()) return false;

        return true;
    }

    protected override Vector3 ExecuteAction()
    {
        if (currentDashTime < MaxDashTime)
        {
            Vector3 currentDashDirect =
                dashDirect * (1f - DashControll) +
                (DashControll * (dashX * transform.right + transform.forward * dashZ));
            return currentDashDirect * DashSpeed * Time.deltaTime;
        }
        IsExecutingAction = false;
        ChangeGravityMultiplier(true);
        //manager.AddActiveFeature("Jumping");
        return Vector3.zero;
    }

    protected override void Init()
    {
        dashX = Input.GetAxis("Horizontal");
        dashZ = Input.GetAxis("Vertical");
        dashDirect = transform.right * dashX + transform.forward * dashZ;
        if (dashDirect == Vector3.zero)
        {
            dashDirect = transform.forward;
        }
        currentDashTime = 0;
        ChangeGravityMultiplier(false);
        IsExecutingAction = true;
        //manager.RemoveActiveFeature("Jumping");
    }

    private bool CheckInput()
    {
        return CheckInputGetKeysDown();
    }
    private bool CheckDashCount()
    {
        return currentDashCount < MaxDashCount;
    }

    private bool CheckFeatures()
    {
        List<string> requiredFeatures = new List<string>
        {
            "isJumping",
            "isWallJumping"
        };

        return CheckIfFeaturesActive(requiredFeatures);
    }

    private void ChangeGravityMultiplier(bool undoGravity)
    {
        if (undoGravity)
        {
            manager.GravityMultiplier = prevGravityMultiplier;
            return;
        }

        prevGravityMultiplier = manager.GravityMultiplier;
        manager.GravityMultiplier = 0;
    }

    void ResetDash()
    {
        if ( !IsExecutingAction && (manager.IsGrounded() || manager.IsFeatureActive("WallRunning") || manager.IsFeatureActive("Grappling")))
        {
            currentDashCount = 0;
        }
    }
}
