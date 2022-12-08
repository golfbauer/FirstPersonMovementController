using UnityEngine;
using System.Collections;

public class Sprinting : Walking
{
    protected override bool CanExecute()
    {
        return manager.IsGrounded() && CheckInputGetKeys();
    }
}

