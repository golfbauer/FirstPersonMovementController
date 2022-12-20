using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jumping : PlayerFeature
{

    public int MaxJumpCount { get; set; }
    public float JumpHeight { get; set; }

    public int CurrentJumpCount;

    public override void CheckAction()
    {

        if (Disabled || !CanExecute())
        {
            IsExecutingAction = !CheckIsExecuting();
            if (manager.IsGrounded()) CurrentJumpCount = 0; 

        } else
        {
            if (!IsExecutingAction) Init();

            ExecuteAction();
            manager.AddRawVelocity(velocity);

            CurrentJumpCount++;
            IsExecutingAction = true;
        }

        UpdateElapsedSince();
    }

    protected new bool CanExecute()
    {
        if (!CheckAllInputGetKeysDown()) return false;

        if (CurrentJumpCount == 0 && !manager.IsGrounded()) return false;

        if (CurrentJumpCount >= MaxJumpCount) return false;

        if (CheckExcludingFeatures()) return false;

        return true;
    }

    protected new void ExecuteAction()
    {
        manager.ProjectOnPlane = false;
        velocity = new Vector3(0,  Mathf.Sqrt(JumpHeight * -2.0f * manager.Gravity.y), 0);
    }

    private bool CheckIsExecuting()
    {
        return IsExecutingAction && (manager.IsGrounded() || manager.IsFeatureActive("Wallrun"));
    }
}

