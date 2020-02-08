using ExtUnityComponents;
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
            SetupParentZone();
        }

        private void SetupParentZone()
        {
            this.UpdateEditor();
            Transform parent = this.GetValue<Transform>("_parent");
            if (parent == null)
            {
                this.SetValue<Transform>("_parent", _attractor.transform.parent);
                this.ApplyModification();
            }
        }

        /// <summary>
        /// this function is called on the first OnSceneGUI()
        /// usefull to initialize scene GUI
        /// </summary>
        /// <param name='sceneview'>current drawing scene view</param>
        protected override void InitOnFirstOnSceneGUI(SceneView sceneview)
        {
            //initialise scene GUI
        }

        public override void ShowTinyEditorContent()
        {
            GUILayout.Button("Play");
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (ExtPrefabs.IsEditingInPrefabMode(_attractor.gameObject))
            {
                return;
            }
            SetStaticIfNotMovable();
            LockAttractorTransform();
        }

        /// <summary>
        /// set/unset static for this attractor (depending if the object is movable or not...)
        /// </summary>
        private void SetStaticIfNotMovable()
        {
            if (!_attractor.SettingsGlobal.IsMovable && !_attractor.gameObject.isStatic)
            {
                _attractor.gameObject.SetStaticRecursively(true);
            }
            else if (_attractor.SettingsGlobal.IsMovable && _attractor.gameObject.isStatic)
            {
                _attractor.gameObject.SetStaticRecursively(false);
            }
        }

        private void LockAttractorTransform()
        {
            this.UpdateEditor();
            Transform parent = GetProperty("_parent").GetValue<Transform>();

            if (_attractor.transform == parent)
            {
                return;
            }


            //here we don't find any AttractorLister, that's mean we are outside, go back inside from the last parent !
            AttractorListerLogic lister = _attractor.transform.GetExtComponentInParents<AttractorListerLogic>(99, false);
            if (lister == null)
            {
                _attractor.transform.SetParent(parent);
                this.ApplyModification();
                Debug.LogError("attractor can't leave outside a parent zone");
                return;
            }

            //here the lister found is not the same as the lister saved, do a transfert !
            AttractorListerLogic current = this.GetValue<AttractorListerLogic>("_attractorListerLogic");
            if (lister != current)
            {
                AttractorListerLogic currentLister = this.GetValue<AttractorListerLogic>("_attractorListerLogic");
                currentLister.Lister.RemoveAttractor(_attractor);
                _attractor.InitOnCreation(lister);
                return;
            }

            //here we are inside another attractor.. go back from the last parent !
            Attractor otherAttractor = _attractor.transform.GetExtComponentInParents<Attractor>(99, false);
            if (otherAttractor != null)
            {
                _attractor.transform.SetParent(parent);
                Debug.LogError("Can't have Attractor inside another Attractor");
                return;
            }
        }
    }
}