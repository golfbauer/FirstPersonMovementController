﻿using UnityEngine;
using System.Collections;

public abstract class PlayerFeatureExecuteOverTime : PlayerFeature
{
    // Max movement that can be applied to player
    public float MoveCap { get; set; }

    // Speed that gets added over time
    public float MoveSpeed { get; set; }

    // Control given to player during execution 0 - 1
    public float MoveControl { get; set; }

    // Time action is executed
    public float MoveTime { get; set; }

    protected Vector3 initVelocity;

    // Initial X direction
    protected float initMoveX;

    // Initial Z direction
    protected float initMoveZ;

    // Move Direction during execution
    protected Vector3 moveDirect;


    public override void CheckAction()
    {
        if ((!Disabled && CanExecute()) || Execute)
        {
            Init();
        }

        if (IsExecutingAction)
        {
            ExecuteAction();
            manager.AddVelocity(velocity, MoveCap);
        }

        UpdateElapsedSince();
    }

    protected virtual new bool CanExecute()
    {
        if (IsExecutingAction)
        {
            return false;
        }
        if (!CheckKeys()) return false;
        if (!CheckRequiredFeatures()) return false;
        if (CheckExcludingFeatures()) return false;

        return true;
    }

    protected virtual new void Init()
    {
        IsExecutingAction = true;
        initVelocity = manager.GetVelocity();
        DisableGivenFeatures();
    }

    protected virtual void FinishExecution()
    {
        ResetToInitVelocity();
        IsExecutingAction = false;
        EnableFeatures();
    }

    protected virtual void ResetToInitVelocity()
    {
        manager.SetVelocity(initVelocity);
    }
}

