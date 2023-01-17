using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class ParkourUtils : MonoBehaviour
{
    public static float DisplayTime = 5f;

    public static string EnableJumpText = "Wow! Looks like you died due to a skill issue! Please try and use the space bar to Jump!";
    public static string EnabledDoubleJumpText = "That is unfortunate! You know that you have a double jump right? Just press Space again!";
    public static string EnableCrouchText = "Why didnt you try to go under that obstacle? Just crouch by pressing the C key!";
    public static string EnableSlopeText = "";
    public static string EnableDashText = "Congrats you unlocked a dash! You can dash by Jumping and pressing the Left Shift key!";
    public static string EnableSlideText = "Alright you can use the slide to go below those things. But dont you dare go around the obstacles! Just press the Left Shift key while sprinting!";
    public static string EnableWallRun = "Alright looks like you need some spicy wallrunning! Just hold the Space bar when you touch a wall and release to jump away!";
    public static string EnableGrapple = "Looks like quite a long gap there! You can use the grapple to get across! Just aim at the wall and press K!";
    public static string EnableJetpack = "Yay you found the jetpack!";

    public static string[] EnableFeatureTexts = new string[]
    {
        EnableJumpText,
        EnabledDoubleJumpText,
        EnableCrouchText,
        EnableSlopeText,
        EnableDashText,
        EnableSlideText,
        EnableWallRun,
        EnableGrapple,
        EnableJetpack,
    };

    public static string[] AdditionalMessages = new string[]
    {
        "You can climb up slopes! Just press the W key to go up!",
        "Oh that didnt work huh? Well you need to aim at the wall and press K to grapple! No hold up its F! My bad!",
        "Hey this looks like quite some distance you have to jump there. Maybe you can use a jetpack. But where can you get one? But if I would have a jetpack I would hold E to use it.",
        "Oi oi oi. These platforms are even further apart. I recommand to cancel your grapple by pressing F again. This way you will keep your momentum!",
    };

    public static string GetFeatureText(ParkourMovementFeature feature)
    {
        return EnableFeatureTexts[(int)feature];
    }

    public static string GetAdditionalMessage(int index)
    {
        return AdditionalMessages[index];
    }

    public enum ParkourMovementFeature
    {
        None = -1,
        EnableJump = 0,
        EnableDoubleJump = 1,
        EnableCrouch = 2,
        EnableSlopeLimit = 3,
        EnableDash = 4,
        EnableSlide = 5,
        EnableWallRun = 6,
        EnableGrapple = 7,
        EnableJetpack = 8,
    }
}
