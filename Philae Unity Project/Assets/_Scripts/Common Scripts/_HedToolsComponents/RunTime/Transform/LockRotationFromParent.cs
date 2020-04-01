using extUnityComponents;
using hedCommon.extension.runtime;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

namespace extUnityComponents.transform
{
    [ExecuteInEditMode]
    public class LockRotationFromParent : MonoBehaviour, IEditorOnly
    {
        public Component GetReference()
        {
            return (this);
        }

        [FoldoutGroup("GamePlay"), Tooltip("")]
        public Transform ToLock;
        [FoldoutGroup("GamePlay"), Tooltip("")]
        public bool RotateWithTheParent = true;
        [FoldoutGroup("GamePlay"), Tooltip("")]
        public bool OverrideRotationUpGlobal = false;

        private Quaternion _saveRotation;

        private void Awake()
        {
#if UNITY_EDITOR
            if (ToLock == null && transform.childCount > 0)
            {
                ToLock = transform.GetChild(0);
            }
#endif

            _saveRotation = ToLock.rotation;
        }

#if UNITY_EDITOR

        public void ResetLocalRotation()
        {
            ToLock.localRotation = Quaternion.identity;
            _saveRotation = ToLock.rotation;
        }


        public void RotateUp()
        {
            ToLock.rotation = ExtRotation.TurretLookRotation(_saveRotation * Vector3.forward, Vector3.up);
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (OverrideRotationUpGlobal)
            {
                if (!RotateWithTheParent)
                {
                    ToLock.rotation = ExtRotation.TurretLookRotation(_saveRotation * Vector3.forward, Vector3.up);
                }
                else
                {
                    ToLock.rotation = ExtRotation.TurretLookRotation(ToLock.forward, Vector3.up);
                }
                _saveRotation = ToLock.rotation;
            }

            if (RotateWithTheParent)
            {
                _saveRotation = ToLock.rotation;
                return;
            }


            if (Selection.activeGameObject == ToLock.gameObject)
            {
                _saveRotation = ToLock.rotation;
                return;
            }
            ToLock.rotation = _saveRotation;
        }


#endif
    }
}