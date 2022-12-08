using UnityEngine;
using System.Collections;

public class NewMonoBehaviour : Walking
{
    protected override bool CanExecute()
    {
        return manager.IsGrounded() && CheckInputGetKeys();
    }
}

