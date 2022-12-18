using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sliding : PlayerFeatureExecuteOverTime
{
    private Crouching crouching;



    new private void Start()
    {
        base.Start();
        crouching = GetComponent<Crouching>();
    }

    new protected bool CanExecute()
    {
        if(!base.CanExecute()) return false;
        if (crouching.IsCrouched) return false;

        return true;
    }

    new protected void ExecuteAction()
    {
        if(elapsedSinceStartExecution < MoveTime)
        {
            Vector3 currentSlideDirect =
                    moveDirect * (1f - MoveControl) +
                    (MoveControl * (initMoveX * transform.right + transform.forward * initMoveZ));
            velocity = currentSlideDirect * MoveSpeed * Time.deltaTime;
            return;
        }

        crouching.Crouch = true;
        IsExecutingAction = false;
    }

    new protected void Init()
    {
        base.Init();
        initMoveX = Input.GetAxis("Horizontal");
        initMoveZ = Input.GetAxis("Vertical");
        moveDirect = transform.right * initMoveX + transform.forward * initMoveZ;
        crouching.Crouch = true;
    }
}

