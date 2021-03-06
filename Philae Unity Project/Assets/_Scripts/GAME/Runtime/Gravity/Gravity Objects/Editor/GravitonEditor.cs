﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using extUnityComponents;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.editorGlobal;
using philae.gravity.graviton;
using philae.gravity.attractor;
using philae.data.gravity;
using philae.gravity.physicsBody;
using static philae.gravity.physicsBody.PhysicBody;
using hedCommon.extension.editor.sceneView;
using hedCommon.eventEditor;

namespace philae.gravity.zones
{
    [CustomEditor(typeof(Graviton))]
    public class GravitonEditor : DecoratorComponentsEditor
    {
        private Graviton _graviton;

        public GravitonEditor()
                    : base(showExtension: false, tinyEditorName: "Graviton")
        {

        }

        public override void OnCustomEnable()
        {
            _graviton = GetTarget<Graviton>();
        }

        public override void ShowTinyEditorContent()
        {
            GUILayout.Button("here");
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (ExtPrefabs.IsEditingInPrefabMode(_graviton.gameObject))
            {
                return;
            }

            RigidGraviton rigidGraviton = this.GetValue<RigidGraviton>("_rigidGraviton");
            ExtHandle.DoMultiHandle(rigidGraviton.transform, out bool hasChanged);
            if (hasChanged)
            {
                ConstrainPosition.ApplyConstraint(rigidGraviton.ConstrainPositions, rigidGraviton.transform);
            }
            if (ExtEventEditor.IsKeyDown(KeyCode.F))
            {
                ExtSceneView.FocusOnSelection(this.GetValue<RigidGraviton>("_rigidGraviton").transform.gameObject, 100);
            }
        }
    }
}