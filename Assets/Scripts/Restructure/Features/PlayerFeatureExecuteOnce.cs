using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeatureExecuteOnce : PlayerFeature
{

    public Vector3 MoveForce { get; set; }
    
    public override void CheckAction()
    {
        if (Disabled || !CanExecute())
        {
            IsExecuting();
        }
        else
        {
            Init();
            ExecuteAction();
            manager.AddRawVelocity(velocity);
            IsExecutingAction = true;
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
        DisableGivenFeatures();    
    }

    protected override void ExecuteAction()
    {
        velocity = MoveForce;
    }
}
