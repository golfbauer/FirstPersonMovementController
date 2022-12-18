using UnityEngine;
using System.Collections;

public class Sprinting : Walking
{
    new protected bool CanExecute()
    {
        return manager.IsGrounded() && CheckAllInputGetKeys();
    }
}

