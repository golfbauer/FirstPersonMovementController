using UnityEngine;
using static Utils;

public class Sliding : PlayerFeatureExecuteOverTime
{
    public bool CanCancelSlide { get; set; }

    private Crouching crouching;

    private new void Start()
    {
        base.Start();
        crouching = GetComponent<Crouching>();
    }

    protected override bool CanExecute()
    {
        if(!base.CanExecute()) return false;
        if (!manager.IsGrounded()) return false;
        if (crouching.IsCrouched) return false;

        return true;
    }

    protected override void ExecuteAction()
    {
        if (elapsedSinceStartExecution < MoveTime)
        {
            if(CancelSlide())
            {
                FinishExecution();
                return;
            }
            Vector3 moveX = transform.right * Input.GetAxis("Horizontal");
            Vector3 currentSlideDirect =
                    moveDirect * (1f - MoveControl) +
                    (MoveControl * (moveX + transform.forward * moveDirect.z));
            velocity = currentSlideDirect * MoveSpeed;
            return;
        }

        FinishExecution();
    }

    protected override void Init()
    {
        base.Init();
        moveDirect = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        crouching.Execute = true;
    }

    protected override void FinishExecution()
    {
        IsExecutingAction = false;
        EnableFeatures();
        if (manager.IsGrounded())
        {
            ResetToInitVelocity();
        }
        crouching.Execute = true;
    }

    protected virtual bool CancelSlide()
    {
        return (CanCancelSlide && CheckKeys() && elapsedSinceStartExecution > 0 && !manager.IsFeatureActive(Features.Crouching)) || !manager.IsGrounded();
    }
}
