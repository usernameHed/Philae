using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ExtUnityComponents.transform
{
    /// <summary>
    /// this script allow you to lock a child from the parent movements.
    /// ExecuteInEditMode allow us to execute it even in play mode
    /// </summary>
    [ExecuteInEditMode]
    public class AlwaysLockPositionFromParents : MonoBehaviour
    {
        [SerializeField] private bool _workInPlayMode = true;            //define if this script work in play mode
        [SerializeField] private bool _lockPosition = true;              //lock position from the parent
        [SerializeField] private bool _lockPositionWhenScaling = true;   //if the scale of the parent change, do we lock the child position anyway ? (work only if lockPosition is true)
        [SerializeField] private bool _lockRotation = true;              //lock rotation from the parent
        [SerializeField] private bool _lockToSpecificPosition = false;
        [SerializeField] private Vector3 _specificPosition = Vector3.zero;

        //cache child variables
        private Vector3 _oldPositionChild;
        private Quaternion _oldRotationChild;

        //cache parent variable
        private Vector3 _oldPositionParent;
        private Quaternion _oldRotationParent;
        private Vector3 _lossyScaleParent;

        //the parent ref, if no parent: no lock
        private Transform _parentRef;

        /// <summary>
        /// save on awake the current state of the gameObject and his parent
        /// </summary>
        private void Awake()
        {
            SavePositionAndRotationChild();
            _parentRef = transform.parent;
            SavePositionAndRotationParent();
        }

        /// <summary>
        /// here apply the lock position & rotation if the parent has change
        /// </summary>
        private void ApplyLockIfParentChange()
        {
            if (_parentRef == null)
            {
                _parentRef = transform.parent;
                return;
            }
            //Debug.Log("position parent: " + _oldRotationParent);
            if (_parentRef.position != _oldPositionParent
                || _parentRef.rotation != _oldRotationParent
                || (_parentRef.lossyScale != _lossyScaleParent && _lockPositionWhenScaling))
            {
                ApplyLock();
            }
        }

        /// <summary>
        /// here Apply the lock depending on out settings
        /// </summary>
        private void ApplyLock()
        {
            if (_lockPosition)
            {
                transform.position = _oldPositionChild;
            }
            else
            {
                _oldPositionChild = transform.position;
            }

            if (_lockRotation)
            {
                transform.rotation = _oldRotationChild;
            }
            else
            {
                _oldRotationChild = transform.rotation;
            }
        }

        /// <summary>
        /// save the child position and rotation for later
        /// </summary>
        private void SavePositionAndRotationChild()
        {
            _oldPositionChild = transform.position;
            _oldRotationChild = transform.rotation;
        }

        /// <summary>
        /// save the parent position and rotation for later
        /// </summary>
        private void SavePositionAndRotationParent()
        {
            if (_parentRef == null)
            {
                return;
            }
            _oldPositionParent = _parentRef.position;
            _oldRotationParent = _parentRef.rotation;
            _lossyScaleParent = _parentRef.lossyScale;
        }


        /// <summary>
        /// called in play & in editor
        /// </summary>
        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (_workInPlayMode)
                {
                    ApplyLockIfParentChange();
                }
            }
            else
            {
                ApplyLockIfParentChange();
            }
#else
        if (_workInPlayMode)
        {
            ApplyLockIfParentChange();
        }
#endif
            

            SavePositionAndRotationChild();
            SavePositionAndRotationParent();

            LockAtLocalScaleZero();
        }

        private void LockAtLocalScaleZero()
        {
            if (_lockToSpecificPosition)
            {
                transform.position = _specificPosition;
            }
        }
    }
}