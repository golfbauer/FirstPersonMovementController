using UnityEngine;
using System.Collections;

public class Sprinting : Walking
{

    public override void CheckAction()
    {
        if(Disabled || !CanExecute())
        {
            IsExecutingAction = false;
            UpdateElapsedSince();
            return;
        }

        base.CheckAction();
    }
    
    protected override bool CanExecute()
    {
        return manager.IsGrounded() && CheckAllInputGetKeys();
    }
}

