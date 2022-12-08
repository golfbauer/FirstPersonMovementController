using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerFeature : MonoBehaviour
{
    // Time since the action was last executed, will reset on action executed
    protected float elapsedSinceLastExecution { get; set; }

    // Time since start of execution, will reset when execution finishes
    protected float elapsedSinceStartExecution { get; set; }

    // Final movement passed on to the manager
    protected Vector3 velocity { get; set; }

    // All Keys that will be checked before performing action
    protected KeyCode[] actionKeys { get; set; }

    // Contains a list of features that will still be checked for actions after this feature performs an action
    public List<string> SupportedFeatures { get; set; }

    // Identifies Feature for manager
    public string Identifier { get; set; }

    // Checks as long as action is executed
    public bool IsExecutingAction { get; set; }

    // Will disable Feature, controlled through the manager
    public bool DisableFeature { get; set; }


    public PlayerMovementManager manager { get; set; }

    private void Start()
    {
        if (manager == null)
        {
            manager = GetComponent<PlayerMovementManager>();
        }
        if (manager == null)
        {
            throw new System.Exception("Couldnt attach manager to " + Identifier);
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
    protected abstract bool CanExecute();

    /// <summary>
    /// Initializes run, will only be called once when isExecutingAction
    /// </summary>
    protected abstract void Init();

    /// <summary>
    /// Does the calculation and returns the current movement
    /// </summary>
    /// <returns>Calculated velocity</returns>
    protected abstract Vector3 ExecuteAction();

    /// <summary>
    /// Update the elapsed Since timers
    /// </summary>
    protected void UpdateElapsedSince()
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
    /// Checks if all Input Keys have been pressed
    /// </summary>
    /// <returns>True if all keys pressed</returns>
    protected bool CheckInputGetKeys()
    {
        foreach(KeyCode key in actionKeys)
        {
            if (!Input.GetKey(key)) return false;
        }
        return true;
    }

    /// <summary>
    /// Checks  if player is in one of the passed states
    /// </summary>
    /// <param name="states">List of states player can be in</param>
    /// <returns>True if player is in one of the states</returns>
    protected bool IsPlayerInOneOfStates(List<string> states)
    {
        foreach(string state in states)
        {
            if (manager.IsFeatureActive(state)) return true;
        }

        return false;
    }
}

