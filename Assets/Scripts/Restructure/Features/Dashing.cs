using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : PlayerFeatureExecuteOverTime
{
    public int MaxDashCount { get; set; }

    private float prevGravityMultiplier;
    private int currentDashCount;

    public override void CheckAction()
    {
        base.CheckAction();

        ResetDash();
    }

    new protected bool CanExecute()
    {
        if (manager.IsGrounded()) return false;
        if (!base.CanExecute()) return false;
        if(!CheckDashCount()) return false;

        return true;
    }

    new protected void ExecuteAction()
    {
        if (elapsedSinceLastExecution < MoveTime)
        {
            Vector3 currentDashDirect =
                moveDirect * (1f - MoveControl) +
                (MoveControl * (initMoveX * transform.right + transform.forward * initMoveZ));
            velocity = currentDashDirect * MoveSpeed * Time.deltaTime;
        }
        IsExecutingAction = false;
        ChangeGravityMultiplier(true);
    }

    new protected void Init()
    {
        base.Init();
        initMoveX = Input.GetAxis("Horizontal");
        initMoveZ = Input.GetAxis("Vertical");
        moveDirect = transform.right * initMoveX + transform.forward * initMoveZ;
        if (moveDirect == Vector3.zero)
        {
            moveDirect = transform.forward;
        }
        ChangeGravityMultiplier(false);
    }

    private bool CheckDashCount()
    {
        return currentDashCount < MaxDashCount;
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
        if (!IsExecutingAction && (manager.IsGrounded() || manager.IsFeatureActive("WallRunning") || manager.IsFeatureActive("Grappling")))
        {
            currentDashCount = 0;
        }
    }
}
