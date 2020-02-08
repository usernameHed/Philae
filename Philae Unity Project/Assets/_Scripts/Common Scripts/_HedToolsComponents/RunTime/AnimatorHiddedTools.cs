using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtUnityComponents
{
    [ExecuteInEditMode]
    public class AnimatorHiddedTools : MonoBehaviour, IEditorOnly
    {
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