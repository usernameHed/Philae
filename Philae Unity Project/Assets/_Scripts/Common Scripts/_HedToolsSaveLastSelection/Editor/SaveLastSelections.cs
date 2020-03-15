using hedCommon.editor.editorWindow;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace hedCommon.saveLastSelection
{
    public class SaveLastSelections
    {
        private const int NUMBER_SELECTED_OBJECTS = 20;
        private const string KEY_EDITOR_PREF_SAVE_LAST_SELECTION = "KEY_EDITOR_PREF_SAVE_LAST_SELECTION";
        private List<UnityEngine.Object> _selectedObjects = new List<UnityEngine.Object>(NUMBER_SELECTED_OBJECTS);
        private UnityEngine.Object _lastSelectedObject;

        private List<UnityEngine.Object> _selectedObjectsWithoutDoublon = new List<UnityEngine.Object>(NUMBER_SELECTED_OBJECTS);

        private int _currentIndex;
        private TinyEditorWindowSceneView _tinyEditorWindowSceneView;

        public SaveLastSelections()
        {
            _selectedObjects.Clear();
            EditorApplication.update += UpdateEditor;
            _tinyEditorWindowSceneView = new TinyEditorWindowSceneView();
            _tinyEditorWindowSceneView.TinyInit(KEY_EDITOR_PREF_SAVE_LAST_SELECTION, "Save Last Selection", TinyEditorWindowSceneView.DEFAULT_POSITION.UP_LEFT);
            _tinyEditorWindowSceneView.IsClosed = true;
            SceneView.duringSceneGui += OnCustomGUI;
        }

        ~SaveLastSelections()
        {
            EditorApplication.update -= UpdateEditor;
            SceneView.duringSceneGui -= OnCustomGUI;
        }

        private void UpdateEditor()
        {
            AttemptToRemoveNull();
            UnityEngine.Object currentSelectedObject = Selection.activeObject;
            if (currentSelectedObject != null && currentSelectedObject != _lastSelectedObject)
            {
                AddNewSelection(currentSelectedObject);
            }
        }

        private void AddNewSelection(UnityEngine.Object currentSelectedObject)
        {
            _lastSelectedObject = currentSelectedObject;
            if (_selectedObjects.Count >= NUMBER_SELECTED_OBJECTS)
            {
                _selectedObjects.RemoveAt(0);
            }
            _selectedObjects.Add(_lastSelectedObject);
            _currentIndex = _selectedObjects.Count - 1;

            _selectedObjectsWithoutDoublon = ExtList.RemoveRedundancy(_selectedObjects);
        }

        public void DisplayButton()
        {
            if (_currentIndex >= _selectedObjects.Count)
            {
                _currentIndex = _selectedObjects.Count - 1;
            }

            if (GUILayout.Button("..."))
            {
                ExtReflection.OpenEditorWindow(ExtReflection.AllNameAssemblyKnown.SceneView, out Type animationWindowType);
                _tinyEditorWindowSceneView.IsClosed = !_tinyEditorWindowSceneView.IsClosed;
            }
            EditorGUI.BeginDisabledGroup(_selectedObjects.Count == 0);
            {
                if (GUILayout.Button("<") || ExtEventEditor.IsScrollingDown(Event.current, out float delta))
                {
                    //if (Selection.activeObject != null)
                    //{
                        AddToIndex(-1);
                    //}
                    ForceSelection(_selectedObjects[_currentIndex]);
                }
                if (GUILayout.Button(">") || ExtEventEditor.IsScrollingUp(Event.current, out delta))
                {
                    //if (Selection.activeObject != null)
                    //{
                        AddToIndex(1);
                    //}
                    ForceSelection(_selectedObjects[_currentIndex]);
                }
                if (_selectedObjects.Count == 0)
                {
                    GUILayout.Label("-/-");
                }
                else
                {
                    GUILayout.Label((_currentIndex + 1).ToString() + "/" + (_selectedObjects.Count));
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void AttemptToRemoveNull()
        {
            if (_selectedObjects != null && _selectedObjects.IsThereNullInList())
            {
                _selectedObjects = ExtList.CleanNullFromList(_selectedObjects, out bool hasChanged);
                _selectedObjectsWithoutDoublon = ExtList.CleanNullFromList(_selectedObjectsWithoutDoublon, out hasChanged);
            }
        }

        private void ForceSelection(UnityEngine.Object forcedSelection)
        {
            _lastSelectedObject = forcedSelection;
            Selection.activeObject = _lastSelectedObject;
        }

        private void AddToIndex(int add)
        {
            _currentIndex += add;
            _currentIndex = Mathf.Clamp(_currentIndex, 0, _selectedObjects.Count - 1);
        }

        private void OnCustomGUI(SceneView sceneView)
        {
            if (!_tinyEditorWindowSceneView.IsClosed)
            {
                _tinyEditorWindowSceneView.ShowEditorWindow(DrawList, SceneView.currentDrawingSceneView, Event.current);
            }
        }

        private void DrawList()
        {
            GUILayout.Label("previously selected:");
            for (int i = 0; i < _selectedObjectsWithoutDoublon.Count; i++)
            {
                if (_selectedObjectsWithoutDoublon[i] == null)
                {
                    continue;
                }

                GUI.color = (_lastSelectedObject == _selectedObjectsWithoutDoublon[i]) ? Color.green : Color.white;
                if (GUILayout.Button(_selectedObjectsWithoutDoublon[i].name))
                {
                    ForceSelection(_selectedObjectsWithoutDoublon[i]);
                }
            }
        }

        
    }
}