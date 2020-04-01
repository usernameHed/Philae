using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extUnityComponents.transform
{
    [ExecuteInEditMode]
    public class TransformHiddedTools : MonoBehaviour, IEditorOnly
    {
        public bool ShowName = false;
        public Color ColorText = Color.white;
        public bool HideHandle = false;

        private void Awake()
        {
            this.hideFlags = HideFlags.HideInInspector;
        }

        public Component GetReference()
        {
            return (this);
        }
    }
}