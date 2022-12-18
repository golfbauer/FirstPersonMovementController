using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : PlayerFeature
{
    public float MoveCap { get; set; }
    public float GrappleCooldown { get; set; }
    public float MaxGrappleDistance { get; set; }
    public float GrappleSpeed { get; set; }
    public string[] GrappleLayers { get; set; }
    public bool CanCancelGrapple { get; set; }

    public bool GrapplingAnimation;
    public RaycastHit GrappleHit;

    private float prevGravityMultiplier;
    private Vector3 localGrappleHitPoint;
    private Vector3 grappleMoveDirect;

    private Jumping jumping;


    new void Start()
    {
        base.Start();
        prevGravityMultiplier = manager.GravityMultiplier;
        jumping = GetComponent<Jumping>();
    }

    public override void CheckAction()
    {
        if(Disabled || !CanExecute())
        {
            ChangeGravityMultiplier(true);

        } else
        {
            if (!IsExecutingAction) Init();
            IsExecutingAction = true;
            ExecuteAction();
            manager.AddVelocity(velocity, MoveCap);
        }

        UpdateElapsedSince();
    }

    new protected bool CanExecute()
    {
        if (IsExecutingAction)
        {
            if (CancelGrapple()) return false;
            return true;
        }
        if (CheckAllInputGetKeysDown())
        {
            if (CheckCooldown()) return false;
            if (!CheckGrappleHit()) return false;
            return true;
        }
        return false;
    }

    new protected void ExecuteAction()
    {
        if (GrapplingAnimation)
        {
            // we need some freeze function
            manager.SetVelocity(Vector3.zero);
            return;
        }
        //if (manager.kcc.CheckObjectHit(grappleMoveDirect))
        //{
        //    manager.AddActiveFeature("Jumping");
        //    IsExecutingAction = false;
        //    return Vector3.zero;
        //}
        grappleMoveDirect = (GrappleHit.collider.transform.position + localGrappleHitPoint - transform.position).normalized;
        velocity = grappleMoveDirect * GrappleForceFunction() * GrappleSpeed;
    }

    new protected void Init()
    {
        localGrappleHitPoint = GrappleHit.point - GrappleHit.collider.transform.position;
        GrapplingAnimation = true;
        ChangeGravityMultiplier(false);
    }

    private void ChangeGravityMultiplier(bool undoGravity)
    {
        if (undoGravity)
        {
            manager.GravityMultiplier = prevGravityMultiplier;
            return;
        }

        prevGravityMultiplier = manager.GravityMultiplier;
        manager.GravityMultiplier = 0;
    }

    private bool CancelGrapple()
    {
        if(CanCancelGrapple && !GrapplingAnimation && CheckAllInputGetKeysDown())
        {
            jumping.CurrentJumpCount = 1;
            IsExecutingAction = false;
            return true;
        }
        return false;
    }

    private bool CheckCooldown()
    {
        return elapsedSinceLastExecution < GrappleCooldown;
    }

    private bool CheckGrappleHit()
    {
        Transform cameraTransform = CameraController.transform;

        return Physics.Raycast(cameraTransform.position, cameraTransform.forward, out GrappleHit, MaxGrappleDistance, LayerMask.GetMask(GrappleLayers));
    }

    private float GrappleForceFunction()
    {
        return Mathf.Sqrt(elapsedSinceStartExecution);
    }

    new void UpdateElapsedSince()
    {
        if (IsExecutingAction)
        {
            elapsedSinceStartExecution += Time.deltaTime;
            elapsedSinceLastExecution = 0;
            return;
        }

        elapsedSinceLastExecution += Time.deltaTime;
        elapsedSinceStartExecution = 1f;
    }
}
