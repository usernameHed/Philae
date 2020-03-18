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
        private SaveLastSelectionsEditorWindow _saveLastSelectionsEditorWindow;
        private TinyEditorWindowSceneView _tinyEditorWindowSceneView;
        private const string KEY_EDITOR_PREF_SAVE_LAST_SELECTION = "KEY_EDITOR_PREF_SAVE_LAST_SELECTION";


        public SaveLastSelections()
        {
            _saveLastSelectionsEditorWindow = SaveLastSelectionsEditorWindow.ShowSaveLastSelections();

            _tinyEditorWindowSceneView = new TinyEditorWindowSceneView();
            _tinyEditorWindowSceneView.TinyInit(KEY_EDITOR_PREF_SAVE_LAST_SELECTION, "Save Last Selection", TinyEditorWindowSceneView.DEFAULT_POSITION.UP_LEFT);
            _tinyEditorWindowSceneView.IsClosed = true;

            EditorApplication.update += UpdateEditor;
            SceneView.duringSceneGui += OnCustomSceneGUI;
        }

        ~SaveLastSelections()
        {
            EditorApplication.update -= UpdateEditor;
            SceneView.duringSceneGui -= OnCustomSceneGUI;
        }

        private void UpdateEditor()
        {
            AttemptToRemoveNull();
            UnityEngine.Object currentSelectedObject = Selection.activeObject;
            if (currentSelectedObject != null && currentSelectedObject != _saveLastSelectionsEditorWindow.LastSelectedObject)
            {
                AddNewSelection(currentSelectedObject);
            }
        }

        private void AttemptToRemoveNull()
        {
            if (_saveLastSelectionsEditorWindow.SelectedObjects != null && _saveLastSelectionsEditorWindow.SelectedObjects.IsThereNullInList())
            {
                _saveLastSelectionsEditorWindow.SelectedObjects = CleanNullFromList(_saveLastSelectionsEditorWindow.SelectedObjects, out bool hasChanged);
                _saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon = CleanNullFromList(_saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon, out hasChanged);
            }
        }

        /// <summary>
        /// Clean  null item (do not remove items, remove only the list)
        /// </summary>
        /// <param name="listToClean"></param>
        /// <returns>true if list changed</returns>
        public static List<UnityEngine.Object> CleanNullFromList(List<UnityEngine.Object> listToClean, out bool hasChanged)
        {
            hasChanged = false;
            if (listToClean == null)
            {
                return (listToClean);
            }
            for (int i = listToClean.Count - 1; i >= 0; i--)
            {
                if (listToClean[i] == null || listToClean[i].ToString() == "null")
                {
                    listToClean.RemoveAt(i);
                    hasChanged = true;
                }
            }
            return (listToClean);
        }

        public void DisplayButton()
        {
            if (_saveLastSelectionsEditorWindow.CurrentIndex >= _saveLastSelectionsEditorWindow.SelectedObjects.Count)
            {
                _saveLastSelectionsEditorWindow.CurrentIndex = _saveLastSelectionsEditorWindow.SelectedObjects.Count - 1;
            }

            if (GUILayout.Button("..."))
            {
                ExtReflection.OpenEditorWindow(ExtReflection.AllNameAssemblyKnown.SceneView, out Type animationWindowType);

                if (_saveLastSelectionsEditorWindow == null)
                {
                    _saveLastSelectionsEditorWindow = SaveLastSelectionsEditorWindow.ShowSaveLastSelections();
                }

                _tinyEditorWindowSceneView.IsClosed = !_tinyEditorWindowSceneView.IsClosed;
            }
            EditorGUI.BeginDisabledGroup(_saveLastSelectionsEditorWindow.SelectedObjects.Count == 0);
            {
                bool isScrollingDown = ExtEventEditor.IsScrollingDown(Event.current, out float delta);
                if (GUILayout.Button("<") || isScrollingDown)
                {
                    AddToIndex(-1);
                    ForceSelection(_saveLastSelectionsEditorWindow.SelectedObjects[_saveLastSelectionsEditorWindow.CurrentIndex]);
                }
                bool isScrollingUp = ExtEventEditor.IsScrollingUp(Event.current, out delta);
                if (GUILayout.Button(">") || isScrollingUp)
                {
                    AddToIndex(1);
                    ForceSelection(_saveLastSelectionsEditorWindow.SelectedObjects[_saveLastSelectionsEditorWindow.CurrentIndex]);
                }
                if (_saveLastSelectionsEditorWindow.SelectedObjects.Count == 0)
                {
                    GUILayout.Label("-/-");
                }
                else
                {
                    GUILayout.Label((_saveLastSelectionsEditorWindow.CurrentIndex + 1).ToString() + "/" + (_saveLastSelectionsEditorWindow.SelectedObjects.Count));
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void AddNewSelection(UnityEngine.Object currentSelectedObject)
        {
            _saveLastSelectionsEditorWindow.LastSelectedObject = currentSelectedObject;
            if (_saveLastSelectionsEditorWindow.SelectedObjects.Count >= SaveLastSelectionsEditorWindow.NUMBER_SELECTED_OBJECTS)
            {
                _saveLastSelectionsEditorWindow.SelectedObjects.RemoveAt(0);
            }
            _saveLastSelectionsEditorWindow.SelectedObjects.Add(_saveLastSelectionsEditorWindow.LastSelectedObject);
            _saveLastSelectionsEditorWindow.CurrentIndex = _saveLastSelectionsEditorWindow.SelectedObjects.Count - 1;

            _saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon = ExtList.RemoveRedundancy(_saveLastSelectionsEditorWindow.SelectedObjects);
        }



        private void ForceSelection(UnityEngine.Object forcedSelection, bool select = true)
        {
            _saveLastSelectionsEditorWindow.LastSelectedObject = forcedSelection;
            if (select)
            {
                Selection.activeObject = _saveLastSelectionsEditorWindow.LastSelectedObject;
            }
            else
            {
                ExtSelection.Ping(_saveLastSelectionsEditorWindow.LastSelectedObject);
            }
        }

        private void AddToIndex(int add)
        {
            _saveLastSelectionsEditorWindow.CurrentIndex += add;
            _saveLastSelectionsEditorWindow.CurrentIndex = Mathf.Clamp(_saveLastSelectionsEditorWindow.CurrentIndex, 0, _saveLastSelectionsEditorWindow.SelectedObjects.Count - 1);
        }

        private void OnCustomSceneGUI(SceneView sceneView)
        {
            if (_tinyEditorWindowSceneView == null)
            {
                return;
            }
            if (!_tinyEditorWindowSceneView.IsClosed)
            {
                _tinyEditorWindowSceneView.ShowEditorWindow(DrawList, SceneView.currentDrawingSceneView, Event.current);
                /*
                if (_tinyEditorWindowSceneView.IsMouseOver())
                {
                    if (ExtEventEditor.IsScrollingDown(Event.current, out float delta))
                    {
                        AddToIndex(-1);
                        ForceSelection(_saveLastSelectionsEditorWindow.SelectedObjects[_saveLastSelectionsEditorWindow.CurrentIndex]);
                        ExtEventEditor.Use();
                    }
                    if (ExtEventEditor.IsScrollingUp(Event.current, out delta))
                    {
                        AddToIndex(1);
                        ForceSelection(_saveLastSelectionsEditorWindow.SelectedObjects[_saveLastSelectionsEditorWindow.CurrentIndex]);
                        ExtEventEditor.Use();
                    }
                }
                */
            }

        }

        private void DrawList()
        {
            GUILayout.Label("previously selected:");
            for (int i = 0; i < _saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon.Count; i++)
            {
                if (_saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon[i] == null)
                {
                    continue;
                }

                GUI.color = (_saveLastSelectionsEditorWindow.LastSelectedObject == _saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon[i]) ? Color.green : Color.white;
                using (ExtGUIScopes.Horiz())
                {
                    if (ExtGUIButtons.ButtonImage("ping", GUILayout.Width(17), GUILayout.Height(17)))
                    {
                        ForceSelection(_saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon[i], false);
                    }
                    if (GUILayout.Button(_saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon[i].name))
                    {
                        ForceSelection(_saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon[i]);
                    }
                }
            }

            GUI.color = Color.white;
            ExtGUI.HorizontalLine();

            EditorGUI.BeginDisabledGroup(_saveLastSelectionsEditorWindow.SelectedObjects.Count == 0);
            {
                if (GUILayout.Button("clear"))
                {
                    _saveLastSelectionsEditorWindow.Reset();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}