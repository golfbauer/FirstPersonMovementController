using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jumping : PlayerFeature
{

    public int MaxJumpCount { get; set; }
    public Vector3 JumpForce { get; set; }
    public float JumpCap { get; set; }
    public List<string> BreakingFeatures { get; set; }

    private int currentJumpCount;

    public override void CheckAction()
    {
        if (DisableFeature || !CanExecute())
        {
            IsExecutingAction = !CheckIsExecuting();
            UpdateElapsedSince();
            return;
        }

        if (!IsExecutingAction) Init();
        Velocity = ExecuteAction();

        manager.AddVelocity(Velocity, JumpCap);
        IsExecutingAction = true;
        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        if (!CheckInputGetKeysDown()) return false;

        if (currentJumpCount == 0 && !manager.IsGrounded()) return false;

        if (currentJumpCount >= MaxJumpCount) return false;

        if (IsPlayerInOneOfStates(BreakingFeatures)) return false;

        return true;
    }

    protected override void Init()
    {
        return;
    }

    protected override Vector3 ExecuteAction()
    {
        return JumpForce;
    }

    private bool CheckIsExecuting()
    {
        return IsExecutingAction && (manager.IsGrounded() || manager.IsFeatureActive("Wallrun"));
    }
}

