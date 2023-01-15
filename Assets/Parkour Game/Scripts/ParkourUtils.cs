using System;
using Unity.VisualScripting;
using UnityEngine;

public class ParkourUtils : MonoBehaviour
{
    public static float DisplayTime = 5f;

    public static string EnableJumpText = "Wow! Looks like you died due to a skill issue! Please try and use the space bar to Jump!";
    public static string EnabledDoubleJumpText = "That is unfortunate! You know that you have a double jump right? Just press Space again!";
    public static string EnableCrouchText = "Why didnt you try to go under that obstacle? Just crouch by pressing the C key!";
    public static string EnableSlopeText = "You can climb up slopes! Just press the W key to go up!";
    public static string EnableDashText = "Congrats you unlocked a dash! You can dash by Jumping and pressing the Left Shift key!";
    public static string EnableSlideText = "Alright you can use the slide to go below those things. But dont you dare go around the obstacles! Just press the Left Shift key while sprinting!";

    public static string[] MovementFeatureTexts = new string[]
    {
        EnableJumpText,
        EnabledDoubleJumpText,
        EnableCrouchText,
        EnableSlopeText,
        EnableDashText,
        EnableSlideText
    };

    public static string GetFeatureText(ParkourMovementFeature feature)
    {
        return MovementFeatureTexts[(int)feature];
    }
    
    public enum ParkourMovementFeature
    {
        EnableJump = 0,
        EnableDoubleJump = 1,
        EnableCrouch = 2,
        EnableSlopeLimit = 3,
        EnableDash = 4,
        EnableSlide = 5
    }
}
