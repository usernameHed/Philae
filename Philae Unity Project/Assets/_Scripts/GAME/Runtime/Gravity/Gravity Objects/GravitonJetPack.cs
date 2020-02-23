using hedCommon.extension.runtime;
using philae.data.gravity;
using philae.gravity.physicsBody;
using philae.gravity.player;
using philae.sound;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.graviton
{
    public class GravitonJetPack : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private Vector3 _forceJetPack;
        [SerializeField]
        private RigidGraviton _rigidbody;

        private bool _isInJetPack = false;
        private Graviton _graviton;
        private GravitonGravityCalculation _gravitonGravityCalculation;
        private GravitonSettingsLocal _gravitonSettingsLocal;

        public void Init(Graviton graviton, GravitonSettingsLocal gravitonSettingsLocal, GravitonGravityCalculation gravitonGravityCalculation)
        {
            _graviton = graviton;
            _isInJetPack = false;
            _gravitonSettingsLocal = gravitonSettingsLocal;
            _gravitonGravityCalculation = gravitonGravityCalculation;
        }

        public bool CanApplyJetPack(float squaredDist)
        {
            bool canApplyJetPack = squaredDist < _gravitonSettingsLocal.RadiusJetPack * _gravitonSettingsLocal.RadiusJetPack;
            
            return (canApplyJetPack);
        }

        public void SetJetPackState(bool isInJetPack)
        {
            if (SoundManager.Instance)
            {
                if (isInJetPack && !_isInJetPack)
                {
                    _isInJetPack = true;
                }
                else if (!isInJetPack && _isInJetPack)
                {
                    _isInJetPack = false;
                }
            }
        }

        public float ForceJetPack(float squaredDist)
        {
            float squaredRadius = _gravitonSettingsLocal.RadiusJetPack * _gravitonSettingsLocal.RadiusJetPack;
            float currentDistDifference = squaredDist - squaredRadius;

            float force = _gravitonSettingsLocal.JetPackForce * currentDistDifference;
            force *= _gravitonSettingsLocal.Mass;
            return (force);
        }

        public Vector3 ForceAcceleration(Vector3 currentVelocity, Vector3 currentJetPack)
        {
            float dotProduct = ExtVector3.DotProduct(currentVelocity, currentJetPack);

            if (dotProduct < -0.1f && dotProduct > -1f)
            {
                Vector3 middle = ExtVector3.FastNormalized(currentJetPack + currentVelocity);
                return (middle);
            }
            return (Vector3.zero);
        }

        private float GetRadiusFromSize()
        {
            return (_gravitonSettingsLocal.RadiusJetPack * _graviton.LocalScale.Maximum());
        }

        private void OnDrawGizmos()
        {
            ExtDrawGuizmos.DebugWireSphere(_graviton.Position, Color.cyan, GetRadiusFromSize());
        }

        private void OnDestroy()
        {

        }
    }
}