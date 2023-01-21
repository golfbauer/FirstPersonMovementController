using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class Dashing : PlayerFeatureExecuteOverTime
{
    public int MaxDashCount { get; set; }

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
            float moveX = Input.GetAxis("Horizontal");
            Vector3 currentDashDirect =
                moveDirect * (1f - MoveControl) +
                (MoveControl * (moveDirect + transform.right * moveX));
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
        moveDirect = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        if (moveDirect == Vector3.zero)
        {
            moveDirect = transform.forward;
        }
        ChangeGravityMultiplier(GravityMultiplier);
    }

    /// <summary>
    /// Checks the dash count.
    /// </summary>
    /// <returns><c>true</c>, if dash count is below max dash count, <c>false</c> otherwise.</returns>
    protected virtual bool CheckDashCount()
    {
        return currentDashCount < MaxDashCount;
    }

    /// <summary>
    /// Resets the dash count if the player is grounded or wall running or grappling.
    /// </summary>
    protected virtual void ResetDash()
    {
        if (!IsExecutingAction && (manager.IsGrounded() || manager.IsFeatureActive(Features.WallRunning) || manager.IsFeatureActive(Features.Grappling)))
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
        UndoChangeGravityMultiplier();
        currentDashCount++;
    }
}
