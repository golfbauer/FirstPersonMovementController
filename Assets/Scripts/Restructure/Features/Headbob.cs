using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Headbob : PlayerFeature
{
    public Dictionary<string, Vector2> HeadbobFeatures { get; set; }

    protected Vector2 currentHeadob;
    protected string currentFeature;
    protected float prevBobAmount;
    protected float prevBobSpeed;
    protected bool prevIsCameraTop;
    protected float headBobTimer;


    public override void CheckAction()
    {
        if (CanExecute()) 
        {
            Init();
        }

        if (IsExecutingAction)
        {
            ExecuteAction();
        }
    }

    protected override bool CanExecute()
    {
        if(manager.IsGrounded() && !CheckExcludingFeatures())
        {
            return true;
        }

        currentFeature = null;
        currentHeadob = Vector2.zero;
        return false;
    }

    protected override void Init()
    {
        if (currentFeature != null && manager.IsFeatureActive(currentFeature)) return;

        foreach (string feature in HeadbobFeatures.Keys)
        {   
            if (manager.IsFeatureActive(feature))
            {
                currentHeadob = HeadbobFeatures[feature];
                currentFeature = feature;
                IsExecutingAction = true;
                return;
            }
        }
        currentFeature = null;
        currentHeadob = Vector2.zero;
    }

    protected override void ExecuteAction()
    {
        if (currentHeadob == Vector2.zero)
        {
            FinishPlayerHeadBob();
        }
        else
        {
            prevBobSpeed = currentHeadob.x;
            prevBobAmount = currentHeadob.y;
            prevIsCameraTop = CameraController.transform.localPosition.y > CameraController.LocalStartPosition.y;

            headBobTimer += Time.deltaTime * currentHeadob.x;
            CameraController.HeadBobCamera(headBobTimer, currentHeadob.y);
        }
    }

    void FinishPlayerHeadBob()
    {
        if (
            (prevIsCameraTop && CameraController.transform.localPosition.y <= CameraController.LocalStartPosition.y) ||
            (!prevIsCameraTop && CameraController.transform.localPosition.y >= CameraController.LocalStartPosition.y))
        {
            headBobTimer = 0f;
            IsExecutingAction = false;
            return;
        }
        if (headBobTimer / Mathf.PI > (int)Math.Round(headBobTimer / Mathf.PI))
        {
            float interfaceWithx = (int)Math.Round(headBobTimer / Mathf.PI) * Mathf.PI + Mathf.PI;
            headBobTimer = interfaceWithx - ((headBobTimer / Mathf.PI) % 1) * Mathf.PI;
        }
        headBobTimer += Time.deltaTime * prevBobSpeed;
        CameraController.HeadBobCamera(headBobTimer, prevBobAmount);
    }
}
