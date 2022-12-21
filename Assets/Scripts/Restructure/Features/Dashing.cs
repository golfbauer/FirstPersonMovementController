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

    protected override bool CanExecute()
    {
        if (manager.IsGrounded()) return false;
        if (!base.CanExecute()) return false;
        if (!CheckDashCount()) return false;

        return true;
    }

    protected override void ExecuteAction()
    {
        if (elapsedSinceStartExecution < MoveTime)
        {
            Vector3 currentDashDirect =
                moveDirect * (1f - MoveControl) +
                (MoveControl * (initMoveX * transform.right + transform.forward * initMoveZ));
            velocity = currentDashDirect * MoveSpeed;
            return;
        }

        FinishExecution();
    }

    protected override void Init()
    {
        Vector3 managerVelocity = manager.GetVelocity();
        managerVelocity.y = 0f;
        manager.SetVelocity(managerVelocity);
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

    protected virtual bool CheckDashCount()
    {
        return currentDashCount < MaxDashCount;
    }

    protected virtual void ChangeGravityMultiplier(bool undoGravity)
    {
        if (undoGravity)
        {
            manager.GravityMultiplier = prevGravityMultiplier;
            return;
        }

        prevGravityMultiplier = manager.GravityMultiplier;
        manager.GravityMultiplier = 0;
    }

    protected virtual void ResetDash()
    {
        if (!IsExecutingAction && (manager.IsGrounded() || manager.IsFeatureActive("WallRunning") || manager.IsFeatureActive("Grappling")))
        {
            currentDashCount = 0;
        }
    }

    protected override bool CheckRequiredFeatures()
    {
        return CheckIfFeatureActive(RequiredFeatures);
    }

    protected override void FinishExecution()
    {
        base.FinishExecution();
        ChangeGravityMultiplier(true);
        currentDashCount++;
    }
}
