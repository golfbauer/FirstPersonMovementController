﻿using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour
{
    public const float Epsilon = 0.001f;
    public const float MaxAngleShoveRadians = 90f;

	public static PlayerCameraLook CreatePlayerCameraLook(
        GameObject target,
        Transform playerTransform,
        float mouseSensitivity,
        float timeToTiltCameraWallRun,
        float maxCameraTilt
        )
    {
        PlayerCameraLook mouseLook = target.AddComponent<PlayerCameraLook>();
        mouseLook.MouseSensitivity = mouseSensitivity;
        mouseLook.PlayerTransform = playerTransform;
        mouseLook.TimeToTiltCameraWallRun = timeToTiltCameraWallRun;
        mouseLook.MaxCameraTilt = maxCameraTilt;

        return mouseLook;
    }

    public static PlayerMovement CreateMovement(
        GameObject target,
        GameObject playerCamera,
        float moveSpeed, 
        float sprintSpeed, 
        KeyCode sprintKey, 
        KeyCode jumpKey,
        float crouchHeight, 
        float standingHeight, 
        float timeToCrouch, 
        Vector3 crouchingCenter, 
        Vector3 standingCenter,
        KeyCode crouchKey,
        Vector3 gravity,
        float jumpForce,
        int countAllowedJumps,
        KeyCode slideKey,
        float slideSpeed,
        float timeSlide,
        bool canCancelSlide,
        float slideControll,
        float wallRunSpeed,
        float wallRunMaxAngle,
        int wallRunLayer,
        float maxTimeOnWall,
        float wallRunGravityMultiplier,
        float wallRunMinimumHeight,
        KeyCode wallRunKey,
        Vector2 wallJumpForce,
        float wallPushForce,
        bool canChangeWallJumpDirect,
        KeyCode wallJumpKey,
        Vector2 crouchHeadBobWalk,
        Vector2 crouchHeadBobSprint,
        Vector2 crouchHeadBobDefault,
        Vector2 sprintHeadBob,
        Vector2 walkHeadBob,
        Vector2 defaultHeadBob,
        int grappleLayer,
        float grappleCoolDown,
        float grappleSpeed,
        float maxGrappleDistance,
        KeyCode grappleKey,
        bool canCancelGrapple,
        float dashSpeed,
        float maxDashTime,
        float dashControll,
        int maxDashCount,
        KeyCode dashKey
        )
    {
        PlayerMovement movement = target.AddComponent<PlayerMovement>();
        movement.PlayerCamera = playerCamera;
        movement.MoveSpeed = moveSpeed;
        movement.SprintSpeed = sprintSpeed;
        movement.SprintKey = sprintKey;
        movement.JumpKey = jumpKey;
        movement.CrouchHeight = crouchHeight;
        movement.StandingHeight = standingHeight;
        movement.TimeToCrouch = timeToCrouch;
        movement.CrouchingCenter = crouchingCenter;
        movement.StandingCenter = standingCenter;
        movement.CrouchKey = crouchKey;
        movement.Gravity = gravity;
        movement.JumpForce = jumpForce;
        movement.CountAllowedJumps = countAllowedJumps;
        movement.SlideKey = slideKey;
        movement.SlideSpeed = slideSpeed;
        movement.TimeSlide = timeSlide;
        movement.CanCancelSlide = canCancelSlide;
        movement.SlideControl = slideControll;

        movement.WallRunSpeed = wallRunSpeed;
        movement.WallRunMaxAngle = wallRunMaxAngle;
        movement.WallRunLayer = wallRunLayer;
        movement.MaxTimeOnWall = maxTimeOnWall;
        movement.WallRunGravityMultiplier = wallRunGravityMultiplier;
        movement.WallRunMinimumHeight = wallRunMinimumHeight;
        movement.WallRunKey = wallRunKey;   

        movement.WallJumpForce = wallJumpForce;
        movement.WallPushForce = wallPushForce;
        movement.CanChangeWallJumpDirect = canChangeWallJumpDirect;
        movement.WallJumpKey = wallJumpKey;

        movement.CrouchHeadBobDefault = crouchHeadBobDefault;
        movement.CrouchHeadBobWalk = crouchHeadBobWalk;
        movement.CrouchHeadBobSprint = crouchHeadBobSprint;
        movement.DefaultHeadBob = defaultHeadBob;
        movement.WalkHeadBob = walkHeadBob;
        movement.SprintHeadBob = sprintHeadBob;

        movement.GrappleLayer = grappleLayer;
        movement.GrappleCoolDown = grappleCoolDown;
        movement.GrappleSpeed = grappleSpeed;
        movement.MaxGrappleDistance = maxGrappleDistance;
        movement.GrappleKey = grappleKey;
        movement.CanCancelGrapple = canCancelGrapple;

        movement.DashSpeed = dashSpeed;
        movement.MaxDashTime = maxDashTime;
        movement.DashControll = dashControll;
        movement.MaxDashCount = maxDashCount;
        movement.DashKey = dashKey;

        return movement;
    }

    public static KinematicCharacterController CreateKinemeticCharacterController(
        GameObject target, 
        float slopeLimit, 
        float stairOffset,
        float stairSnapdownDistance,
        Vector3 center,
        float height, 
        float radius,
        float anglePower,
        float maxBounces
        )
    {
        KinematicCharacterController controller = target.AddComponent<KinematicCharacterController>();
        controller.SlopeLimit = slopeLimit;
        controller.StairOffset = stairOffset;
        controller.StairSnapdownDistance = stairSnapdownDistance;
        controller.Center = center;
        controller.Height = height;
        controller.Radius = radius;
        controller.AnglePower = anglePower;
        controller.MaxBounces = maxBounces;
        return controller;
    }

    public enum WallRunDirect
    {
        Right,
        Left,
        Up,
        Down,
        Stop
    }

    public class Features
    {
        public const string Walking = "Walking";
        public const string Sprinting = "Sprinting";
        public const string Jumping = "Jumping";
        public const string Crouching = "Crouching";
        public const string Sliding = "Sliding";
        public const string Dashing = "Dashing";
        public const string WallJumping = "WallJumping";
        public const string WallRunning = "WallRunning";
        public const string Grappling = "Grappling";
        public const string Jetpack = "Jetpack";
        public const string Headbob = "Headbob";
    }
}

