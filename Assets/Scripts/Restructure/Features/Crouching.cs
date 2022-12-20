using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crouching : PlayerFeature
{

    public float TimeToCrouch { get; set; }
    public float HeightDifference { get; set; }

    public bool IsCrouched;

    private float targetHeight;
    private float currentHeight;

    private KinematicCharacterController kcc;

    private new void Start()
    {
        base.Start();
        kcc = manager.Kcc;
    }

    public override void CheckAction()
    {
        if (!Disabled && CanExecute())
        {
            Init();
            IsExecutingAction = true;
            Execute = false;
        }

        if (IsExecutingAction)
        {
            ExecuteAction();
        }

        UpdateElapsedSince();
    }

    protected new bool CanExecute()
    {
        if (IsExecutingAction) return false;

        if (Execute) return true;

        if (!CheckKeys()) return false;
        if (!manager.IsGrounded()) return false;
        if (CheckExcludingFeatures()) return false;

        return true;
    }

    protected new void ExecuteAction()
    {
        if (elapsedSinceStartExecution < TimeToCrouch)
        {
            float heightDifference = Mathf.Lerp(currentHeight, targetHeight, elapsedSinceStartExecution / TimeToCrouch);

            if (IsCrouched)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + (heightDifference - kcc.Height), transform.position.z);
            }

            CameraController.SetCameraHeight(heightDifference - kcc.Height);
            kcc.Height = heightDifference;

            return;
        }

        IsExecutingAction = false;
        IsCrouched = !IsCrouched;
        return;
    }

    protected new void Init()
    {
        if(IsCrouched)
        {
            currentHeight = kcc.Height;
            targetHeight = kcc.Height + HeightDifference;
            return;
        }

        currentHeight = kcc.Height;
        targetHeight = kcc.Height - HeightDifference;
    }
}
