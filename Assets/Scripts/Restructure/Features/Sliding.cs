using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sliding : PlayerFeature
{
    public float MoveCap { get; set; }
    public float MoveSpeed { get; set; }
    public float MoveControll { get; set; }
    public float MoveTime { get; set; }

    private float initMoveX;
    private float initMoveZ;
    private Vector3 moveDirect;

    private Crouching crouching;

    new private void Start()
    {
        base.Start();
        crouching = GetComponent<Crouching>();
    }

    public override void CheckAction()
    {
        if (!DisableFeature && CanExecute())
        {
            Init();
            IsExecutingAction = true;
        }

        if (IsExecutingAction)
        {
            Velocity = ExecuteAction();
            manager.AddVelocity(Velocity, MoveCap);
        }

        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        if (IsExecutingAction)
        {
            return false;
        }
        if (!CheckKeys()) return false;
        if (CheckActiveFeatures()) return false;
        if (crouching.IsCrouched) return false;

        return true;
    }

    protected override Vector3 ExecuteAction()
    {
        if(ElapsedSinceStartExecution < MoveTime)
        {
            Vector3 currentSlideDirect =
                    moveDirect * (1f - MoveControll) +
                    (MoveControll * (initMoveX * transform.right + transform.forward * initMoveZ));
            return currentSlideDirect * MoveSpeed * Time.deltaTime;
        }

        crouching.Crouch = true;
        IsExecutingAction = false;
        return Vector3.zero;
    }

    protected override void Init()
    {
        initMoveX = Input.GetAxis("Horizontal");
        initMoveZ = Input.GetAxis("Vertical");
        moveDirect = transform.right * initMoveX + transform.forward * initMoveZ;
        crouching.Crouch = true;
    }

    private bool CheckKeys()
    {
        return CheckInputGetKeysDown();
    }

    private bool CheckActiveFeatures()
    {
        return !manager.IsFeatureActive("IsSliding") && manager.IsFeatureActive("IsSprinting");
    }
}

