using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extUnityComponents
{
    public interface IHaveDecoratorOnIt
    {
        /*
        #region IHaveDecoratorOnIt
        [FoldoutGroup("IHaveDecoratorOnIt"), SerializeField]
        private bool _showExtensions = true;
        [FoldoutGroup("IHaveDecoratorOnIt"), SerializeField]
        private bool _showTinyEditorWindows = true;
        public Component GetReferenceDecoratorsComponent() => this;

        public bool ShowExtension { get => _showExtensions; set => _showExtensions = value; }
        public bool ShowTinyEditorWindow { get => _showTinyEditorWindows; set => _showTinyEditorWindows = value; }
        #endregion
        */

        /// <summary>
        /// can the decorator show the extension
        /// </summary>
        bool ShowExtension
        {
            get;
            set;
        }

        /// <summary>
        /// can the decorator show the TinyEditorWindow
        /// </summary>
        bool ShowTinyEditorWindow
        {
            get;
            set;
        }

        Component GetReferenceDecoratorsComponent();
    }
}