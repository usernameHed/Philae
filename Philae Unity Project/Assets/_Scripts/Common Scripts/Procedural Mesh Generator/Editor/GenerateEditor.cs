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
using static UnityEditor.EditorGUILayout;

namespace hedCommon.procedural
{
    [CustomEditor(typeof(Generate), true)]
    public class GenerateEditor : DecoratorComponentsEditor
    {
        protected Generate _generate;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public GenerateEditor()
            : base(true)
        {


        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            _generate = GetTarget<Generate>();
        }

        protected override void OnCustomInspectorGUI()
        {
            this.UpdateEditor();
            using (HorizontalScope scope = new HorizontalScope())
            {
                if (GUILayout.Button("Construct"))
                {
                    Generate();
                    this.ApplyModification();
                    SaveMesh();
                    this.ApplyModification();
                }
                if (GUILayout.Button("Generate"))
                {
                    Generate();
                    this.ApplyModification();
                }
                if (GUILayout.Button("Save"))
                {
                    SaveMesh();
                    this.ApplyModification();
                }
            }
        }

        private void SetupReference()
        {
            SerializedProperty meshFilterProperty = this.GetPropertie("_meshFilter");
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<MeshFilter>(meshFilterProperty, _generate.transform, false);
            if (meshFilterProperty.objectReferenceValue == null)
            {
                MeshFilter meshFilter = _generate.gameObject.AddComponent<MeshFilter>();
                meshFilterProperty.objectReferenceValue = meshFilter;
            }
            SerializedProperty meshRendererProperty = this.GetPropertie("_meshRenderer");

            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<MeshRenderer>(meshRendererProperty, _generate.transform, false);
            if (meshRendererProperty.objectReferenceValue == null)
            {
                MeshRenderer meshRenderer = _generate.gameObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = ExtMaterials.GetDefaultMaterial();
                meshRendererProperty.objectReferenceValue = meshRenderer;
            }
        }

        private void Generate()
        {
            SetupReference();
            this.ApplyModification();
            _generate.GeneratePlease();
        }

        private void SaveMesh()
        {
            Mesh mesh = ExtMesh.SaveSelectedMeshObj(_generate.gameObject, true);
            _generate.gameObject.transform.GetOrAddComponent<MeshFilter>().sharedMesh = mesh;
        }
    }
}