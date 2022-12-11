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

    public CameraController CameraController;
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
        if(DisableFeature || !CanExecute())
        {
            ChangeGravityMultiplier(true);

        } else
        {
            if (!IsExecutingAction) Init();
            IsExecutingAction = true;
            Velocity = ExecuteAction();
            manager.AddVelocity(Velocity, MoveCap);
        }

        UpdateElapsedSince();
    }

    protected override bool CanExecute()
    {
        if (IsExecutingAction)
        {
            if (CancelGrapple()) return false;
            return true;
        }
        if (CheckInputGetKeysDown())
        {
            if (CheckCooldown()) return false;
            if (!CheckGrappleHit()) return false;
            return true;
        }
        return false;
    }

    protected override Vector3 ExecuteAction()
    {
        if (GrapplingAnimation)
        {
            // we need some freeze function
            manager.SetVelocity(Vector3.zero);
            return Vector3.zero;
        }
        //if (manager.kcc.CheckObjectHit(grappleMoveDirect))
        //{
        //    manager.AddActiveFeature("Jumping");
        //    IsExecutingAction = false;
        //    return Vector3.zero;
        //}
        grappleMoveDirect = (GrappleHit.collider.transform.position + localGrappleHitPoint - transform.position).normalized;
        return grappleMoveDirect * GrappleForceFunction() * GrappleSpeed;
    }

    protected override void Init()
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
        if(CanCancelGrapple && !GrapplingAnimation && CheckInputGetKeysDown())
        {
            jumping.CurrentJumpCount = 1;
            IsExecutingAction = false;
            return true;
        }
        return false;
    }

    private bool CheckCooldown()
    {
        return ElapsedSinceLastExecution < GrappleCooldown;
    }

    private bool CheckGrappleHit()
    {
        Transform cameraTransform = CameraController.transform;

        return Physics.Raycast(cameraTransform.position, cameraTransform.forward, out GrappleHit, MaxGrappleDistance, LayerMask.GetMask(GrappleLayers));
    }

    private float GrappleForceFunction()
    {
        return Mathf.Sqrt(ElapsedSinceStartExecution);
    }

    new void UpdateElapsedSince()
    {
        if (IsExecutingAction)
        {
            ElapsedSinceStartExecution += Time.deltaTime;
            ElapsedSinceLastExecution = 0;
            return;
        }

        ElapsedSinceLastExecution += Time.deltaTime;
        ElapsedSinceStartExecution = 1f;
    }
}
