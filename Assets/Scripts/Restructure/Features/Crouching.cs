using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crouching : PlayerFeature
{

    public float TimeToCrouch { get; set; }
    public float HeightDifference { get; set; }

    public bool IsCrouched;
    public bool Crouch;

    private float targetHeight;
    private float currentHeight;

    private KinematicCharacterController kcc;
    public CameraController CameraController;

    new private void Start()
    {
        base.Start();
        kcc = GetComponent<KinematicCharacterController>();
    }

    public override void CheckAction()
    {
        if (CanExecute())
        {
            Init();
            IsExecutingAction = true;
            Crouch = false;
        }

        if (IsExecutingAction)
        {
            ExecuteAction();
        }

        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        if (IsExecutingAction) return false;

        if (Crouch) return true;

        if (!CheckKeys()) return false;
        if (!manager.IsGrounded()) return false;
        if (CheckFeatures()) return false;

        return true;
    }

    protected override Vector3 ExecuteAction()
    {
        if (ElapsedSinceStartExecution < TimeToCrouch)
        {
            float heightDifference = Mathf.Lerp(currentHeight, targetHeight, ElapsedSinceStartExecution / TimeToCrouch);

            if (IsCrouched)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + (heightDifference - kcc.Height), transform.position.z);
            }

            CameraController.SetCameraHeight(heightDifference - kcc.Height);
            kcc.Height = heightDifference;

            return Vector3.zero;
        }

        IsExecutingAction = false;
        IsCrouched = !IsCrouched;
        return Vector3.zero;
    }

    protected override void Init()
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

    private bool CheckKeys()
    {
        return CheckInputGetKeysDown();
    }

    private bool CheckFeatures()
    {
        List<string> requiredFeatures = new List<string>
        {
            "isSliding",
            "isSprinting"
        };

        return CheckIfFeaturesActive(requiredFeatures);
    }
}
