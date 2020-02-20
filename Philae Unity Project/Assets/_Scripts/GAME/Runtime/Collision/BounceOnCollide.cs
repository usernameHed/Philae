using hedCommon.extension.runtime;
using hedCommon.time;
using philae.gravity.physicsBody;
using philae.sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.collision
{

    public class BounceOnCollide : MonoBehaviour
    {
        [Serializable]
        public class CollisionGraviton
        {
            public RigidGraviton Graviton = null;
            public FrequencyChrono FrequencyChrono = new FrequencyChrono();

            public CollisionGraviton(RigidGraviton graviton)
            {
                Graviton = graviton;
                FrequencyChrono.StartChrono();
            }
        }

        [SerializeField]
        private float _timeBeforeBounceAgain = 0.3f;
        [SerializeField]
        private float _bounceMultiplier = 0.9f;
        [SerializeField]
        private List<CollisionGraviton> _bounceOnce = new List<CollisionGraviton>(50);
        [SerializeField]
        private FmodEventEmitter _collide;
        [SerializeField]
        private GardienZone Wall;

        private void OnEnable()
        {
            _bounceOnce.Clear();
        }

        private void OnCollisionEnter(Collision collision)
        {
            RigidGraviton rigidGraviton = collision.gameObject.GetComponent<RigidGraviton>();
            if (rigidGraviton == null || !CanCollide(rigidGraviton))
            {
                return;
            }
            Debug.Log("Collide with: " + rigidGraviton);
            Wall.LanchDisolve();
            SoundManager.Instance.PlaySound(_collide);

            _bounceOnce.Add(new CollisionGraviton(rigidGraviton));
            Vector3 currentVelocity = rigidGraviton.GetPreviousVelocity();
            Vector3 normal = -ExtVector3.GetMiddleOfXContactNormal(collision.contacts);
            Vector3 newDirection = Vector3.Reflect(currentVelocity, normal) * _bounceMultiplier;
            rigidGraviton.SetVelocity(newDirection);
            Debug.DrawRay(rigidGraviton.Position, newDirection, Color.black, 2f);
        }

        public bool CanCollide(RigidGraviton graviton)
        {
            for (int i = 0; i < _bounceOnce.Count; i++)
            {
                if (_bounceOnce[i].Graviton == graviton)
                {
                    if (_bounceOnce[i].FrequencyChrono.GetTimer() > _timeBeforeBounceAgain)
                    {
                        _bounceOnce.RemoveAt(i);
                        return (true);
                    }
                    return (false);
                }
            }
            return (true);
        }
    }
}