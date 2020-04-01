using extUnityComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public class SelectOtherOnEnable : MonoBehaviour, IEditorOnly
    {
        [SerializeField]
        private bool _isActive = false;
        [SerializeField]
        private Transform _otherTarget;

        public Component GetReference()
        {
            return (this);
        }
    }
}