using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public LineRenderer lr;
    public Transform grapplingGunPos;

    private void LateUpdate()
    {
        if (playerMovement.isGrappling)
        {
            lr.SetPosition(0, grapplingGunPos.position);
        } else
        {
            lr.enabled = false;
        }
    }

    void Update()
    {
        if (!playerMovement)
        {
            try
            {
                playerMovement = GetComponent<PlayerMovement>();
            } catch {
                Debug.Log("PlayerMovement not assigned yet");
            }
        }

        if (playerMovement && playerMovement.grapplingAnimation)
        {
            PlayerGrapleAnimation();
        }

    }

    void PlayerGrapleAnimation()
    {
        lr.enabled = true;
        lr.SetPosition(1, playerMovement.grappleHit.point);
        playerMovement.grapplingAnimation = false;
    }
}
