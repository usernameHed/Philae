using philae.gravity.physicsBody;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.graviton
{
    public class InitialPush : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _initialVelocity = new Vector3(0, 0, 0);
        [SerializeField]
        private RigidGraviton _rigidbody;

        public void Push()
        {
            _rigidbody.SetVelocity(_initialVelocity);
        }

        public void Push(Vector3 velocity)
        {
            _rigidbody.SetVelocity(velocity);
        }
    }
}