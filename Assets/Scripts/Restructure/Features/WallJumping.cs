using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumping : PlayerFeatureExecuteOnce
{
    private Jumping jumping;
    
    protected override void Start()
    {
        base.Start();
        jumping = GetComponent<Jumping>();
    }

    protected override void ExecuteAction()
    {
        velocity = CameraController.transform.forward * MoveForce.x;
        velocity.y = MoveForce.y;
    }

   protected override void IsExecuting()
    {
        EnableFeatures();
        if (!IsExecutingAction) return;
        
        IsExecutingAction = !(manager.IsGrounded() || manager.IsFeatureActive("WallRunning") || manager.IsFeatureActive("Jumping") || manager.IsFeatureActive("Dashing"));
    }

    protected override void Init()
    {
        base.Init();
        jumping.CurrentJumpCount++;
        manager.SetVelocity(Vector3.zero);
    }

    protected override bool CheckKeys()
    {
        return CheckAllInputGetKeysUp();
    }
}
