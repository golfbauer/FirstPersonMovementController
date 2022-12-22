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
            Vector3 currentSlideDirect =
                    moveDirect * (1f - MoveControl) +
                    (MoveControl * (initMoveX * transform.right + transform.forward * initMoveZ));
            velocity = currentSlideDirect * MoveSpeed;
            return;
        }

        FinishExecution();
    }

    protected override void Init()
    {
        base.Init();
        initMoveX = Input.GetAxis("Horizontal");
        initMoveZ = Input.GetAxis("Vertical");
        moveDirect = transform.right * initMoveX + transform.forward * initMoveZ;
        crouching.Execute = true;
    }

    protected override void FinishExecution()
    {
        base.FinishExecution();
        crouching.Execute = true;
    }
}

// TODO:
// Slide: cancel on no ground
// Wallrun: when end wall just go camera direct
