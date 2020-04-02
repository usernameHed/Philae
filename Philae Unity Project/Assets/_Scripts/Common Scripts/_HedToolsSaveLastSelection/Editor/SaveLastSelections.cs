using hedCommon.extension.editor;
using hedCommon.extension.editor.editorWindow;
using hedCommon.extension.runtime;
using hedCommon.time;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.saveLastSelection
{
    public class SaveLastSelections
    {
        private SaveLastSelectionsEditorWindow _saveLastSelectionsEditorWindow;
        private bool _isInit = false;
        private bool _isClosed = false;
        private FrequencyCoolDown _frequencyCoolDown = new FrequencyCoolDown();

        private const int _heightText = 8;
        private const int _widthButtons = 17;
        private const int _heightButtons = 14;

        public SaveLastSelections()
        {
            _isInit = false;
            EditorApplication.update += UpdateEditor;
            SceneView.duringSceneGui += OnCustomSceneGUI;
        }

        ~SaveLastSelections()
        {
            EditorApplication.update -= UpdateEditor;
            SceneView.duringSceneGui -= OnCustomSceneGUI;
        }

        public void Init()
        {
            _isInit = true;
            _saveLastSelectionsEditorWindow = SaveLastSelectionsEditorWindow.ShowSaveLastSelections();
            _isClosed = _saveLastSelectionsEditorWindow.TinyEditorWindowSceneView.IsClosed;
        }

        private void UpdateEditor()
        {
            if (_saveLastSelectionsEditorWindow == null)
            {
                if (_frequencyCoolDown.IsNotRunning())
                {
                    _saveLastSelectionsEditorWindow = SaveLastSelectionsEditorWindow.ShowSaveLastSelections();
                    _frequencyCoolDown.StartCoolDown(2f);
                }
                return;
            }

            AttemptToRemoveNull();
            UnityEngine.Object currentSelectedObject = Selection.activeObject;
            if (currentSelectedObject != null && currentSelectedObject != _saveLastSelectionsEditorWindow.LastSelectedObject)
            {
                AddNewSelection(currentSelectedObject);
            }
        }

        private void AttemptToRemoveNull()
        {
            if (_saveLastSelectionsEditorWindow.SelectedObjects != null && IsThereNullInList(_saveLastSelectionsEditorWindow.SelectedObjects))
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

        public static bool IsThereNullInList(List<UnityEngine.Object> listToClean)
        {
            for (int i = 0; i < listToClean.Count; i++)
            {
                if (listToClean[i] == null || listToClean[i].ToString() == "null")
                {
                    return (true);
                }
            }
            return (false);
        }

        public void DisplaySelectionsButtons()
        {
            if (_saveLastSelectionsEditorWindow == null)
            {
                return;
            }

            if (_saveLastSelectionsEditorWindow.CurrentIndex >= _saveLastSelectionsEditorWindow.SelectedObjects.Count)
            {
                _saveLastSelectionsEditorWindow.CurrentIndex = _saveLastSelectionsEditorWindow.SelectedObjects.Count - 1;
            }

            using (VerticalScope vertical = new VerticalScope())
            {
                GUILayout.Label("Browse selections", ExtGUIStyles.miniTextCentered, GUILayout.Height(_heightText));
                using (HorizontalScope horizontal = new HorizontalScope())
                {
                    if (GUILayout.Button("...", ExtGUIStyles.microButton))
                    {
                        ExtEditorWindow.OpenEditorWindow(ExtEditorWindow.AllNameAssemblyKnown.SceneView, out Type animationWindowType);

                        if (_saveLastSelectionsEditorWindow == null)
                        {
                            _saveLastSelectionsEditorWindow = SaveLastSelectionsEditorWindow.ShowSaveLastSelections();
                        }

                        _saveLastSelectionsEditorWindow.TinyEditorWindowSceneView.IsClosed = !_saveLastSelectionsEditorWindow.TinyEditorWindowSceneView.IsClosed;
                        _saveLastSelectionsEditorWindow.SaveCloseStatus();
                        _isClosed = _saveLastSelectionsEditorWindow.TinyEditorWindowSceneView.IsClosed;
                    }
                    EditorGUI.BeginDisabledGroup(_saveLastSelectionsEditorWindow.SelectedObjects.Count == 0);
                    {
                        bool isScrollingDown = ExtEventEditor.IsScrollingDown(Event.current, out float delta);
                        if (GUILayout.Button("<", ExtGUIStyles.microButton) || isScrollingDown)
                        {
                            AddToIndex(-1);
                            ForceSelection(_saveLastSelectionsEditorWindow.SelectedObjects[_saveLastSelectionsEditorWindow.CurrentIndex]);
                        }
                        bool isScrollingUp = ExtEventEditor.IsScrollingUp(Event.current, out delta);
                        if (GUILayout.Button(">", ExtGUIStyles.microButton) || isScrollingUp)
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
            }
            GUILayout.FlexibleSpace();
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
            if (!_isInit)
            {
                Init();
            }
            if (_saveLastSelectionsEditorWindow == null)
            {
                return;
            }
            if (_saveLastSelectionsEditorWindow.TinyEditorWindowSceneView == null)
            {
                return;
            }
            if (!_saveLastSelectionsEditorWindow.TinyEditorWindowSceneView.IsClosed)
            {
                _saveLastSelectionsEditorWindow.TinyEditorWindowSceneView.ShowEditorWindow(DrawList, SceneView.currentDrawingSceneView, Event.current);
                
            }
            if (_isClosed != _saveLastSelectionsEditorWindow.TinyEditorWindowSceneView.IsClosed)
            {
                _saveLastSelectionsEditorWindow.SaveCloseStatus();
                _isClosed = _saveLastSelectionsEditorWindow.TinyEditorWindowSceneView.IsClosed;
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