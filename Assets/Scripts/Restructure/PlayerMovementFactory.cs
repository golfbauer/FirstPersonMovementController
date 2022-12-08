using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementFactory : MonoBehaviour
{
    private PlayerMovementManager manager;

    // Start is called before the first frame update
    void Start()
    {

        InitializeManager();

        InitializeWalking();
        InitializeSprinting();
        InitializeJumping();

    }

    void InitializeWalking()
    {
        Walking walking = this.AddComponent<Walking>();
        walking.MoveSpeed = 10f;
        walking.MoveCap = 10f;
        walking.SupportedFeatures = new List<string>();
        walking.Identifier = "Walking";
        manager.AddFeature(walking.Identifier, walking);
    }

    void InitializeSprinting()
    {
        Sprinting sprinting = this.AddComponent<Sprinting>();
        sprinting.MoveSpeed = 10f;
        sprinting.MoveCap = 10f;
        sprinting.SupportedFeatures = new List<string>();
        sprinting.Identifier = "Sprinting";
        manager.AddFeature(sprinting.Identifier, sprinting);
    }

    void InitializeJumping()
    {
        Jumping jumping = this.AddComponent<Jumping>();
        jumping.MaxJumpCount = 2;
        jumping.JumpForce = new Vector3(0, 10f, 0);
        jumping.JumpCap = 30f;
        jumping.BreakingFeatures = new List<string>();
        jumping.SupportedFeatures = new List<string>();
        jumping.Identifier = "Jumping";
        manager.AddFeature(jumping.Identifier, jumping);
    }

    void InitializeManager()
    {
        manager = this.AddComponent<PlayerMovementManager>();
        manager.BaseGravity = new Vector3(0, -9.81f, 0);
        manager.GroundedVelocityDeclineRate = 5;
        manager.AirborneVelocityDeclineRate = 0;
    }
}
