using UnityEngine;
using System.Collections;

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

        Init();
        velocity = ExecuteAction();

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
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

        return moveDirect * MoveSpeed * Time.deltaTime;
    }
}

