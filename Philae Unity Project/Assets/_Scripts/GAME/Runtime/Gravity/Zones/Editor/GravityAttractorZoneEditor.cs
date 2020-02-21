using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExtUnityComponents;
using hedCommon.extension.editor;
using hedCommon.geometry.shape3d;
using static UnityEditor.EditorGUILayout;
using hedCommon.extension.runtime;
using hedCommon.editorGlobal;
using philae.gravity.graviton;
using philae.gravity.attractor;
using philae.data.gravity;
using static philae.gravity.physicsBody.PhysicBody;

namespace philae.gravity.zones
{
    [CustomEditor(typeof(GravityAttractorZone))]
    public class GravityAttractorZoneEditor : DecoratorComponentsEditor
    {
        private GravityAttractorZone _zone;

        public GravityAttractorZoneEditor()
                    : base()
        {

        }

        public override void OnCustomEnable()
        {
            _zone = GetTarget<GravityAttractorZone>();

            if (ExtPrefabs.IsEditingInPrefabMode(_zone.gameObject))
            {
                return;
            }

            if (!ZonesLister.Instance.Contain(_zone))
            {
                ZonesLister.Instance.Init();
            }
            _zone.GetScalerZoneReference.name = _zone.name;
            SetupParentZone();
            ExtSceneView.Repaint();
        }

        private void SetupParentZone()
        {
            this.UpdateEditor();
            Transform parent = this.GetValue<Transform>("_parent");
            if (parent == null)
            {
                this.SetValue<Transform>("_parent", _zone.transform.parent);
                this.ApplyModification();
            }
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (ExtPrefabs.IsEditingInPrefabMode(_zone.gameObject))
            {
                return;
            }

            LockZoneTransform();
        }

        private void LockZoneTransform()
        {
            Transform parent = this.GetPropertie("_parent").GetValue<Transform>();

            if (_zone.transform == parent)
            {
                return;
            }

            GravityAttractorZone otherZone = _zone.transform.GetExtComponentInParents<GravityAttractorZone>(99, false);
            if (otherZone != null)
            {
                _zone.transform.SetParent(parent);
                Debug.LogError("Can't have Zone inside another Zone");
                return;
            }
        }

        //end of class
    }
    //end of nameSpace
}