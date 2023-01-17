using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackElevator : MonoBehaviour
{
    private EnvironmentController environmentController;
    public HashSet<string> portalNames = new HashSet<string>();

    private void Start() {
        environmentController = GetComponent<EnvironmentController>();
    }

    private void Update() {
        if(portalNames.Count == 3)
        {
            environmentController.Movement = true;
        }
    }

}
