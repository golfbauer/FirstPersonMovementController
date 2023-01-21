using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovementManager : MonoBehaviour
{

    public KinematicCharacterController Kcc { get; private set; }

    private Vector3 velocity;
    private Vector3 movement;

    private string frozen;
    private float prevGravityMultiplier;
    private string prevGravityMultiplierFeature;

    private Vector2 HorizontalVelocity
    {
        get { return new Vector2(velocity.x, velocity.z); }
        set { velocity.x = value.x; velocity.z = value.y; }
    }

    private int lastParentId;
    private Vector3 lastParentPosition;
    private Vector3 currentParentMovement;
    private bool airborneLastUpdate;
    private float platformJumpCooldown = 0.1f;

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
    public bool PrintDebugInfo { get; set; } = false;

    // List of features that have been added to the controller
    private Dictionary<string, PlayerFeature> features;
    // IDs of features that are currently active
    private HashSet<string> activeFeatures;
    private Dictionary<string, Vector3> velocityDeltas;

    RaycastHit groundHit;

    private void Awake()
    {
        Kcc = GetComponent<KinematicCharacterController>();

        features = new Dictionary<string, PlayerFeature>();
        activeFeatures = new HashSet<string>();
    }

    private void OnEnable() 
    {
        Kcc = GetComponent<KinematicCharacterController>();

        features = new Dictionary<string, PlayerFeature>();
        activeFeatures = new HashSet<string>();

        foreach (PlayerFeature feature in gameObject.GetComponents<PlayerFeature>()) 
        {
            AddFeature(feature.Identifier, feature);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        movement = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        velocityDeltas = new Dictionary<string, Vector3>();

        if(frozen != null) {
            if(this.features.TryGetValue(frozen, out PlayerFeature value)) {
                value.CheckAction();
                return;
            } else {
                frozen = null;
            }
        }
        
        movement = Vector3.zero;

        if (velocity.y <= 0)
        {
            ProjectOnPlane= true;  
        }

        ApplyPlatformMovement();
        ApplyFriction();
        ApplyGravity();

        foreach (PlayerFeature feature in features.Values)
        {
            Vector3 startVelocity = velocity;
            feature.CheckAction();
            if (feature.IsExecutingAction)
            {
                velocityDeltas.Add(feature.Identifier, velocity - startVelocity);
                activeFeatures.Add(feature.Identifier);
                feature.DebugFeatureOnActive();
                continue;
            }
                activeFeatures.Remove(feature.Identifier); 
            }

        if (IsGrounded() && ProjectOnPlane && velocity.y > 0 )
        {
            velocity.y= 0;
        }

        movement = movement + velocity * Time.deltaTime;
        if (ProjectOnPlane)
        {
            movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
        }

        transform.position += Kcc.PushOut();
        transform.position = Kcc.MovePlayer(movement);

        if (PrintDebugInfo) PrintMovementDeltas();

    }

    private void ApplyPlatformMovement()
    {
        if (IsGrounded() && groundHit.distance != 0f)
        {
            transform.SetParent(groundHit.transform, true);
            int currentParentId = groundHit.transform.gameObject.GetInstanceID();

            if (lastParentId == currentParentId && !airborneLastUpdate)
            {
                currentParentMovement = groundHit.transform.position - lastParentPosition;
            }

            lastParentId = currentParentId;
            lastParentPosition = groundHit.transform.position;
            airborneLastUpdate = false;
            platformJumpCooldown = Math.Max(platformJumpCooldown - Time.deltaTime, 0);

        }
        else
        {
            if (!airborneLastUpdate && platformJumpCooldown <= 0)
            {
                AddRawVelocity(currentParentMovement / Time.deltaTime);
                platformJumpCooldown = 0.1f;
            }

            currentParentMovement = Vector3.zero;
            airborneLastUpdate = true;
            transform.SetParent(null);

            if (groundHit.transform != null)
            {
                lastParentId = groundHit.transform.gameObject.GetInstanceID();
                lastParentPosition = groundHit.transform.position;
            }
        }
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

    private void PrintMovementDeltas()
    {
        foreach (KeyValuePair<string, Vector3> delta in velocityDeltas)
        {
            Debug.Log(delta.Key + ": " + delta.Value);
        }
    }

    /// <summary>
    /// Adds Velocity to PlayerMovementManager.velocity up to the specified maxSpeed
    /// </summary>
    /// <param name="velocity">Velocity to add. This value should be in units/s and not relative to Time.deltaTime</param>
    /// <param name="maxSpeed">Maximum magnitude of horizontal (x and z) part of the PlayerMovementManager.velocity vector. This method will add up the the specified value but won't reduce the velocity if it is already higher.</param>
    /// <returns> The total, time relative net difference in velocity</returns>
    public Vector3 AddVelocityHorizontallyCapped(Vector3 velocityDelta, float maxSpeed) 
    {
        Vector3 startVelocity = velocity;
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

        return velocity - startVelocity;
    }

    public Vector3 AddVelocityVerticalyCapped(Vector3 velocityDelta, float maxSpeed)
    {
        Vector3 startVelocity = velocity;
        float startSpeed = velocity.y;

        velocity += velocityDelta * Time.deltaTime;

        if (startSpeed > maxSpeed && velocity.y > startSpeed)
        {
            velocity.y = (Vector3.up * startSpeed).y;
        }
        if (startSpeed < maxSpeed && velocity.y > maxSpeed)
        {
            velocity.y = (Vector3.up * maxSpeed).y;
        }

        return velocity - startVelocity;
    }

    public Vector3 AddVelocityCapped(Vector3 velocityDelta, float maxSpeed)
    {
        Vector3 startVelocity = velocity;
        float startSpeed = velocity.magnitude;

        velocity += velocityDelta * Time.deltaTime;

        if (startSpeed > maxSpeed && velocity.magnitude > startSpeed)
        {
            velocity = startVelocity;
        }
        if (startSpeed < maxSpeed && velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        return velocity - startVelocity;
    }



    /// <summary>
    /// Adds Velocity directly to PlayerMovementManager.velocity. 
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
    /// Returns a PlayerFeature from Manager
    /// <summary>
    public PlayerFeature GetFeature(string featureID)
    {
        if(features.TryGetValue(featureID, out PlayerFeature feature))
        {
            return feature;
        }

        return null;
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

    public List<String> GetExecutingFeatures()
    {
        return (List<String>)(from feature in this.features.Values
                              where feature.IsExecutingAction == true
                              select feature.Identifier);
    }

    /// <summary>
    /// Disables features
    /// </summary>
    /// <param name="featureKeys">List of featuers to be disabled</param>
    public void DisableFeatures(List<string> featureKeys)
    {
        foreach(string feature in featureKeys)
        {
            if (this.features.TryGetValue(feature, out PlayerFeature value))
            {
                value.Disabled = true;
            }
        }
    }

    /// <summary>
    /// Enables features
    /// </summary>
    /// <param name="featureKeys">List of features to be disabled</param>
    public void EnableFeatures(List<string> featureKeys)
    {
        foreach(string featureKey in featureKeys)
        {
            if(features.TryGetValue(featureKey, out PlayerFeature value))
            {
                value.Disabled = false;
            }
        }
    }

    public bool IsFeatureDisabled(string featureId)
    {
        return features[featureId].Disabled;
    }

    public List<String> GetDisabledFatures()
    {
        return (List<string>)(from feature in this.features.Values
               where feature.Disabled
               select feature.Identifier);
    }

    /// <summary>
    /// Freeze player on spot and set velocity to zero
    /// </summary>
    public void Freeze(string featureId){
        this.frozen = featureId;
        SetVelocity(Vector3.zero);
    }

    /// <summary>
    /// Unfreeze player
    /// </summary>
    public void UnFreeze(){
        this.frozen = null;
    }

    /// <summary>
    /// Changes the gravity multiplier temporarly. Can only be done by one feature at a time.
    /// </summary>
    public bool ChangeGravityMultiplier(float multiplier, string featureId)
    {
        if(prevGravityMultiplierFeature != null && prevGravityMultiplierFeature != featureId) return false;

        prevGravityMultiplier = GravityMultiplier;
        prevGravityMultiplierFeature = featureId;
        GravityMultiplier = multiplier;
        return true;
    }

    /// <summary>
    /// Undoes the gravity multiplier change. Can only be done by the feature that made the change.
    /// </summary>
    public bool UndoChangeGravityMultiplier(string featureId)
    {
        if (prevGravityMultiplierFeature != featureId) return false;

        GravityMultiplier = prevGravityMultiplier;
        prevGravityMultiplierFeature = null;
        return true;
    }

    /// <summary>
    /// Returns the velocity that was added by PlayerFeature with given featureId in the current update. 
    /// </summary>
    /// <param name="featureId"> Identifier of feature to be retrieved</param>
    /// <returns> Total change in velocity that occured by feature; Vector3.zero if feature was not found</returns>
    public Vector3 GetVelocityDelta(string featureId)
    {
        if (velocityDeltas.ContainsKey(featureId))
        {
            return velocityDeltas[featureId];
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Returns features and velocity changes for current update
    /// </summary>
    /// <param name="featureId"> Identifier of feature to be retrieved</param>
    /// <returns> A dictornary of all features that were active in the current update containg featureIds and the corresponding net difference in velocity for each feature</returns>
    public Dictionary<string, Vector3> GetVeclocityDeltas()
    {
        return velocityDeltas;
    }
}
