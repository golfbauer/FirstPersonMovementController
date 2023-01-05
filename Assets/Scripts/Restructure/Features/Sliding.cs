using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Sliding : PlayerFeatureExecuteOverTime
{
    public bool CanCancelSlide { get; set; }

    private Crouching crouching;
    private bool cancelSlide => CanCancelSlide && CheckKeys() && elapsedSinceStartExecution > 0;

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
            if(cancelSlide)
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
        base.FinishExecution();
        crouching.Execute = true;
    }
}
