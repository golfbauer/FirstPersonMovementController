using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public abstract class PlayerFeature
{
    // Time since the action was last executed, will reset on action executed
    float elapsedSinceLastExecution;
    // Time since start of execution, will reset when execution finishes
    float elapsedSinceStartExecution;

    // Identifies Feature for manager
    string identifier;

    // Final movement passed on to the manager
    Vector3 movement;

    // Contains a list of features that will still be checked for actions after this feature performs an action
    string[] supportedFeatures;

    // Checks as long as action is executed
    bool isExecutingAction;

    // All Keys that will be checked before performing action
    KeyCode[] actionKeys;

    // Will disable Feature, controlled through the manager
    bool disableFeature;


    public PlayerFeature()
    {
    }

    // Gets called by manager and returns movement
    public abstract Vector3 CheckAction();

    // Checks if the action can be performed
    public abstract bool CanExecute();

    // Initializes run, will only be called once when isExecutingAction
    public abstract void Init();

    // Does the calc and returns the current movement
    public abstract Vector3 ExecuteAction();

}

