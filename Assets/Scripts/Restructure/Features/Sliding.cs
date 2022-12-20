using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sliding : PlayerFeatureExecuteOverTime
{
    private Crouching crouching;


    private new void Start()
    {
        base.Start();
        crouching = GetComponent<Crouching>();
    }

    protected override bool CanExecute()
    {
        if(!base.CanExecute()) return false;
        if (crouching.IsCrouched) return false;

        return true;
    }

    protected override void ExecuteAction()
    {
        if (elapsedSinceStartExecution < MoveTime)
        {
            Vector3 currentSlideDirect =
                    moveDirect * (1f - MoveControl) +
                    (MoveControl * (initMoveX * transform.right + transform.forward * initMoveZ));
            velocity = currentSlideDirect * MoveSpeed;
            return;
        }

        crouching.Execute = true;
        IsExecutingAction = false;
    }

    protected override void Init()
    {
        base.Init();
        initMoveX = Input.GetAxis("Horizontal");
        initMoveZ = Input.GetAxis("Vertical");
        moveDirect = transform.right * initMoveX + transform.forward * initMoveZ;
        crouching.Execute = true;
    }
}

