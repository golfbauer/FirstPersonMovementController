using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{

    public KinematicCharacterController Kcc { get; private set; }

    private Vector3 velocity;
    private Vector3 movement;

    private Vector2 HorizontalVelocity
    {
        get { return new Vector2(velocity.x, velocity.z); }
        set { velocity.x = value.x; velocity.z = value.y; }
    }

    public float GroundedVelocityDeclineRate { get; set; }
    public float AirborneVelocityDeclineRate { get; set; }
    public Vector3 BaseGravity { get; set; }
    public float GravityMultiplier { get; set; } = 1;
    public Vector3 Gravity
    {
        get { return BaseGravity * GravityMultiplier; }
        set { GravityMultiplier = value.magnitude / BaseGravity.magnitude; }
    }
    public bool ProjectOnPlane { get; set; } = true;

    // List of features that have been added to the controller
    private Dictionary<string, PlayerFeature> features;
    // IDs of features that are currently active
    private List<string> activeFeatures;

    RaycastHit groundHit;

    private void Awake()
    {
        Kcc = GetComponent<KinematicCharacterController>();

        features = new Dictionary<string, PlayerFeature>();
        activeFeatures = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        movement = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        if (velocity.y <= 0)
        {
            ProjectOnPlane= true;  
        }

        ApplyFriction();
        ApplyGravity();

        foreach (PlayerFeature feature in features.Values)
        {
            feature.CheckAction();
            if (feature.IsExecutingAction)
            {
                activeFeatures.Add(feature.Identifier);
                continue;
            }
            activeFeatures.Remove(feature.Identifier);
        }

        if (IsGrounded() && ProjectOnPlane && velocity.y > 0 )
        {
            velocity.y= 0;
        }

        movement = velocity * Time.deltaTime;
        if (ProjectOnPlane)
        {
            movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
        }
        transform.position = Kcc.MovePlayer(movement);

    }

    private void ApplyFriction()
    {
        if (IsGrounded())
        {
            HorizontalVelocity = HorizontalVelocity.normalized * Math.Max(0, (HorizontalVelocity.magnitude - (GroundedVelocityDeclineRate * Time.deltaTime)));
        } 
        else
        {
            HorizontalVelocity = HorizontalVelocity.normalized * Math.Max(0, (HorizontalVelocity.magnitude - (AirborneVelocityDeclineRate * Time.deltaTime)));
        }
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && velocity.y < 0) 
        {
            velocity.y = 0;
        }

        velocity += Gravity * Time.deltaTime;
    }

    /// <summary>
    /// Adds Velocity to PlayerMovementManager.velocity up to the specified maxSpeed
    /// </summary>
    /// <param name="velocity">Velocity to add. This value should be in units/s and not relative to Time.deltaTime</param>
    /// <param name="maxSpeed">Maximum magnitude of horizontal (x and z) part of the PlayerMovementManager.velocity vector. This method will add up the the specified value but won't reduce the velocity if it is already higher.</param>
    public void AddVelocity(Vector3 velocityDelta, float maxSpeed) 
    {
        float startSpeed = HorizontalVelocity.magnitude;

        velocity += velocityDelta * Time.deltaTime;
       
        if (startSpeed > maxSpeed && HorizontalVelocity.magnitude > startSpeed)
        {
            HorizontalVelocity = HorizontalVelocity.normalized * startSpeed;
        } 
        if (startSpeed < maxSpeed && HorizontalVelocity.magnitude > maxSpeed)
        {
            HorizontalVelocity = HorizontalVelocity.normalized * maxSpeed;
        }
    }

    /// <summary>
    /// Adds Velocity directly to PlayerMovementManager.velocity up to the specified maxSpeed. 
    /// This is inteded for forces that are applied instantaneously (like a jump or push) as 
    /// opposed to forces that are applyied continuously (like walking).
    /// </summary>
    /// <param name="velocity">Velocity to add.</param>
    public void AddRawVelocity(Vector3 velocityDelta)
    {
        velocity += velocityDelta;
    }

    /// <summary>
    /// Sets velocity to the specified Value
    /// </summary>
    /// <param name="velocity"></param>
    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public Vector3 GetVelocity() 
    { 
        return this.velocity;
    }

    /// <summary>
    /// Determens wether the player is currently staning on ground.
    /// </summary>
    /// <returns>True if there is an object <= 0.1f below the player or if player is standing on slope with an angle greater then slope limit; otherwise false</returns>
    public bool IsGrounded()
    {
        return velocity.y <= 0 && Kcc.CheckGrounded(out groundHit);
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

    public void SetFeatureActive(string featureId)
    {
        if (!IsFeatureAdded(featureId)) {
            throw new ArgumentException("Feature " + featureId + " needs to be added to manager before it can be set active.");
        }

        activeFeatures.Add(featureId);
    }

    public void SetFeatureNotActive(string featureId)
    {
        activeFeatures.Remove(featureId);
    }

}
