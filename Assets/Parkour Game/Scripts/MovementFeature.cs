using System;
using System.Collections;
using UnityEngine;
using static ParkourUtils;

namespace Assets.Parkour_Game.Scripts
{
    public class MovementFeature
    {
        public ParkourMovementFeature feature;
        public bool unlockOnCollision;
        public Action action;
        public string text;
        public bool displayMessage;

        public MovementFeature(ParkourMovementFeature feature, bool unlockOnCollision, Action action)
        {
            this.feature = feature;
            this.unlockOnCollision = unlockOnCollision;
            this.text = GetFeatureText(feature);
            this.action = action;
            this.displayMessage = true;
        }
        
    }
}