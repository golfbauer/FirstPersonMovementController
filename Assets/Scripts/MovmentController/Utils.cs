using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour
{
    public const float Epsilon = 0.001f;
    public const float MaxAngleShoveRadians = 90f;

    public enum WallPosition
    {
        Right,
        Left,
        None
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

