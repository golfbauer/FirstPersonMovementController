using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class ParkourUtils : MonoBehaviour
{
    public static float DisplayTime = 5f;

    public static string EasyModeMessage = "Easy Mode Enabled";

    public static string EnableJumpText = "Wow! Looks like you died due to what's called a skill issue! Please try and use the space bar to Jump!";
    public static string EnabledDoubleJumpText = "That is unfortunate! You know that you have a double jump right? Just press Space again!";
    public static string EnableCrouchText = "Alright I guess you're not the creative kind. Here press C to crouch.";
    public static string EnableSlopeText = "";
    public static string EnableDashText = "Congrats you found an invisble dash! Trust me bro it was there. Just press the left shift key while Jumping!";
    public static string EnableSlideText = "Alright you can use slide to go below those things. Just press the Left Shift key while sprinting. But dont you dare go around! ";
    public static string EnableWallRun = "Alright looks like you need some spicy wallrunning! Just hold the Space bar when you touch a wall and release to jump away!";
    public static string EnableGrapple = "Looks like quite a long gap there! You can use the grapple to get across! Just aim at the wall and press K!";
    public static string EnableJetpack = "Hold E! Go up! Go up!";

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

    public static string AdditionalCrochMessage = "Alright you made it! Now I want you to be creative on this one. You can do it!";
    public static string AdditionalSlopeMessage = "What are you doing? That slope isn't so steep, just walk up!";
    public static string AdditionalGrappleStartMessage = "I swear to use the Grapple you have to press K! Oh wait, no! It's F! My bad!";
    public static string AdditionalGrappleMidMessage = "In case you haven't noticed yet, You can cancel Grapple by pressing F again! It will help you on this part. Wow, I am so nice!";
    public static string AdditionalJetpackMessage = "Oh boi that platform is far away. I am sure you can make that with a simple Jump and Dash. In case you can't, you might want to go down.";
    public static string AdditionalJetpackMessage2 = "Look at the bottom right. There you can see a super well designed fuel meter for your jetpack.";
    public static string EndMessage = "Alright, congrats! I didn't think you would make it this far. But I guess you did. I am proud of you!";

    public static string[] AdditionalMessages = new string[]
    {
        AdditionalCrochMessage,
        AdditionalSlopeMessage,
        AdditionalGrappleStartMessage,
        AdditionalGrappleMidMessage,
        AdditionalJetpackMessage,
        AdditionalJetpackMessage2,
        EndMessage,
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
