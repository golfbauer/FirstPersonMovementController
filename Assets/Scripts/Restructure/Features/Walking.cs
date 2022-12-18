using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Walking : PlayerFeature
{

    public float MoveSpeed { get; set; }
    public float MoveCap { get; set; }

    public override void CheckAction()
    {
        if (Disabled || !CanExecute())
        {
            IsExecutingAction = false;
            UpdateElapsedSince();
            return;
        }
        if(!IsExecutingAction) Init();
        ExecuteAction();

        if(velocity != Vector3.zero)
        {
            manager.AddVelocity(velocity, MoveCap);
            IsExecutingAction = true;
            UpdateElapsedSince();
            return;
        }

        IsExecutingAction = false;
        UpdateElapsedSince();
    }

    new protected void ExecuteAction()
    {

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

        velocity = moveDirect * MoveSpeed;
    }
}

