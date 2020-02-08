using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtUnityComponents
{
    [ExecuteInEditMode]
    public class RigidBodySpecialSettings : MonoBehaviour
    {
        public const float DEFAULT_SLEEP_THRESHOLD = 0.005f;
        public const int DEFAULT_SOLVER_ITERATION = 6;
        public const float DEFAULT_MAX_DEPENETRATION_VELOCITY = 9999f;

        [SerializeField]
        private float _sleepThreshold = DEFAULT_SLEEP_THRESHOLD;
        [SerializeField]
        private int _solverIteration = DEFAULT_SOLVER_ITERATION;
        [SerializeField]
        private float _maxDepenetrationVelocity = DEFAULT_MAX_DEPENETRATION_VELOCITY;

        [SerializeField]
        private bool _centerOfMassChanged = false;

        [SerializeField]
        private Vector3 _defaultCenterOfMass = Vector3.zero;

        private Rigidbody _rigidbody;

        public void SetCenterOfMass(Vector3 newCenterOfMass)
        {
            _centerOfMassChanged = true;
            _defaultCenterOfMass = newCenterOfMass;
        }

        public void SetSleepThreshold(float sleed)
        {
            _sleepThreshold = sleed;
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
            _rigidbody.sleepThreshold = _sleepThreshold;
        }

        public void SetSolverIteration(int solverIteration)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            _solverIteration = solverIteration;
            _rigidbody.solverIterations = _solverIteration;
        }

        public void SetMaxDepenetrationVelocity(float maxDepenetrationVelocity)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            _maxDepenetrationVelocity = maxDepenetrationVelocity;
            _rigidbody.maxDepenetrationVelocity = _maxDepenetrationVelocity;
        }

        public void Awake()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
                if (_rigidbody == null)
                {
                    Destroy(this);
                    return;
                }
            }
            if (_centerOfMassChanged)
            {
                _rigidbody.centerOfMass = _defaultCenterOfMass;
            }

            if (_rigidbody.sleepThreshold != _sleepThreshold)
            {
                _rigidbody.sleepThreshold = _sleepThreshold;
            }
            if (_rigidbody.solverIterations != _solverIteration)
            {
                _rigidbody.solverIterations = _solverIteration;
            }
            if (_rigidbody.maxDepenetrationVelocity != _maxDepenetrationVelocity)
            {
                _rigidbody.maxDepenetrationVelocity = _maxDepenetrationVelocity;
            }
        }

        public void ResetCenterOfMass()
        {
            _centerOfMassChanged = false;
            _rigidbody.ResetCenterOfMass();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }
            if (_rigidbody == null)
            {
                DestroyImmediate(this);
                return;
            }
        }
#endif
    }
}