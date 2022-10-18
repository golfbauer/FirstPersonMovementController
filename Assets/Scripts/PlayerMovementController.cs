
// Copyright (C) 2022 Nicholas Maltbie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Capsule Collider")]
    [SerializeField] private float slopeLimit;
    [SerializeField] private float stepOffset;
    [SerializeField] private float jumpingStepOffset = 0.1f;
    [SerializeField] private float skinWidth;
    [SerializeField] private Vector3 center;
    [SerializeField] private float height;
    [SerializeField] private float radius;

    [Header("Player Components")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject playerCamera;

    [Header("Mouse Configurations")]
    [SerializeField] private float mouseSensivity = 100f;

    [Header("Movement Configurations")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float sprintSpeed = 40f;

    [Header("Jumping Configurations")]
    [SerializeField] private float jumpForce = 1.0f;

    [Header("Crouch Configurations")]
    [SerializeField] private float crouchHeight;
    [SerializeField] private float standingHeight;
    [SerializeField] private float timeToCrouch;
    [SerializeField] private Vector3 crouchingCenter;
    [SerializeField] private Vector3 standingCenter;

    [Header("Environmental Configurations")]
    [SerializeField] private Vector3 gravity = new Vector3(0, -9.81f, 0);

    [Header("Key Bindings")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    private PlayerCameraLook mouseLookCamera;
    private PlayerMovement playerMovement;
    private PlayerPhysics playerPhysics;
    private KinematicCharacterController controller;

    private CapsuleCollider capsuleCollider;
    private Rigidbody rigidbody;
    private Vector3 velocity;
    private int maxBounces = 5;
    public const float Epsilon = 0.001f;
    public const float MaxAngleShoveRadians = 90f;
    public float anglePower = 0.5f;


    private void Awake()
    {
        //    controller = Utils.CreateCapsuleCharacterController(this.gameObject, slopeLimit, stepOffset, jumpingStepOffset, skinWidth, center, height, radius);
        mouseLookCamera = Utils.CreateMouseLook(playerCamera, playerTransform, mouseSensivity);
        //    playerMovement = Utils.CreateMovement(
        //        this.gameObject,
        //        controller,
        //        moveSpeed, 
        //        sprintSpeed, 
        //        sprintKey, 
        //        jumpKey,
        //        crouchHeight,
        //        standingHeight,
        //        timeToCrouch,
        //        crouchingCenter,
        //        standingCenter,
        //        crouchKey
        //        );
        //    playerPhysics = Utils.CreatePhysics(this.gameObject, controller, playerTransform, gravity, jumpForce);
    }

    private void Start()
    {
        rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.height = height;
        capsuleCollider.radius = radius;
        capsuleCollider.center = center;
    }

    private void Update()
    {
        bool falling = !CheckGrounded(out RaycastHit groundHit);
        if (falling)
        {
            velocity += gravity * Time.deltaTime;
        } else
        {
            velocity = Vector3.zero;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirect = transform.right * moveX + transform.forward * moveZ;

        Vector3 movement = moveDirect * moveSpeed * Time.deltaTime;

        if (!falling)
        {
            movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
        }

        // Attempt to move the player based on player movement
        transform.position = MovePlayer(movement);

        // Move player based on falling speed
        transform.position = MovePlayer(velocity * Time.deltaTime);
    }

    private Vector3 MovePlayer(Vector3 movement)
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        Vector3 remaining = movement;

        int bounces = 0;

        while (bounces < maxBounces && remaining.magnitude > Epsilon)
        {
            // Do a cast of the collider to see if an object is hit during this
            // movement bounce
            float distance = remaining.magnitude;
            if (!CastSelf(position, rotation, remaining.normalized, distance, out RaycastHit hit))
            {
                // If there is no hit, move to desired position
                position += remaining;

                // Exit as we are done bouncing
                break;
            }

            // If we are overlapping with something, just exit.
            if (hit.distance == 0)
            {
                break;
            }

            float fraction = hit.distance / distance;

            // Set the fraction of remaining movement (minus some small value)
            position += remaining * (fraction);
            // Push slightly along normal to stop from getting caught in walls
            position += hit.normal * Epsilon * 2;
            // Decrease remaining movement by fraction of movement remaining
            remaining *= (1 - fraction);

            // Plane to project rest of movement onto
            Vector3 planeNormal = hit.normal;

            // Only apply angular change if hitting something
            // Get angle between surface normal and remaining movement
            float angleBetween = Vector3.Angle(hit.normal, remaining) - 90.0f;

            // Normalize angle between to be between 0 and 1
            // 0 means no angle, 1 means 90 degree angle
            angleBetween = Mathf.Min(MaxAngleShoveRadians, Mathf.Abs(angleBetween));
            float normalizedAngle = angleBetween / MaxAngleShoveRadians;

            // Reduce the remaining movement by the remaining movement that ocurred
            remaining *= Mathf.Pow(1 - normalizedAngle, anglePower) * 0.9f + 0.1f;

            // Rotate the remaining movement to be projected along the plane 
            // of the surface hit (emulate pushing against the object)
            Vector3 projected = Vector3.ProjectOnPlane(remaining, planeNormal).normalized * remaining.magnitude;

            // If projected remaining movement is less than original remaining movement (so if the projection broke
            // due to float operations), then change this to just project along the vertical.
            if (projected.magnitude + Epsilon < remaining.magnitude)
            {
                remaining = Vector3.ProjectOnPlane(remaining, Vector3.up).normalized * remaining.magnitude;
            }
            else
            {
                remaining = projected;
            }

            // Track number of times the character has bounced
            bounces++;
        }

        return position;
    }

    private bool CheckGrounded(out RaycastHit groundHit)
    {
        // 0.1f = GroundDistance
        bool onGround = CastSelf(transform.position, transform.rotation, Vector3.down, 0.1f, out groundHit);
        float angle = Vector3.Angle(groundHit.normal, Vector3.up);
        // 60f = MaxWalkingAngle
        return onGround && angle < 60f;
    }

    private bool CastSelf(Vector3 pos, Quaternion rot, Vector3 dir, float dist, out RaycastHit hit)
    { 
        // Get Parameters associated with the KCC
        Vector3 center = rot * capsuleCollider.center + pos;
        float radius = capsuleCollider.radius;
        float height = capsuleCollider.height;

        // Get top and bottom points of collider
        Vector3 bottom = center + rot * Vector3.down * (height / 2 - radius);
        Vector3 top = center + rot * Vector3.up * (height / 2 - radius);

        // Check what objects this collider will hit when cast with this configuration excluding itself
        IEnumerable<RaycastHit> hits = Physics.CapsuleCastAll(
            top, bottom, radius, dir, dist, ~0, QueryTriggerInteraction.Ignore)
            .Where(hit => hit.collider.transform != transform);

        bool didHit = hits.Count() > 0;

        // Find the closest objects hit
        float closestDist = didHit ? Enumerable.Min(hits.Select(hit => hit.distance)) : 0;
        IEnumerable<RaycastHit> closestHit = hits.Where(hit => hit.distance == closestDist);

        // Get the first hit object out of the things the player collides with
        hit = closestHit.FirstOrDefault();

        // Return if any objects were hit
        return didHit;
    }
}
