using extUnityComponents;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.line;
using philae.gravity.attractor.logic;
using philae.gravity.zones;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor
{
    [CustomEditor(typeof(Attractor), true)]
    public class AttractorEditor : DecoratorComponentsEditor
    {
        protected Attractor _attractor;

        private Transform _parent;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {


        }

        public AttractorEditor()
            : base(false, "Attractor")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            _attractor = GetTarget<Attractor>();
            _parent = _attractor.transform.parent;
        }

        public override void ShowTinyEditorContent()
        {
            if (GUILayout.Button("Fill Auto Zones"))
            {
                _attractor.AutomaticlySetupZone();
                EditorUtility.SetDirty(_attractor);
            }
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (ExtPrefabs.IsEditingInPrefabMode(_attractor.gameObject))
            {
                return;
            }
            LockAttractorTransform();
        }
        
        private void LockAttractorTransform()
        {
            if (_attractor.transform == _parent)
            {
                return;
            }

            //here we are inside another attractor.. go back from the last parent !
            Attractor otherAttractor = _attractor.transform.GetExtComponentInParents<Attractor>(99, false);
            if (otherAttractor != null)
            {
                _attractor.transform.SetParent(null);
                Debug.LogError("Can't have Attractor inside another Attractor");
                return;
            }
        }
        
    }
}