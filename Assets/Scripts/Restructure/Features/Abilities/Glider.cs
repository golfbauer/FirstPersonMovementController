using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Please write a class with the following feature:
/// This feature is supposed to implement the glider. This means that the player can glide down from a certain height in the air.
/// For this the class has to check whether the player is in the air and has a certain distance to the ground and the key is pressed.
/// After that the player will Rotate along the x Axis about 90 degrees to face forward. The Gravity is changed to simulate the glider.
/// Also there is a force going forward. The glider can be controlled with the mouse. If the player looks down the glider will speed to the ground.
/// If the player looks forward the glider will go up. If the player looks to the side the glider will go to the side.
/// </summary>
public class Glider : PlayerFeatureExecuteOverTime
{
    public float MinDistanceToGround { get; set; }
    public float RotationSpeed { get; set; }

    protected Vector3 rotation;
    protected float drag;
    protected KinematicCharacterController controller;

    protected override void Start()
    {
        base.Start();
        controller = manager.Kcc;
    }

    public override void CheckAction()
    {
        if ((!Disabled && CanExecute()) || Execute)
        {
            Init();
        }

        if (IsExecutingAction)
        {
            ExecuteAction();
            manager.AddVelocity(velocity, MoveCap);
        }

        UpdateElapsedSince();
    }
    
    protected override bool CanExecute()
    {
        if (!base.CanExecute()) return false;
        if (manager.IsGrounded()) return false;
        if (CheckDistanceToGround(MinDistanceToGround)) return false;

        return true;
        
    }

    protected override void Init()
    {
        base.Init();
    }

    protected virtual bool CheckDistanceToGround(float distance)
    {
        return Physics.Raycast(transform.position, Vector3.down, distance);
    }

    protected override void ExecuteAction()
    {
        if (!CheckDuringExecution())
        {
            FinishExecution();
            return;
        }

        //if (transform.localRotation.eulerAngles.x < 80)
        //{
        //    transform.Rotate(RotationSpeed, 0f, 0f);
        //}

        UpwardsDrag();
        Steering();
        velocity *= MoveSpeed;
    }

    protected virtual void Steering()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Debug.Log(moveZ);

        if(moveZ > 0)
        {
            drag *= 0;
        } else
        {
            drag *= 1f;
        }

        manager.AddRawVelocity(Vector3.up * drag);
        velocity = transform.right * moveX + transform.forward;
    }

    protected virtual void UpwardsDrag()
    {
        drag = -manager.GetVelocity().y;
    }

    protected virtual bool CheckDuringExecution()
    {
        if (manager.IsGrounded()) return false;
        return true;
    }

    protected override void FinishExecution()
    {
        base.FinishExecution();
        //transform.Rotate(0f, 0f, 0f);
    }
}
