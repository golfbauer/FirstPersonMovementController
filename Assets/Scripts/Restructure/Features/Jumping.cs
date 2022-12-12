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

        if (DisableFeature || !CanExecute())
        {
            IsExecutingAction = !CheckIsExecuting();
            UpdateElapsedSince();

            if (manager.IsGrounded()) CurrentJumpCount = 0; 

            return;
        }

        if (!IsExecutingAction) Init();
        Velocity = ExecuteAction();

        manager.AddRawVelocity(Velocity);
        CurrentJumpCount++;

        IsExecutingAction = true;
        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        if (!CheckInputGetKeysDown()) return false;

        if (CurrentJumpCount == 0 && !manager.IsGrounded()) return false;

        if (CurrentJumpCount >= MaxJumpCount) return false;

        if (CheckIfFeaturesActive(BreakingFeatures)) return false;

        return true;
    }

    protected override void Init()
    {
        return;
    }

    protected override Vector3 ExecuteAction()
    {
        manager.ProjectOnPlane = false;
        return new Vector3(0,  Mathf.Sqrt(JumpHeight * -2.0f * manager.Gravity.y), 0);
    }

    private bool CheckIsExecuting()
    {
        return IsExecutingAction && (manager.IsGrounded() || manager.IsFeatureActive("Wallrun"));
    }
}

