using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jumping : PlayerFeature
{

    public int MaxJumpCount { get; set; }
    public float JumpHeight { get; set; }
    public List<string> BreakingFeatures { get; set; }

    public int CurrentJumpCount;

    public override void CheckAction()
    {

        if (Disabled || !CanExecute())
        {
            IsExecutingAction = !CheckIsExecuting();
            UpdateElapsedSince();

            if (manager.IsGrounded()) CurrentJumpCount = 0; 

            return;
        }

        if (!IsExecutingAction) Init();
        ExecuteAction();

        manager.AddRawVelocity(velocity);
        CurrentJumpCount++;

        IsExecutingAction = true;
        UpdateElapsedSince();
    }

    new protected bool CanExecute()
    {
        if (!CheckAllInputGetKeysDown()) return false;

        if (CurrentJumpCount == 0 && !manager.IsGrounded()) return false;

        if (CurrentJumpCount >= MaxJumpCount) return false;

        if (CheckIfFeatureActive(BreakingFeatures)) return false;

        return true;
    }

    new protected void Init()
    {
        return;
    }

    new protected void ExecuteAction()
    {
        manager.ProjectOnPlane = false;
        velocity = new Vector3(0,  Mathf.Sqrt(JumpHeight * -2.0f * manager.Gravity.y), 0);
    }

    private bool CheckIsExecuting()
    {
        return IsExecutingAction && (manager.IsGrounded() || manager.IsFeatureActive("Wallrun"));
    }
}

