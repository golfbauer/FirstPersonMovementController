using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Grappling grappleFeature;
    public LineRenderer lr;
    public Transform grapplingGunPos;

    private void LateUpdate()
    {
        if (grappleFeature.IsExecutingAction)
        {
            lr.SetPosition(0, grapplingGunPos.position);
        } else
        {
            lr.enabled = false;
        }
    }

    void Update()
    {
        if (!grappleFeature)
        {
            try
            {
                grappleFeature = GetComponent<Grappling>();
            } catch {
                Debug.Log("PlayerMovement not assigned yet");
            }
        }

        if (grappleFeature && grappleFeature.GrapplingAnimation)
        {
            PlayerGrapleAnimation();
        }

    }

    void PlayerGrapleAnimation()
    {
        lr.enabled = true;
        lr.SetPosition(1, grappleFeature.GrappleHit.point);
        grappleFeature.GrapplingAnimation = false;
    }
}
