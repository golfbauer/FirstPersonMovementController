using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ParkourUtils;
using static Utils;

namespace Assets.Parkour_Game.Scripts
{
    public class PlatformLevelCollision : MonoBehaviour
    {
        [SerializeField] private bool enableOnCollision;
        [SerializeField] private int hitsNeededToEnable;

        [SerializeField] private ParkourMovementFeature enableFeature;
        [SerializeField] private ParkourMovementFeature[] requiredFeatures;
        
        [SerializeField] private int hitsNeededForAdditionalMessage;
        [SerializeField] private int additionalMessageIndex;

        private bool featureEnabled;
        private bool messageDisplayed;
        private int hitCount = 0;
        private ParkourGameManager parkourGameManager;
        private MovementFeature movementFeature = null;
        

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (featureEnabled && messageDisplayed)
                return;
            
            if (other.gameObject.CompareTag("Player"))
            {
                hitCount++;
                if (!parkourGameManager)
                {
                    parkourGameManager = other.gameObject.GetComponent<ParkourGameManager>();
                }
                if (!parkourGameManager) return;
                if (!featureEnabled) EnableFeature();
                if (!messageDisplayed) DisplayMessage();
            }
        }

        protected virtual void EnableFeature()
        {
            if (!CheckRequiredFeatureEnabled() || !CheckFeatureEnabled() || HitsNeededToEnable())
                return;

            Action action = parkourGameManager.GetMovementAction(enableFeature);
            movementFeature = new MovementFeature(enableFeature, enableOnCollision, action, DisplayTime);
            parkourGameManager.EnableFeatures.Add(movementFeature);
            featureEnabled = true;
        }
        
        protected virtual void DisplayMessage()
        {
            if (HitsNeededToDisplayMessage()) return;
            
            string message = GetAdditionalMessage(additionalMessageIndex);
            parkourGameManager.DisplayMessage(message, DisplayTime);
            messageDisplayed = true;
        }

        protected virtual bool CheckRequiredFeatureEnabled()
        {
            if (requiredFeatures == null || requiredFeatures.Length == 0)
                return true;

            foreach (var requiredFeature in requiredFeatures)
            {
                if (!parkourGameManager.EnabledFeatures.Contains(requiredFeature))
                    return false;
            }
            return true;
        }

        protected virtual bool CheckFeatureEnabled()
        {
            foreach (ParkourMovementFeature feature in parkourGameManager.EnabledFeatures)
            {
                if (feature == enableFeature)
                    return false;
            }

            return true;
        }

        protected virtual bool HitsNeededToEnable()
        {
            if (hitsNeededToEnable == 0)
            {
                featureEnabled = true;
                return true;
            }
            
            return hitCount < hitsNeededToEnable;
        }

        protected virtual bool HitsNeededToDisplayMessage()
        {
            if(hitsNeededForAdditionalMessage == 0)
            {
                messageDisplayed = true;
                return true;
            }
            
            return hitCount < hitsNeededForAdditionalMessage;
        }
    }
}