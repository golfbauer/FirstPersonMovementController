using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if(IsCrouched && value != heightDifference)
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

        if(!CastStandUp()) {
            if(elapsedSinceStartExecution <= 0f){
                IsExecutingAction = false;
                EnableFeatures();
            }
            return false;
        }

        return true;
    }

    protected virtual bool CastStandUp(){
        (Vector3 center, Vector3 bottom, Vector3 top, float radius, float height) = manager.Kcc.GetCapsuleParameters(transform.position, transform.rotation);
        Vector2 direction = manager.GetVelocity() * Time.deltaTime;
        top.y += HeightDifference + 0.1f;
         IEnumerable<RaycastHit> hits = Physics.CapsuleCastAll(
            top, bottom, radius, direction, direction.magnitude, ~0, QueryTriggerInteraction.Ignore)
            .Where(hit => hit.collider.transform != transform && hit.distance == 0);

        return hits.Count() == 0;
    }
}
