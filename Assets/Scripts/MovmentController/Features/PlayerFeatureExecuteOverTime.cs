using UnityEngine;
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

    // Gravity multiplier during execution
    public float GravityMultiplier { get; set; }

    // Cooldown after execution
    public float CoolDown {get; set; }

    // Can the action be cancelled
    public bool CanCancelExecution { get; set; }

    // This is the velocity of the player in the beginning of the execution
    protected Vector3 initVelocity;

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
    
    protected override bool CanExecute()
    {
        if (IsExecutingAction)
        {
            return false;
        }
        
        if (!CheckKeys()) return false;
        if (!CheckRequiredFeatures()) return false;
        if (CheckExcludingFeatures()) return false;
        if(CheckCooldown()) return false;

        return true;
    }

    protected override void Init()
    {
        IsExecutingAction = true;
        Execute = false;
        initVelocity = manager.GetVelocity();
        DisableGivenFeatures();
    }

    /// <summary>
    /// This method is called once in the end of the execution
    /// </summary>
    protected virtual void FinishExecution()
    {
        ResetToInitVelocity();
        IsExecutingAction = false;
        EnableFeatures();
    }

    /// <summary>
    /// This method will reset the velocity to the initial velocity
    /// </summary>
    protected virtual void ResetToInitVelocity()
    {
        manager.SetVelocity(initVelocity);
    }

    /// <summary>
    /// This method checks the cooldown
    /// </summary>
    protected virtual bool CheckCooldown()
    {
        return elapsedSinceLastExecution < CoolDown;
    }

    /// <summary>
    /// This  method will temporarly change the gravity multiplier
    /// </summary>
    protected virtual void ChangeGravityMultiplier(float gravityMultiplier)
    {
        manager.ChangeGravityMultiplier(gravityMultiplier, Identifier);
    }

    /// <summary>
    /// This method will undo the gravity multiplier change
    /// </summary>
    protected virtual void UndoChangeGravityMultiplier()
    {
        manager.UndoChangeGravityMultiplier(Identifier);
    }

    /// <inheritdoc />
    public override void DebugFeatureOnActive()
    {
        if (!DebugFeature) return;

        Debug.Log("Feature " + Identifier + " is active");
        Debug.Log("Elapsed Since Start Execution: " + elapsedSinceStartExecution);
        Debug.Log("Velocity: " + velocity);

        if(elapsedSinceStartExecution >= MoveTime) 
        {
            Debug.Log("Move Cap: " + MoveCap);
            Debug.Log("Move Speed: " + MoveSpeed);
            Debug.Log("Move Control: " + MoveControl);
            Debug.Log("Move Time: " + MoveTime);
            Debug.Log("Gravity Multiplier: " + GravityMultiplier);
            Debug.Log("Can Cancel Execution: " + CanCancelExecution);
        }
        Debug.Log("---------------------------------");
        Debug.Log("---------------------------------");
    }
}

