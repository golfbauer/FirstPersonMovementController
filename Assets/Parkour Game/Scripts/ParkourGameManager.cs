using Assets.Parkour_Game.Scripts;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ParkourUtils;

public class ParkourGameManager : MonoBehaviour
{
    [Header("Player Configs")]
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private float deathPlaneY;

    [Header("UI")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject ingameUI;
    [SerializeField] private GameObject settings;

    public Vector3 SpawnPoint
    {
        get => spawnPosition;
        set
        {
            spawnPosition = value;
        }
    }
    public HashSet<MovementFeature> EnableFeatures;
    public HashSet<ParkourMovementFeature> EnabledFeatures;
    public UIManager UiManager;
    public bool EasyModeEnabled;
    public UnityEvent resetGame = new UnityEvent();
    
    private bool playerHasDied;
    private Vector3 initSpawnPosition;
    private PlayerMovementManager movementManager;


    void Start()
    {
        if(spawnPosition == Vector3.zero){
            spawnPosition = transform.position;
        }
        initSpawnPosition = spawnPosition;
        
        EnableFeatures = new HashSet<MovementFeature>();
        EnabledFeatures = new HashSet<ParkourMovementFeature>();
        UiManager = this.AddComponent<UIManager>();
        UiManager.PauseUI = pauseUI;
        UiManager.IngameUI = ingameUI;
        UiManager.Settings = settings;
    }

    void Update()
    {
        CheckDeath();

        EnableNewFeature();
    }

    void CheckDeath(){
        if(transform.position.y < deathPlaneY){
            playerHasDied = true;
            ResetPlayer();
            return;
        }

        playerHasDied = false;
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

    void EnableNewFeature()
    {
        if (EnableFeatures.Count == 0) return;
        
        foreach (MovementFeature movementFeature in EnableFeatures.ToList())
        {
            if (EnabledFeatures.Contains(movementFeature.Feature))
            {
                EnableFeatures.Remove(movementFeature);
                continue;
            }

            if (!playerHasDied && !movementFeature.EnableOnCollision)
            {
                continue;
            }

            UpdateEnabledFeatures(movementFeature);
        }
    }
    
    void UpdateEnabledFeatures(MovementFeature movementFeature)
    {
        if (movementFeature.Action == null)
        {
            Debug.Log("No action found for new Feature: " + movementFeature.Feature);
            return;
        }

        DisplayFeatureMessage(movementFeature);
        movementFeature.Action();

        EnabledFeatures.Add(movementFeature.Feature);
        EnableFeatures.Remove(movementFeature);
    }

    public bool IsFeatureEnabled(ParkourMovementFeature movementFeature)
    {
        return EnabledFeatures.Contains(movementFeature);
    }

    public void DisplayFeatureMessage(MovementFeature movementFeature)
    {
        UiManager.DisplayInfoText(movementFeature.MessageOnEnable, movementFeature.DisplayTime);
    }

    public void DisplayMessage(string text, float time)
    {
        UiManager.DisplayInfoText(text, time);
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
            case ParkourMovementFeature.EnableWallRun:
                return EnableWallRun;
            case ParkourMovementFeature.EnableGrapple:
                return EnableGrapple;
            case ParkourMovementFeature.EnableJetpack:
                return EnableJetpack;
            default:
                return null;
        }
    }

    void EnableFeature(PlayerFeature feature)
    {
        if (!feature) return;

        feature.Disabled = false;
    }

    void EnableJump()
    {
        Jumping jumpFeature = GetComponent<Jumping>();

        EnableFeature(jumpFeature);
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

        EnableFeature(crouchFeature);
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
        Dashing dashFeature = GetComponent<Dashing>();
        EnableFeature(dashFeature);
    }

    void EnableSlide()
    {
        Sliding slideFeature = GetComponent<Sliding>();
        EnableFeature(slideFeature);
    }

    void EnableWallRun()
    {
        WallRunning wallRun = GetComponent<WallRunning>();
        WallJumping wallJump = GetComponent<WallJumping>();
        EnableFeature(wallRun);
        EnableFeature(wallJump);
    }

    void EnableGrapple()
    {
        Grappling grapple = GetComponent<Grappling>();
        EnableFeature(grapple);
    }
    
    void EnableJetpack()
    {
        Jetpack jetpack = GetComponent<Jetpack>();
        UiManager.JetpackActive = true;
        EnableFeature(jetpack);
    }

    public void EnableAllFeatures()
    {
        EnableFeatures.Clear();
        EnabledFeatures.Clear();
        EnableJump();
        EnabledDoubleJump();
        EnableCrouch();
        EnableSlope();
        EnableDash();
        EnableSlide();
        EnableWallRun();
        EnableGrapple();
        EnableJetpack();
    }

    public void DisableAllFeatures()
    {
        Jumping jumpFeature = GetComponent<Jumping>();
        Crouching crouchFeature = GetComponent<Crouching>();
        Dashing dashFeature = GetComponent<Dashing>();
        Sliding slideFeature = GetComponent<Sliding>();
        WallRunning wallRun = GetComponent<WallRunning>();
        WallJumping wallJump = GetComponent<WallJumping>();
        Grappling grapple = GetComponent<Grappling>();
        Jetpack jetpack = GetComponent<Jetpack>();

        jumpFeature.Disabled = true;
        jumpFeature.MaxJumpCount = 1;
        crouchFeature.Disabled = true;
        dashFeature.Disabled = true;
        slideFeature.Disabled = true;
        wallRun.Disabled = true;
        wallJump.Disabled = true;
        grapple.Disabled = true;
        jetpack.Disabled = true;
    }

    public void ResetGame()
    {
        EnableFeatures.Clear();
        EnabledFeatures.Clear();
        DisableAllFeatures();
        EasyModeEnabled = false;
        spawnPosition = initSpawnPosition;
        transform.position = spawnPosition;
    }
}
