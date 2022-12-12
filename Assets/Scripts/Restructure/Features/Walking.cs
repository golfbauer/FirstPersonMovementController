using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Walking : PlayerFeature
{

    public float MoveSpeed { get; set; }
    public float MoveCap { get; set; }

    public override void CheckAction()
    {
        if (DisableFeature || !CanExecute())
        {
            IsExecutingAction = false;
            UpdateElapsedSince();
            return;
        }
        if(!IsExecutingAction) Init();
        Velocity = ExecuteAction();

        if(Velocity != Vector3.zero)
        {
            manager.AddVelocity(Velocity, MoveCap);
            IsExecutingAction = true;
            UpdateElapsedSince();
            return;
        }

        IsExecutingAction = false;
        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        return true;
    }

    protected override void Init()
    {
        return;
    }

    protected override Vector3 ExecuteAction()
    {

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

        return moveDirect * MoveSpeed;
    }
}

