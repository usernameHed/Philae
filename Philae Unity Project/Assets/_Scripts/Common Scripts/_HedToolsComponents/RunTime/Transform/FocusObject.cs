using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtUnityComponents.transform
{
    public class FocusObject : MonoBehaviour, IEditorOnly, IHaveDecoratorOnIt
    {
        public Component GetReference()
        {
            return (this);
        }

        #region IHaveDecoratorOnIt
        [FoldoutGroup("IHaveDecoratorOnIt"), SerializeField]
        private bool _showExtensions = true;
        [FoldoutGroup("IHaveDecoratorOnIt"), SerializeField]
        private bool _showTinyEditorWindows = true;
        public Component GetReferenceDecoratorsComponent() => this;

        public bool ShowExtension { get => _showExtensions; set => _showExtensions = value; }
        public bool ShowTinyEditorWindow { get => _showTinyEditorWindows; set => _showTinyEditorWindows = value; }
        #endregion


        public bool EnableVisual = true;

        public string MessageInfo = "Focus Object";
        public GameObject ToFocus;
    }
}