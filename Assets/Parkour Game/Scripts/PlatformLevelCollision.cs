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
        
        [SerializeField] private bool unlockOnCollision;
        [SerializeField] private int hitsNeededToEnable;
        [SerializeField] private int hitsNeededToDisplayMessage;
        [SerializeField] private ParkourMovementFeature enableFeature;
        [SerializeField] private ParkourMovementFeature[] requiredFeatures;

        private bool hitPlatform;
        private int hitCount = 0;
        private ParkourGameManager parkourGameManager;
        private MovementFeature movementFeature = null;
        

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (hitPlatform)
                return;
            
            if (other.gameObject.CompareTag("Player"))
            {
                hitCount++;
                if (!parkourGameManager)
                {
                    parkourGameManager = other.gameObject.GetComponent<ParkourGameManager>();
                }
                if (!parkourGameManager) return;
                if (movementFeature == null) InitMovementFeature();
                if (movementFeature != null)
                {
                    EnableMessage();
                    EnableFeature();
                }
            }
        }

        protected virtual void InitMovementFeature()
        {
            if (!CheckRequiredFeatures() || !CheckEnabledFeatures())
                return;

            Action action = parkourGameManager.GetMovementAction(enableFeature);
            movementFeature = new MovementFeature(enableFeature, unlockOnCollision, action);
        }

        protected virtual void EnableFeature()
        {
            if (CheckHitsNeededToEnable()) return;

            parkourGameManager.EnableFeatures.Add(movementFeature);
        }

        protected virtual void EnableMessage()
        {
            if (!unlockOnCollision || CheckHitsNeededToDisplayMessage()) return;
            parkourGameManager.DisplayFeatureMessage(movementFeature);
        }

        protected virtual bool CheckRequiredFeatures()
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

        protected virtual bool CheckEnabledFeatures()
        {
            foreach (ParkourMovementFeature feature in parkourGameManager.EnabledFeatures)
            {
                if (feature == enableFeature)
                    return false;
            }

            return true;
        }

        protected virtual bool CheckHitsNeededToEnable()
        {
            return hitCount < hitsNeededToEnable;
        }

        protected virtual bool CheckHitsNeededToDisplayMessage()
        {
            return hitCount < hitsNeededToDisplayMessage;
        }
     }
}