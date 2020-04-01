using hedCommon.editor.editorWindow;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace extUnityComponents
{
    public class RigidBodyCenterOfMass
    {
        private Rigidbody _currentTarget = null;
        private RigidBodyAdditionalMonobehaviourSettings _specialSettings;
        private DecoratorComponentsEditor _currentEditor;

        private bool _moveCenterOfMass = false;  //keep this value outside of the inspected gameObject
        private bool _hasBeenInternallyInit = false;
        private bool _cachedToolsMode;

        private TinyEditorWindowSceneView _tinyCenterOfMassWindow;
        private readonly string KEY_EDITOR_PREF_CENTER_OF_MASS_WINDOW = "KEY_EDITOR_PREF_CENTER_OF_MASS_WINDOW";

        public void Init(Rigidbody parent, DecoratorComponentsEditor current, RigidBodyAdditionalMonobehaviourSettings specialSettings)
        {
            _currentTarget = parent;
            _currentEditor = current;
            _moveCenterOfMass = false;
            _hasBeenInternallyInit = false;
            _specialSettings = specialSettings;
        }

        public void InitOnFirstOnSceneGUI()
        {
            InitTinyEditorWindow();
        }

        private void InitTinyEditorWindow()
        {
            _tinyCenterOfMassWindow = new TinyEditorWindowSceneView();
            _tinyCenterOfMassWindow.TinyInit(KEY_EDITOR_PREF_CENTER_OF_MASS_WINDOW, "Center Of Mass Tool", TinyEditorWindowSceneView.DEFAULT_POSITION.MIDDLE_RIGHT);
        }

        public void CustomOnInspectorGUI()
        {
            using (VerticalScope verticalScope = new VerticalScope(EditorStyles.helpBox))
            {
                using (HorizontalScope horizontalScope = new HorizontalScope())
                {
                    GUILayout.Label("Change Center Of Mass: ");

                    if (DisplayToggle(_currentTarget.transform))
                    {
                        CenterOfMassStateChanged();
                    }
                }
                if (_moveCenterOfMass && IsParentAPrefabs(_currentTarget.transform))
                {
                    GUI.color = Color.yellow;
                    GUILayout.Label("/!\\ Can't change center of mass of prefabs");
                }
            }
        }





        private bool DisplayToggle(Transform target)
        {
            bool disable = false;
            string messageButton = (_moveCenterOfMass) ? "Apply [M]" : "Move [M]";
            bool previousState = _moveCenterOfMass;

            EditorGUI.BeginDisabledGroup(disable);
            {
                _moveCenterOfMass = GUILayout.Toggle(_moveCenterOfMass, messageButton, EditorStyles.miniButton);
            }
            EditorGUI.EndDisabledGroup();

            return (previousState != _moveCenterOfMass);
        }

        public bool IsParentAPrefabs(Transform parent)
        {
            return (PrefabUtility.IsPartOfAnyPrefab(parent.gameObject));
        }

        /// <summary>
        /// called when the lock state changed
        /// </summary>
        private void CenterOfMassStateChanged()
        {
            if (_moveCenterOfMass)
            {
                _tinyCenterOfMassWindow.IsClosed = false;
                if (!_hasBeenInternallyInit)
                {
                    InitCenterOfMass();
                }
            }
            else
            {
                CustomDisable();
            }
        }

        /// <summary>
        /// called for initializing the parent state and all his children state
        /// </summary>
        /// <param name="parent"></param>
        private void InitCenterOfMass()
        {
            _cachedToolsMode = Tools.hidden;
            Tools.hidden = true;
        }

        /// <summary>
        /// clear childs and remap original pivot tool
        /// </summary>
        public void CustomDisable()
        {
            Tools.hidden = _cachedToolsMode;
            _hasBeenInternallyInit = false;
        }

        public void CustomOnSceneGUI()
        {
            if (_currentTarget == null)
            {
                return;
            }

            if (IsPressingM(Event.current))
            {
                _moveCenterOfMass = !_moveCenterOfMass;
                CenterOfMassStateChanged();
                _currentEditor.Repaint();
            }

            if (_moveCenterOfMass)
            {
                Vector3 centerOfMassPosition = ExtHandle.DoHandleMove(_currentTarget.transform.position + _currentTarget.centerOfMass, _currentTarget.transform.rotation, out bool hasChanged);
                if (hasChanged)
                {
                    _currentTarget.centerOfMass = centerOfMassPosition - _currentTarget.transform.position;
                    _specialSettings.SetCenterOfMass(_currentTarget.centerOfMass);
                    _currentEditor.Repaint();
                }
                ShowTinyEditor();
            }
            ExtHandle.DrawSphereArrow(Color.green, _currentTarget.transform.position
                + _currentTarget.transform.forward * _currentTarget.centerOfMass.z
                + _currentTarget.transform.right * _currentTarget.centerOfMass.x
                + _currentTarget.transform.up * _currentTarget.centerOfMass.y, 0.1f);
        }


        /// <summary>
        /// return true if we press on J
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private bool IsPressingM(Event current)
        {
            if ((current.keyCode == KeyCode.M && current.type == EventType.KeyUp))
            {
                return (true);
            }
            return (false);
        }

        private void ShowTinyEditor()
        {
            _tinyCenterOfMassWindow.ShowEditorWindow(DisplayCenterOfMassWindow, SceneView.currentDrawingSceneView, Event.current);
            if (_tinyCenterOfMassWindow.IsClosed)
            {
                _moveCenterOfMass = false;
                EditorUtility.SetDirty(_currentTarget);
                CenterOfMassStateChanged();
            }
        }

        private void DisplayCenterOfMassWindow()
        {
            if (GUILayout.Button("Restore Default"))
            {
                ExtUndo.Record(_currentTarget, "Tool: lock children move");
                RestorDefault();
                _currentEditor.Repaint();
            }
            Vector3 centerOfMass = ExtGUIVectorFields.Vector3Field(_currentTarget.centerOfMass, _currentTarget, out bool hasChanged, "Center of mass:");
            if (hasChanged)
            {
                _currentTarget.centerOfMass = centerOfMass;
                _currentEditor.Repaint();
            }
        }

        public void RestorDefault()
        {
            _currentTarget.ResetCenterOfMass();
        }
    }
}