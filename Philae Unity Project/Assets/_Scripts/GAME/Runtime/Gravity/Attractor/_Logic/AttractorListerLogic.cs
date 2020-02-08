using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.data.gravity;
using philae.gravity.graviton;
using philae.gravity.zones;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static philae.gravity.graviton.GravitonGravityCalculation;

namespace philae.gravity.attractor.logic
{
    public class AttractorListerLogic : MonoBehaviour
    {
        [FoldoutGroup("Settings"), SerializeField, InlineEditor]
        private AttractorListerSettings _settingsGlobal;
        [FoldoutGroup("Settings"), SerializeField]
        private AttractorListerSettingsLocal _settingsLocal = new AttractorListerSettingsLocal();

        [SerializeField]
        private AttractorLister _attractorLister;
        public AttractorLister Lister { get { return (_attractorLister); } }
        [SerializeField]
        private AttractorsUpdater _attractorsUpdater;

        [SerializeField, ReadOnly]
        private GravityAttractorZone _zone = default;
        public GravityAttractorZone Zone { get { return (_zone); } }

        private AttractorInformation _cachedForceInformation;

        public void Init(GravityAttractorZone zone)
        {
            _zone = zone;
            _attractorLister.Init(_zone);
            _attractorsUpdater.Init(_settingsGlobal);
        }

        /// <summary>
        /// loop thought all attractor, and calculate 
        /// </summary>
        /// <param name="graviton">reference of the object we want to attract</param>
        /// <param name="ForceToFill">array cached to fill at the right given index</param>
        /// <param name="currentIndex">index where to fill the information in the array</param>
        /// <param name="_closestAttractorIndex">if this new attractor is closest than last, save the index for later !</param>
        public AttractorInformation CalculatesPoints(
            Graviton graviton,
            AttractorInformation[] ForceToFill,
            ref int currentIndex,
            ref int _closestAttractorIndex)
        {
            bool jetPackApplied = false;

            foreach (Attractor attractor in _attractorLister)
            {
                Vector3 closestPointToAttractor = attractor.GetClosestPoint(graviton, out bool canApplyGravity);
                if (!canApplyGravity)
                {
                    continue;
                }
                //save information into the slot [currentIndex]
                _cachedForceInformation.PointOfAttraction = closestPointToAttractor;
                _cachedForceInformation.Gravity = attractor.SettingsLocal.Gravity;
                _cachedForceInformation.SquaredDist = ExtVector3.DistanceSquared(closestPointToAttractor, graviton.Position);
                _cachedForceInformation.NormalizedDirection = ExtVector3.FastNormalized(closestPointToAttractor - graviton.Position);
                _cachedForceInformation.CanApplyJetPack = graviton.GravitonJetPack.CanApplyJetPack(_cachedForceInformation.SquaredDist);
                if (_cachedForceInformation.CanApplyJetPack)
                {
                    jetPackApplied = true;
                    _cachedForceInformation.ForceJetPack = graviton.GravitonJetPack.ForceJetPack(_cachedForceInformation.SquaredDist);
                    _cachedForceInformation.ForceAccelerationInsideJetPack = graviton.GravitonJetPack.ForceAcceleration(graviton.GetAccelerationNormalized, _cachedForceInformation.NormalizedDirection);
                }
                else
                {
                    _cachedForceInformation.ForceJetPack = 0;
                    _cachedForceInformation.ForceAccelerationInsideJetPack = Vector3.zero;
                }
                
                ForceToFill[currentIndex] = _cachedForceInformation;

                //save the new index if this one is the closest 
                if (ForceToFill[_closestAttractorIndex].SquaredDist > _cachedForceInformation.SquaredDist)
                {
                    _closestAttractorIndex = currentIndex;
                }
                currentIndex++;
            }
            graviton.GravitonJetPack.SetJetPackState(jetPackApplied);
            return (_cachedForceInformation);
        }
    }
}