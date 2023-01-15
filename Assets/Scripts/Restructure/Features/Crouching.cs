using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crouching : PlayerFeatureExecuteOverTime
{

    public float TimeToCrouch { get; set; }
    public float HeightDifference {
        get
        {
            return heightDifference;
        }
        set
        {
            if(IsCrouched)
            {
                throw new FieldAccessException("Altering the HeightDifference while Crouched is not permitted!");
            }
            heightDifference = value;
        }
    }

    public bool IsCrouched = false;

    private float heightDifference;
    private float targetHeight;
    private float currentHeight;

    private KinematicCharacterController kcc;

    protected override void Start()
    {
        base.Start();
        kcc = manager.Kcc;
    }

    public override void CheckAction()
    {
        if ((!Disabled && CanExecute()) || Execute)
        {
            Init();
        }

        if (IsExecutingAction)
        {
            if(!AllowedToStandUp()) return;
            ExecuteAction();
        }
        
        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        if(!base.CanExecute()) return false;
        if (!manager.IsGrounded()) return false;

        return true;
    }

    protected override void ExecuteAction()
    {
        if (elapsedSinceStartExecution < TimeToCrouch)
        {
            float targetControllerHeight = Mathf.Lerp(currentHeight, targetHeight, elapsedSinceStartExecution / TimeToCrouch);
            float heightDifference = targetControllerHeight - kcc.Height;
            transform.position += Vector3.up * (heightDifference/2);

            CameraController.AddCameraHeight(heightDifference/2);
            kcc.Height = targetControllerHeight;

            return;
        }

        FinishExecution();
        IsCrouched = !IsCrouched;
    }

    protected override void Init()
    {
        base.Init();
        if(IsCrouched)
        {
            currentHeight = kcc.Height;
            targetHeight = kcc.Height + heightDifference;
            return;
        }

        currentHeight = kcc.Height;
        targetHeight = kcc.Height - heightDifference;
    }

    protected override void FinishExecution()
    {
        kcc.Height = targetHeight;
        CameraController.LocalStartPosition = CameraController.transform.localPosition;
        IsExecutingAction = false;
        EnableFeatures();
    }

    /// <summary>
    /// Checks if the player is allowed to stand up.
    /// </summary>
    /// <returns>True if the player is allowed to stand up, false if not.</returns>
    protected virtual bool AllowedToStandUp()
    {
        if(!IsCrouched) return true;
        if(Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, kcc.Height/2 + heightDifference))
        {
            if(elapsedSinceStartExecution <= 0f){
                IsExecutingAction = false;
                EnableFeatures();
            }
            return false;
        }

        return true;
    }
}
