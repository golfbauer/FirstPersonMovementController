using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeatureExecuteOnce : PlayerFeature
{
    // Force added to the player when the action is executed
    public Vector3 MoveForce { get; set; }
    
    public override void CheckAction()
    {
        if ((!Disabled && CanExecute()) || Execute)
        {
            Init();
            ExecuteAction();
            manager.AddRawVelocity(velocity);
            IsExecutingAction = true;
        }
        else
        {
            IsExecuting();
        }

        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        if (!CheckKeys()) return false;
        if (!CheckRequiredFeatures()) return false;
        if (CheckExcludingFeatures()) return false;

        return true;
    }

    protected virtual void IsExecuting()
    {
        IsExecutingAction = false;
        EnableFeatures();
    }

    protected override void Init()
    {
        Execute = false;
        DisableGivenFeatures();    
    }

    protected override void ExecuteAction()
    {
        velocity = MoveForce;
    }
}
