using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Walking : PlayerFeature
{
    public float MoveSpeed { get; set; }
    public float MoveCap { get; set; }

    public override void CheckAction()
    {
        if(Disabled)
        {
            UpdateElapsedSince();
            IsExecutingAction = false;
            return;
        }

        ExecuteAction();

        if (velocity != Vector3.zero)
        {
            manager.AddVelocity(velocity, MoveCap);
            IsExecutingAction = true;
        } else
        {
            IsExecutingAction = false;
        }

        UpdateElapsedSince();
    }

    protected override void ExecuteAction()
    {

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

        velocity = moveDirect * MoveSpeed;
    }
}

