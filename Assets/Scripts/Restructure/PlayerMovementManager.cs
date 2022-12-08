using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{

    private KinematicCharacterController kcc;

    private Vector3 velocity;
    private float groundedVelocityDeclineRate;
    private float airborneVelocityDeclineRate;
    private Vector3 gravity;

    // List of features that have been added to the controller
    private Dictionary<string, PlayerFeature> features;
    // IDs of features that are currently active
    private List<string> activeFeatures;

    RaycastHit groundHit;


    // Start is called before the first frame update
    void Start()
    {
        kcc = GetComponent<KinematicCharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Adds Velocity to PlayerMovementManager.velocity up to the specified maxSpeed
    /// </summary>
    /// <param name="velocity">Velocity to add. This value should be in units/s and not relative to Time.deltaTime</param>
    /// <param name="maxSpeed">Maximum magnitude of the PlayerMovementManager.velocity vector. This method will add up the the specified value but won't reduce the velocity if it is already higher.</param>
    public void AddVelocity(Vector3 velocity, float maxSpeed) 
    {

    }

    public bool IsGrounded()
    {
        return kcc.CheckGrounded(out groundHit);
    }

    /// <summary>
    /// Adds the specified FeatureId and feature to the features Dictionary
    /// </summary>
    /// <param name="featureID">ID of the feature to add</param>
    /// <param name="feature">Feature to add</param>
    public void AddFeature(string featureID, PlayerFeature feature)
    {
        features.Add(featureID, feature);
    }

    /// <summary>
    /// Removes the feature with the specified ID from the features Dictionary
    /// </summary>
    /// <param name="featureID">ID of the feature to be removed</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if ID is not found</returns>


    public bool RemoveFeature(string featureID)
    {
        return features.Remove(featureID);
    }

    /// <summary>
    /// Determines whether the features Dictionary contains the specified ID.
    /// </summary>
    /// <param name="featureId">The ID to locate</param>
    /// <returns>true if the features Dictionary contains an element with the specified ID; otherwise false</returns>
    public bool IsFeatureAdded(string featureId)
    {
        return features.ContainsKey(featureId);
    }

    /// <summary>
    /// Determines whether a feature is currently being executed.
    /// </summary>
    /// <param name="featureId">The ID to check</param>
    /// <returns>true if the feature was active in the last update and has not finished running yet; otherwise false</returns>
    public bool IsFeatureActive(string featureId)
    {
        return activeFeatures.Contains(featureId);
    }

}
