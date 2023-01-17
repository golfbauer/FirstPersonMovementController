using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackPortalCollider : MonoBehaviour
{
    [SerializeField] private string portalName;
    [SerializeField] private JetpackElevator elevator;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            elevator.portalNames.Add(portalName);
        }
    }
}
