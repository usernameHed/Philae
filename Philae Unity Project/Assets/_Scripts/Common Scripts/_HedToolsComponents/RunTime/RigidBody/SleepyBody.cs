using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extUnityComponents
{
    public class SleepyBody : MonoBehaviour
    {
        [SerializeField]
        private float _fallAsleepAfterSomeSeconds = 1f;
        public float FallAsleepTime { get { return (_fallAsleepAfterSomeSeconds); } set { _fallAsleepAfterSomeSeconds = value; } }

        private Rigidbody _rigidbody;
        private FrequencyCoolDown _frequencyCoolDown = new FrequencyCoolDown();

        public void Awake()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
            _frequencyCoolDown.StartCoolDown(_fallAsleepAfterSomeSeconds);
        }

        private void FixedUpdate()
        {
            if (_frequencyCoolDown.IsFinished())
            {
                _rigidbody.Sleep();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            _rigidbody.WakeUp();
            _frequencyCoolDown.StartCoolDown(_fallAsleepAfterSomeSeconds);
        }
    }
}