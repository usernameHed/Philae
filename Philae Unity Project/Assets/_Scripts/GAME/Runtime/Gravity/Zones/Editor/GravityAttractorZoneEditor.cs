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
                    : base(showExtension: false, tinyEditorName: "Gravity Attractor")
        {

        }

        public override void OnCustomEnable()
        {
            _zone = GetTarget<GravityAttractorZone>();
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

        public override void ShowTinyEditorContent()
        {
            serializedObject.Update();

            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                if (ExtGUIButtons.Button("x", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    _zone.GetScalerZoneReference.ResetTransform(true);
                }
                _zone.ShapeZone = ExtGUIEnums.EnumPopup(_zone.ShapeZone, null, out bool shapeHasChanged);
            }

            ShowSpecialVariables();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (ExtPrefabs.IsEditingInPrefabMode(_zone.gameObject))
            {
                return;
            }


            ExtHandle.DoMultiHandle(_zone.GetScalerZoneReference, out bool hasChanged);
            if (hasChanged)
            {
                ConstrainPosition.ApplyConstraint(_zone.SettingsGlobal.ConstrainPosition, _zone.GetScalerZoneReference);
            }


            if (ExtEventEditor.IsKeyDown(Event.current, KeyCode.F))
            {
                ExtSceneView.FocusOnSelection(_zone.GetScalerZoneReference.gameObject, 100);
            }
            LockZoneTransform();
        }

        private void LockZoneTransform()
        {
            Transform parent = GetProperty("_parent").GetValue<Transform>();

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

        private void ShowSpecialVariables()
        {
            switch (_zone.ShapeZone)
            {
                case ZoneSettingsLocal.Shape.SPHERE:
                    ShowSphere();
                    break;
                case ZoneSettingsLocal.Shape.CUBE:
                    ShowSquare();
                    break;
                case ZoneSettingsLocal.Shape.CYLINDER:
                    ShowCylinder();
                    break;
                case ZoneSettingsLocal.Shape.CAPSULE:
                    ShowCapsule();
                    break;
            }
        }

        private void ShowSphere()
        {
            ZoneSphere sphere = (ZoneSphere)_zone.CurrentZone;
            //sphere.Sphere.Radius = ExtGUIFloatFields.FloatField(sphere.Sphere.Radius, _target, out bool valueHasChanged, "Radius", "radius of the sphere", 0);
        }

        private void ShowSquare()
        {
            ZoneCube zoneCube = (ZoneCube)_zone.CurrentZone;
            //zoneCube.Cube.Size = ExtGUIFloatFields.FloatField(zoneCube.Cube.Size, _target, out bool valueHasChanged, "Size", "size", 0);
        }

        private void ShowCylinder()
        {
            ZoneCylinder zoneCylinder = (ZoneCylinder)_zone.CurrentZone;
            CylinderSettings(zoneCylinder.Cylindre);
        }

        private void ShowCapsule()
        {
            ZoneCapsule zoneCapsule = (ZoneCapsule)_zone.CurrentZone;
            float lenght = ExtGUIFloatFields.FloatField(zoneCapsule.Capsule.Lenght, null, out bool lenghtChanged, "Length", "Length");
            if (lenghtChanged)
            {
                zoneCapsule.Capsule.ChangeLenght(lenght);
            }
            zoneCapsule.Capsule.SphereTop = ExtGUIToggles.Toggle(zoneCapsule.Capsule.SphereTop, null, "active top sphere", out bool topChanged);
            zoneCapsule.Capsule.SphereBottom = ExtGUIToggles.Toggle(zoneCapsule.Capsule.SphereBottom, null, "active bottom sphere", out bool bottomChanged);
        }

        private void CylinderSettings(ExtCylindre cylindre)
        {
            float lenght = ExtGUIFloatFields.FloatField(cylindre.Lenght, null, out bool lenghtChanged, "Length", "Length");
            if (lenghtChanged)
            {
                cylindre.ChangeLenght(lenght);
            }
        }
    }
}