using philae.data.gravity;
using philae.gravity.physicsBody;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.graviton
{
    [Serializable]
    public class GravitonPhysicsApplyer
    {
        [SerializeField]
        private RigidGraviton _rigidGraviton = default;

        private Graviton _gravitonElement;
        private GravitonGravityCalculation _calculation;
        private GravitonSettings _gravitonSettings;

        public void Init(Graviton graviton, GravitonGravityCalculation calculation, GravitonSettings settings)
        {
            _gravitonElement = graviton;
            _gravitonSettings = settings;
            _calculation = calculation;
            _rigidGraviton.Init();
        }

        public void ApplyGravity()
        {
            _rigidGraviton.SaveCurrentSpeed();
            _rigidGraviton.AddForceMassDependent(_calculation.Gravity);
            _rigidGraviton.UpdatePosition();
        }
    }
}