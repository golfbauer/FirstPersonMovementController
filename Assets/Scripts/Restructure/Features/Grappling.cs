using UnityEngine;

public class Grappling : PlayerFeatureExecuteOverTime
{
    public float MaxGrappleDistance { get; set; }
    public string[] GrappleLayers { get; set; }

    public bool GrapplingAnimation;
    public RaycastHit GrappleHit;

    protected Vector3 localGrappleHitPoint;
    protected Jumping jumping;


    protected override void Start()
    {
        base.Start();
        jumping = GetComponent<Jumping>();
    }

    protected override bool CanExecute()
    {
        if(!base.CanExecute()) return false;
        if (!CheckGrappleHit()) return false;

        return true;
    }

    /// <summary>
    /// Checks if players grapple has hit an object. If so grapple executes.
    /// </summary>
    /// <returns>true if grapple has hit an object</returns>
    protected virtual bool CheckGrappleHit()
    {
        Transform cameraTransform = CameraController.transform;

        return Physics.Raycast(cameraTransform.position, cameraTransform.forward, out GrappleHit, MaxGrappleDistance, LayerMask.GetMask(GrappleLayers));
    }

    protected override void Init()
    {
        base.Init();
        manager.ProjectOnPlane = false;
        localGrappleHitPoint = GrappleHit.point - GrappleHit.collider.transform.position;
        GrapplingAnimation = true;
        ChangeGravityMultiplier(0f);
    }

    protected override void ExecuteAction()
    {
        if(CheckGrappleAnimation()) return;

        if(CancelGrapple()) return;

        if (CheckForCollision()) return;

        moveDirect = (GrappleHit.collider.transform.position + localGrappleHitPoint - transform.position).normalized;
        velocity = moveDirect * GrappleForceFunction() * MoveSpeed;
    }

    /// <summary>
    /// Checks if player is in grapple animation. If so it freqqzes the player.
    /// </summary>
    /// <returns><c>true</c>, if in grapple animation, <c>false</c> otherwise.</returns>
    protected virtual bool CheckGrappleAnimation(){
        if (GrapplingAnimation)
        {
            manager.Freeze(Identifier);
            return true;
        }

        manager.UnFreeze();
        return false;
    }

    /// <summary>
    /// Cancels the grapple if not in animation and grapple key pressed during execution.
    /// </summary>
    /// <returns><c>true</c>, if grapple was canceled, <c>false</c> otherwise.</returns>
    protected virtual bool CancelGrapple()
    {
        if(CanCancelExecution && !GrapplingAnimation && CheckAllInputGetKeysDown())
        {
            FinishExecution();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if player hits an obsticale when grappling if so he stops the grapple.
    /// </summary>
    /// <returns><c>true</c>, if for collision was checked, <c>false</c> otherwise.</returns>
    protected virtual bool CheckForCollision(){
        if (manager.Kcc.CheckObjectHit(moveDirect))
        {
           manager.SetVelocity(Vector3.zero);
           FinishExecution();
           return true;
        }

        return false;
    } 

    /// <summary>
    /// Grapple force function.
    /// </summary>
    /// <returns>Speed for the current time.</returns>
    protected virtual float GrappleForceFunction()
    {
        return Mathf.Sqrt(elapsedSinceStartExecution);
    }

    protected override void UpdateElapsedSince()
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

    protected override void FinishExecution()
    {
        jumping.CurrentJumpCount = 1;
        manager.ProjectOnPlane = true;
        IsExecutingAction = false;
        UndoChangeGravityMultiplier();
        manager.SetFeatureActive("Jumping");
        EnableFeatures();
    }
}
