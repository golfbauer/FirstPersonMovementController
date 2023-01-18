using System;
using System.Collections;
using UnityEngine;
using static ParkourUtils;

namespace Assets.Parkour_Game.Scripts
{
    public class MovementFeature
    {
        public ParkourMovementFeature Feature;
        public bool EnableOnCollision;
        public Action Action;
        public string MessageOnEnable;
        public float DisplayTime;

        public MovementFeature(ParkourMovementFeature feature, bool enableOnCollision, Action action, float displayTime = 5f)
        {
            this.Feature = feature;
            this.EnableOnCollision = enableOnCollision;
            this.Action = action;
            MessageOnEnable = GetFeatureText(feature);
            DisplayTime = displayTime;
        }
        
    }
}