using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using hedCommon.time;
using philae.data.gravity;
using philae.gravity.attractor;
using philae.gravity.physicsBody;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.graviton
{
    [Serializable]
    public class GravitonGravityCalculation
    {
        private const int MAX_ATTRACTOR_ON_GRAVITON_PER_FRAME = 5;
        private Graviton _graviton;
        private GravitonZoneLocalizer _gravitonZones;

        private GravitonSettings _gravitonSettings;
        private GravitonSettingsLocal _gravitonSettingsLocal;

        private Vector3 _currentGravityVector = Vector3.zero;
        public Vector3 Gravity { get { return (_currentGravityVector); } }

        private FrequencyCoolDown _frequencyToSearchClosestPoints = new FrequencyCoolDown();

        [SerializeField]
        public struct AttractorInformation
        {
            public Vector3 PointOfAttraction;
            public float Gravity;
            public float SquaredDist;
            public Vector3 NormalizedDirection;

            public bool CanApplyJetPack;
            public float ForceJetPack;
            public Vector3 ForceAccelerationInsideJetPack;

            public AttractorInformation(Vector3 point, float gravity, float squaredDist, Vector3 normalizedDirection, bool isCloseEnoughforJetPack, float forceJetPack, Vector3 forceAccelerationInsideJetPack)
            {
                PointOfAttraction = point;
                Gravity = gravity;
                SquaredDist = squaredDist;
                NormalizedDirection = normalizedDirection;
                CanApplyJetPack = isCloseEnoughforJetPack;
                ForceJetPack = forceJetPack;
                ForceAccelerationInsideJetPack = forceAccelerationInsideJetPack;
            }
        }

        public AttractorInformation[] AttractorForces = new AttractorInformation[MAX_ATTRACTOR_ON_GRAVITON_PER_FRAME];
        private float[] ForceAmount = new float[MAX_ATTRACTOR_ON_GRAVITON_PER_FRAME];
        private float[] ForceJetPackAmount = new float[MAX_ATTRACTOR_ON_GRAVITON_PER_FRAME];
        private float[] ForceAccelerationAmount = new float[MAX_ATTRACTOR_ON_GRAVITON_PER_FRAME];
        private int _closestAttractorIndex = -1;
        private int _lastIndex = -1;
        public int LastIndex { get { return (_lastIndex); } }
        private AttractorInformation _closestAttractor;

        public void Init(Graviton graviton, GravitonZoneLocalizer gravitonZoneLocalizer, GravitonSettings settings, GravitonSettingsLocal settingsLocal)
        {
            _graviton = graviton;
            _gravitonZones = gravitonZoneLocalizer;
            _gravitonSettings = settings;
            _gravitonSettingsLocal = settingsLocal;

            _lastIndex = -1;
            _closestAttractorIndex = -1;
            _frequencyToSearchClosestPoints = new FrequencyCoolDown();
        }

        public void CalculateGravity()
        {
            if (_gravitonSettingsLocal.FrequencyToSearchClosestPoints == 0 || _frequencyToSearchClosestPoints.IsNotRunning())
            {
                CalculatePoints();
                _frequencyToSearchClosestPoints.StartCoolDown(_gravitonSettingsLocal.FrequencyToSearchClosestPoints);
            }
            CalculateForces();
            _currentGravityVector = RigidGraviton.ClampVelocity(_currentGravityVector, _gravitonSettingsLocal.MaxSpeed);
        }

        /// <summary>
        /// find all attractor closest points & calculate there 
        /// </summary>
        private void CalculatePoints()
        {
            _lastIndex = 0;
            _closestAttractorIndex = 0;
            for (int i = 0; i < _gravitonZones.ZonesWhereWeAreInside.Count; i++)
            {
                if (_gravitonZones.ZonesWhereWeAreInside[i]?.AttractorLogic == null
                    || !_gravitonZones.ZonesWhereWeAreInside[i].SettingsLocal.IsActiveZone)
                {
                    continue;
                }
                _closestAttractor = _gravitonZones.ZonesWhereWeAreInside[i].AttractorLogic.CalculatesPoints(
                    _graviton,
                    AttractorForces,
                    ref _lastIndex,
                    ref _closestAttractorIndex);
            }
        }

        public AttractorInformation GetClosestAttractorInformation()
        {
            return (_closestAttractor);
        }

        private void CalculateForces()
        {
            //reset force and leave if the graviton is not inside a zone
            if (_lastIndex == 0)
            {
                _currentGravityVector = Vector3.zero;
                return;
            }

            float reference_squared_distance = AttractorForces[_closestAttractorIndex].SquaredDist;
            //float K = _graviton.SettingsGlobal.RatioEaseOutAttractors;

            float refForceMagnitude = _graviton.SettingsLocal.Mass * AttractorForces[_closestAttractorIndex].Gravity;
            Vector3 sumForce = AttractorForces[_closestAttractorIndex].NormalizedDirection * refForceMagnitude;
            ForceAmount[_closestAttractorIndex] = refForceMagnitude;
            ForceJetPackAmount[_closestAttractorIndex] = AttractorForces[_closestAttractorIndex].ForceJetPack;

            float forceJetPackAmount = _graviton.SettingsLocal.Mass * ForceJetPackAmount[_closestAttractorIndex];
            Vector3 forceJetPack = AttractorForces[_closestAttractorIndex].NormalizedDirection * forceJetPackAmount;
            sumForce += forceJetPack;

            if (_graviton.SettingsLocal.DoAcceleration)
            {
                ForceAccelerationAmount[_closestAttractorIndex] = -forceJetPackAmount * _graviton.SettingsLocal.AccelerationForceOnLowGravity;
                Vector3 additionalForce = AttractorForces[_closestAttractorIndex].ForceAccelerationInsideJetPack * ForceAccelerationAmount[_closestAttractorIndex];
                sumForce += additionalForce;
            }

            for (int i = 0; i < _lastIndex; i++)
            {
                if (i == _closestAttractorIndex)
                {
                    continue;
                }
                
                float mass_GRAVITON = _graviton.SettingsLocal.Mass;
                float gravity_ATTRACTOR = AttractorForces[i].Gravity;
                float distance_squared_GRAVITON_ATTRACTOR = AttractorForces[i].SquaredDist;


                // F = (mass_GRAVITON * gravity_ATTRACTOR) / ((distance_GRAVITON_ATTRACTOR / distance_reference) * K)
                float forceMagnitude = (mass_GRAVITON * gravity_ATTRACTOR) / ((distance_squared_GRAVITON_ATTRACTOR / reference_squared_distance));
                ForceAmount[i] = forceMagnitude;
                ForceJetPackAmount[i] = AttractorForces[i].ForceJetPack;

                Vector3 forceAttraction = AttractorForces[i].NormalizedDirection * ForceAmount[i];
                forceJetPackAmount = _graviton.SettingsLocal.Mass * ForceJetPackAmount[i];
                forceJetPack = AttractorForces[i].NormalizedDirection * forceJetPackAmount;

                if (_graviton.SettingsLocal.DoAcceleration)
                {
                    ForceAccelerationAmount[i] = -forceJetPackAmount * _graviton.SettingsLocal.AccelerationForceOnLowGravity;
                    Vector3 additionalForce = AttractorForces[i].ForceAccelerationInsideJetPack * ForceAccelerationAmount[i];
                    sumForce += additionalForce;
                }

                sumForce += forceAttraction;
                sumForce += forceJetPack;
            }
            _currentGravityVector = sumForce;
        }

#if UNITY_EDITOR

        public void OnCustomDrawGizmos()
        {
            if (!_graviton.SettingsGlobal.ShowForce)
            {
                return;
            }

            for (int i = 0; i < _lastIndex; i++)
            {
                Debug.DrawLine(_graviton.Position, AttractorForces[i].PointOfAttraction, Color.blue);
                ExtDrawGuizmos.DrawArrow(_graviton.Position, AttractorForces[i].NormalizedDirection * ForceAmount[i], Color.white);
                ExtDrawGuizmos.DrawArrow(_graviton.Position, AttractorForces[i].NormalizedDirection * ForceJetPackAmount[i], Color.cyan);
                if (_graviton.SettingsLocal.DoAcceleration)
                {
                    ExtDrawGuizmos.DrawArrow(_graviton.Position, AttractorForces[i].ForceAccelerationInsideJetPack * ForceAccelerationAmount[i], Color.yellow);
                }
            }
            ExtDrawGuizmos.DrawArrow(_graviton.Position, _currentGravityVector, Color.green);
        }
#endif
    }
}