using Assets.Parkour_Game.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static ParkourUtils;

public class ParkourGameManager : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private float deathPlaneY;
    [SerializeField] private TMP_Text uiText;

    public HashSet<MovementFeature> EnableFeatures;
    public HashSet<ParkourMovementFeature> EnabledFeatures;
    public UIManager UiManager;

    private int deathCount;
    private PlayerMovementManager movementManager;

    void Start()
    {
        if(spawnPosition == Vector3.zero){
            spawnPosition = transform.position;
        }
        
        EnableFeatures = new HashSet<MovementFeature>();
        EnabledFeatures = new HashSet<ParkourMovementFeature>();
        UiManager = this.AddComponent<UIManager>();
        UiManager.Talker = uiText;
    }

    void Update()
    {
        CheckDeath();

        EnableNewFeature(false);
    }

    void CheckDeath(){
        if(transform.position.y < deathPlaneY){
            deathCount++;
            ResetPlayer();
            EnableNewFeature(true);
        }
    }

    void ResetPlayer()
    {
        if (!movementManager)
        {
            movementManager = GetComponent<PlayerMovementManager>();
        }

        movementManager.SetVelocity(Vector3.zero);
        transform.position = spawnPosition;
    }

    void EnableNewFeature(bool hasDied)
    {
        if (EnableFeatures.Count == 0) return;
        
        foreach (MovementFeature movementFeature in EnableFeatures.ToList())
        {
            if (EnabledFeatures.Contains(movementFeature.feature))
            {
                EnableFeatures.Remove(movementFeature);
                continue;
            }

            if (!hasDied && !movementFeature.unlockOnCollision)
            {
                continue;
            }

            UpdateEnabledFeatures(movementFeature);

        }
    }
    
    void UpdateEnabledFeatures(MovementFeature movementFeature)
    {
        if (movementFeature.displayMessage) DisplayFeatureMessage(movementFeature);
        EnabledFeatures.Add(movementFeature.feature);
        EnableFeatures.Remove(movementFeature);
        if (movementFeature.action != null)
        {
            movementFeature.action();
        }
    }

    public void DisplayFeatureMessage(MovementFeature movementFeature)
    {
        if (!movementFeature.displayMessage) return;
        UiManager.DisplayText(movementFeature.text, DisplayTime);
        movementFeature.displayMessage = false;
    }
    public Action GetMovementAction(ParkourMovementFeature feature)
    {
        switch (feature)
        {
            case ParkourMovementFeature.EnableJump:
                return EnableJump;
            case ParkourMovementFeature.EnableDoubleJump:
                return EnabledDoubleJump;
            case ParkourMovementFeature.EnableCrouch:
                return EnableCrouch;
            case ParkourMovementFeature.EnableSlopeLimit:
                return EnableSlope;
            case ParkourMovementFeature.EnableDash:
                return EnableDash;
            case ParkourMovementFeature.EnableSlide:
                return EnableSlide;
            default:
                return null;
        }
    }

    void EnableJump()
    {
        Jumping jumpFeature = GetComponent<Jumping>();

        if(jumpFeature && jumpFeature.Disabled == true){
            jumpFeature.Disabled = false;
        }
    }

    void EnabledDoubleJump()
    {
        Jumping jumpFeature = GetComponent<Jumping>();

        if (jumpFeature && jumpFeature.Disabled == false){
            jumpFeature.MaxJumpCount = 2;
        }
    }

    void EnableCrouch()
    {
        Crouching crouchFeature = GetComponent<Crouching>();

        if (crouchFeature && crouchFeature.Disabled == true)
        {
            crouchFeature.Disabled = false;
        }
    }

    void EnableSlope()
    {
        KinematicCharacterController kcc = GetComponent<KinematicCharacterController>();
        if (kcc && kcc.SlopeLimit < 60)
        {
            kcc.SlopeLimit = 60;
        }
    }

    void EnableDash()
    {
        Dashing dash = GetComponent<Dashing>();
        if (dash && dash.Disabled == true)
        {
            dash.Disabled = false;
        }
    }

    void EnableSlide()
    {
        Sliding slide = GetComponent<Sliding>();
        if(slide && slide.Disabled == true)
        {
            slide.Disabled = false;
        }
    }
}
