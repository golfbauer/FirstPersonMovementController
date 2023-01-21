using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

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
        
        IsExecutingAction = !(manager.IsGrounded() || manager.IsFeatureActive(Features.WallRunning) || manager.IsFeatureActive(Features.Jumping) || manager.IsFeatureActive(Features.Dashing));
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
