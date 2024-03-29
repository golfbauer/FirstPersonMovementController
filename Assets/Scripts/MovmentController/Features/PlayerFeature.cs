﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerFeature : MonoBehaviour
{
    // All Keys that will be checked before performing action
    public List<KeyCode> ActionKeys { get; set; }

    // Identifies Feature for manager
    public string Identifier { get; set; }

    // Checks as long as action is executed
    public bool IsExecutingAction { get; set; }

    // Will disable Feature, controlled through the manager
    public bool Disabled { get; set; }

    // If this flag is set true the Feature will be executed once
    public bool Execute { get; set; }

    // Pause the execution of the feature
    public bool Pause { get; set; }

    public bool DebugFeature { get; set; }

    // Will not execute of one of the features is not active
    public List<string> RequiredFeatures { get; set; }

    // Will not execute if one feature is active
    public List<string> ExcludingFeatures { get; set; }

    // Features that will be disabled during Execution
    public List<string> DisableFeatures { get; set; }

    // Time since the action was last executed, will reset on action executed
    protected float elapsedSinceLastExecution;

    // Time since start of execution, will reset when execution finishes
    protected float elapsedSinceStartExecution;

    // Final movement passed on to the manager
    protected Vector3 velocity;

    // Manager that handles the features
    protected PlayerMovementManager manager;

    // CameraController
    public CameraController CameraController;


    /// <summary>
    /// Start method called once and init manager
    /// If these are not needed this method shold be overwritten.
    /// </summary>
    protected virtual void Start()
    {
        InitManager();
    }

    /// <summary>
    /// Initializes Manager
    /// </summary>
    /// <exception cref="System.NullReferenceException"></exception>
    protected void InitManager()
    {
        if (manager == null)
        {
            manager = GetComponent<PlayerMovementManager>();
        }
        if (manager == null)
        {
            throw new System.NullReferenceException(
               "Could not get manager for Feature " + Identifier + ". If you dont need the manager overwrite Start()"
               );
        }
    }

    /// <summary>
    /// Gets called by manager
    /// </summary>
    public abstract void CheckAction();

    /// <summary>
    /// Checks if the action can be executed
    /// </summary>
    /// <returns>True if action will be executed</returns>
    protected virtual bool CanExecute()
    {
        return true;
    }

    /// <summary>
    /// Initializes run, will only be called once when isExecutingAction
    /// </summary>
    protected virtual void Init() { }

    /// <summary>
    /// Does the calculation and returns the current movement
    /// </summary>
    /// <returns>Calculated velocity</returns>
    protected virtual void ExecuteAction() { }

    /// <summary>
    /// Update the elapsed Since timers
    /// </summary>
    protected virtual void UpdateElapsedSince()
    {
        if (IsExecutingAction)
        {
            elapsedSinceStartExecution += Time.deltaTime;
            elapsedSinceLastExecution = 0;
            return;
        }

        elapsedSinceLastExecution += Time.deltaTime;
        elapsedSinceStartExecution = 0;
    }

    /// <summary>
    /// Checks if all Input Keys have been hold down
    /// </summary>
    /// <returns>True if all keys pressed</returns>
    protected bool CheckAllInputGetKeys()
    {
        foreach (KeyCode key in ActionKeys)
        {
            if (!Input.GetKey(key)) return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if all Input Keys have been pressed
    /// </summary>
    /// <returns>True if all keys pressed</returns>
    protected bool CheckAllInputGetKeysDown()
    {
        foreach (KeyCode key in ActionKeys)
        {
            if (!Input.GetKeyDown(key)) return false;
        }
        return true;
    }

    /// <summary>
    /// Check if all Input Keys have been released
    /// </summary>
    /// <returns></returns>
    protected bool CheckAllInputGetKeysUp()
    {
        foreach (KeyCode key in ActionKeys)
        {
            if (!Input.GetKeyUp(key)) return false;
        }
        return true;
    }

    /// <summary>
    /// Is used by CanExecute and can be overwritten to change the Key input check
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckKeys()
    {
        return CheckAllInputGetKeysDown();
    }

    /// <summary>
    /// Checks  if if one of passed features is active
    /// </summary>
    /// <param name="features">List of feature keys</param>
    /// <returns>True if one feature is active</returns>
    protected bool CheckIfFeatureActive(List<string> features)
    {
        if (features == null) return false;

        foreach (string feature in features)
        {
            if (manager.IsFeatureActive(feature)) return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if all passed features are active
    /// </summary>
    /// <param name="features">List of feature keys</param>
    /// <returns>True if all features are active</returns>
    protected bool CheckIfFeaturesActive(List<string> features)
    {
        if (features == null) return true;

        foreach (string feature in features)
        {
            if (!manager.IsFeatureActive(feature)) return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if all required features are currently active
    /// </summary>
    /// <returns> True if all features active</returns>
    protected virtual bool CheckRequiredFeatures()
    {
        return CheckIfFeaturesActive(RequiredFeatures);
    }

    /// <summary>
    /// Checks if at least one excluding feature is active.
    /// </summary>
    /// <returns>Returns true if at least one feature active</returns>
    protected virtual bool CheckExcludingFeatures()
    {
        return CheckIfFeatureActive(ExcludingFeatures);
    }

    /// <summary>
    /// Disables features listed in DisabelingFeatures
    /// </summary>
    protected virtual void DisableGivenFeatures()
    {
        if (DisableFeatures == null) return;
        manager.DisableFeatures(DisableFeatures);
    }

    /// <summary>
    /// Enables features listed in DisabelingFeatures
    /// </summary>
    protected virtual void EnableFeatures()
    {
        if (DisableFeatures == null) return;
        manager.EnableFeatures(DisableFeatures);
    }
    
    /// <summary>
    /// Will Debug the feature while its running
    /// </summary>
    public virtual void DebugFeatureOnActive()
    {
        if (!DebugFeature) return;

        Debug.Log("Feature " + Identifier + " is active");
        Debug.Log("Elapsed Since Start Execution: " + elapsedSinceStartExecution);
        Debug.Log("Velocity: " + velocity);
        Debug.Log("---------------------------------");
        Debug.Log("---------------------------------");
    }
}

