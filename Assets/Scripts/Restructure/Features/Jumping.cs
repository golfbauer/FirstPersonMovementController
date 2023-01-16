using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jumping : PlayerFeatureExecuteOnce
{
    public int MaxJumpCount { get; set; }
    public float JumpHeight { get; set; }
    public int CurrentJumpCount { get; set; }
    public bool CanAlwaysJump { get; set; }

    protected override bool CanExecute()
    {
        if(!base.CanExecute()) return false;
        if(!CheckJumpCount()) return false;

        return true;
    }

    protected override void ExecuteAction()
    {
        manager.ProjectOnPlane = false;
        var VelocityOffset = -manager.GetVelocity().y;
        var JumpForce = Mathf.Sqrt(JumpHeight * -2.0f * manager.Gravity.y);
        velocity = new Vector3(0, VelocityOffset + JumpForce, 0);
    }

    protected override void IsExecuting()
    {
        EnableFeatures();
        if (manager.IsGrounded() || manager.IsFeatureActive("WallRunning"))
        {
            CurrentJumpCount = 0;

            if (IsExecutingAction)
            {
                IsExecutingAction = false;
            }
        }
        
    }

    protected override void Init()
    {
        base.Init();
        CurrentJumpCount++;
    }

    /// <summary>
    /// Checks if jump count is smaller than max jump count and if player is grounded && jump count is 0
    /// </summary>
    /// <returns>true if player maxjumpcount allows jump</returns>
    protected virtual bool CheckJumpCount()
    {
        if (CurrentJumpCount == 0 && !manager.IsGrounded() && !CanAlwaysJump) return false;

        if (CurrentJumpCount >= MaxJumpCount) return false;

        return true;
    }
}

