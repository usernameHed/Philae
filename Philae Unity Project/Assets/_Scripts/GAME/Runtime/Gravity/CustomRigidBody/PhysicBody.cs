using hedCommon.extension.runtime;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace philae.gravity.physicsBody
{
    [ExecuteInEditMode]
    public class PhysicBody : MonoBehaviour
    {
        [Serializable]
        public struct ConstrainPosition
        {
            public bool ConstrainX;
            public bool ConstrainY;
            public bool ConstrainZ;

            public static void ApplyConstraint(ConstrainPosition constrain, Transform transform)
            {
                Vector3 position = transform.position;
                if (constrain.ConstrainX)
                {
                    position.x = 0;
                }
                if (constrain.ConstrainY)
                {
                    position.y = 0;
                }
                if (constrain.ConstrainZ)
                {
                    position.z = 0;
                }
                transform.position = position;
            }
        }

        [SerializeField, OnValueChanged("OnMassChanged")]
        private float _mass = 1f;

        [SerializeField, OnValueChanged("OnKinematicChanged")]
        private bool _isKinematic = false;

        public ConstrainPosition ConstrainPositions;


        [SerializeField, ReadOnly]
        private Vector3 _velocityVector;
        [SerializeField, ReadOnly]
        private Vector3 _lastVelocityVector;

        [SerializeField]
        private Rigidbody _rigidbody = default;
        [SerializeField]
        private InitialPush _initialPush = default;

        public Vector3 Position
        { 
            get
            {
                if (transform == null)
                {
                    return (Vector3.zero);
                }
                return (transform.position);
            }
            set
            { 
                if (Application.isPlaying)
                {
                    _rigidbody.position = value;
                }
                else
                {
                    transform.position = value;
                }
            }
        }

        public float Mass
        {
            get
            {
                return (_mass);
            }
            set
            {
                if (_rigidbody == null)
                {
                    return;
                }
                _mass = value;
                _mass = ExtPhysics.ClampMass(_mass);
                _rigidbody.mass = _mass;
            }
        }

        public bool IsKinematic
        {
            get { return (_isKinematic); }
            set
            {
                _isKinematic = value;
                _rigidbody.isKinematic = _isKinematic;
            }
        }

        private void Awake()
        {
            SetupRigidBody();
        }

        private void SetupRigidBody()
        {
            if (_rigidbody == null)
            {
                _rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = IsKinematic;
            }
#if UNITY_EDITOR
            if (ExtPrefabs.IsEditingInPrefabMode(gameObject))
            {
                _rigidbody.hideFlags = HideFlags.None;
            }
            else
            {
                _rigidbody.hideFlags = HideFlags.NotEditable;
            }
#endif
        }

        public void Init()
        {
            SetupRigidBody();
            _velocityVector = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
            _lastVelocityVector = Vector3.zero;
            if (_initialPush)
            {
                _initialPush.Push();
            }
        }

        public void CalculateAutoMass()
        {
            //mass = volume x density
            //Mass = transform.localScale.Maximum() * _density;
        }

        private void OnMassChanged()
        {
            Mass = ExtPhysics.ClampMass(_mass);
        }

        private void OnKinematicChanged()
        {
            IsKinematic = _isKinematic;
        }

        public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            switch (forceMode)
            {
                case ForceMode.Force:
                    AddForceMassDependent(force);
                    break;
                case ForceMode.Acceleration:
                    AddForceMassInDependent(force);
                    break;
                case ForceMode.Impulse:
                    AddImpulsionMassDependant(force);
                    break;
                case ForceMode.VelocityChange:
                    AddImpulsionMassInDependant(force);
                    break;
            }
        }

        public void SetVelocity(Vector3 velocity)
        {
            if (_rigidbody == null)
            {
                return;
            }
            if (Application.isPlaying)
            {
                _rigidbody.velocity = velocity;
            }
            else
            {
                _velocityVector = velocity;
            }
        }

        public Vector3 GetVelocity()
        {
            if (_rigidbody == null)
            {
                return (Vector3.zero);
            }

            if (Application.isPlaying)
            {
                return (_rigidbody.velocity);
            }
            else
            {
                return (_velocityVector);
            }
        }

        public void SlowDown(float amountSlowDownOverTime = 0.95f)
        {
            Vector3 currentVelocity = GetVelocity();
            currentVelocity *= amountSlowDownOverTime;
            SetVelocity(currentVelocity);
        }

        public Vector3 GetPreviousVelocity()
        {
            return (_lastVelocityVector);
        }

        public float GetMagnitude()
        {
            return (GetVelocity().magnitude);
        }

        public static Vector3 ClampVelocity(Vector3 currentVelocity, float maxSpeed)
        {
            if (currentVelocity.magnitude > maxSpeed)
            {
                currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
            }
            return (currentVelocity);
        }

        public Vector3 GetVelocityNormalized()
        {
            return (ExtVector3.FastNormalized(GetVelocity()));
        }

        /// <summary>
        /// Add a continuous force to the rigidbody, using its mass.
        /// it use Time.fixedDeltaTime
        /// </summary>
        /// <param name="force">direction & strenght of force to add</param>
        public void AddForceMassDependent(Vector3 force)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !EditorOptions.Instance.SimulatePhysics)
            {
                return;
            }
#endif
            if (_rigidbody == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                _rigidbody.AddForce(force, ForceMode.Force);
            }
            else
            {
                Vector3 accelerationVector = force / Mass;
                _velocityVector += accelerationVector * Time.fixedDeltaTime;
            }
        }

        /// <summary>
        /// Add a continuous acceleration to the rigidbody, ignoring its mass.
        /// it use Time.fixedDeltaTime
        /// </summary>
        /// <param name="force"></param>
        public void AddForceMassInDependent(Vector3 force)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !EditorOptions.Instance.SimulatePhysics)
            {
                return;
            }
#endif
            if (Application.isPlaying)
            {
                _rigidbody.AddForce(force, ForceMode.Acceleration);
            }
            else
            {
                _velocityVector += force * Time.fixedDeltaTime;
            }
        }

        /// <summary>
        /// Add an instant force impulse to the rigidbody, using its mass.
        /// </summary>
        /// <param name="force"></param>
        public void AddImpulsionMassDependant(Vector3 force)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !EditorOptions.Instance.SimulatePhysics)
            {
                return;
            }
#endif

            if (Application.isPlaying)
            {
                _rigidbody.AddForce(force, ForceMode.Impulse);
            }
            else
            {
                Vector3 accelerationVector = force / Mass;
                _velocityVector += accelerationVector;
            }
            
        }


        /// <summary>
        /// Add an instant velocity change to the rigidbody, ignoring its mass.
        /// </summary>
        /// <param name="force"></param>
        public void AddImpulsionMassInDependant(Vector3 force)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !EditorOptions.Instance.SimulatePhysics)
            {
                return;
            }
#endif

            if (Application.isPlaying)
            {
                _rigidbody.AddForce(force, ForceMode.VelocityChange);
            }
            else
            {
                _velocityVector += force;
            }
            
        }

        public void UpdatePosition()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !EditorOptions.Instance.SimulatePhysics)
            {
                return;
            }
#endif
            if (Application.isPlaying)
            {
                //here unity physics take care of it
            }
            else
            {
                if (_isKinematic)
                {
                    return;
                }
#if UNITY_EDITOR
                Undo.RecordObject(transform, "cusotm physics move");
#endif
                transform.position += _velocityVector * Time.fixedDeltaTime;
                ConstrainPosition.ApplyConstraint(ConstrainPositions, transform);
            }
        }

        public void SaveCurrentSpeed()
        {
            if (_rigidbody == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                _lastVelocityVector = _rigidbody.velocity;
            }
            else
            {
                _lastVelocityVector = _velocityVector;
            }
        }
    }
}