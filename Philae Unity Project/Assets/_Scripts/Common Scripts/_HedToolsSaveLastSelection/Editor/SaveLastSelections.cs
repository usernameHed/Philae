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
            _isClosed = _saveLastSelectionsEditorWindow.IsClosed;
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
                        _saveLastSelectionsEditorWindow.ToggleOpenCloseDisplay();
                        _isClosed = _saveLastSelectionsEditorWindow.IsClosed;
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
                            GUIContent gUIContent = new GUIContent("-/-", "there is no previously selected objects");
                            GUILayout.Label(gUIContent);
                        }
                        else
                        {
                            string showCount = (_saveLastSelectionsEditorWindow.CurrentIndex + 1).ToString() + "/" + (_saveLastSelectionsEditorWindow.SelectedObjects.Count);
                            GUIContent gUIContent = new GUIContent(showCount, "Scroll Up/Down to browse previous/next");
                            GUILayout.Label(gUIContent);
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
            while (_saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon.Count >= SaveLastSelectionsEditorWindow.NUMBER_SHOWN_OBJECTS)
            {
                _saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon.RemoveAt(0);
            }
        }



        private void ForceSelection(UnityEngine.Object forcedSelection, bool select = true)
        {
            _saveLastSelectionsEditorWindow.LastSelectedObject = forcedSelection;
            if (select)
            {
                Selection.activeObject = _saveLastSelectionsEditorWindow.LastSelectedObject;
                EditorGUIUtility.PingObject(_saveLastSelectionsEditorWindow.LastSelectedObject);
            }
            else
            {
                EditorGUIUtility.PingObject(_saveLastSelectionsEditorWindow.LastSelectedObject);
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
            if (!_saveLastSelectionsEditorWindow.IsClosed)
            {
                _saveLastSelectionsEditorWindow.Display(DrawList);
            }
            if (_isClosed != _saveLastSelectionsEditorWindow.IsClosed)
            {
                _saveLastSelectionsEditorWindow.SaveCloseStatus();
                _isClosed = _saveLastSelectionsEditorWindow.IsClosed;
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
                using (HorizontalScope horizontalScope = new HorizontalScope())
                {
                    GUIContent buttonPingContent = new GUIContent("p", "Ping object without selecting it");
                    if (GUILayout.Button(buttonPingContent, ExtGUIStyles.microButton, GUILayout.Width(17)))
                    {
                        ForceSelection(_saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon[i], false);
                    }
                    string nameObjectToSelect = _saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon[i].name;
                    GUIContent buttonSelectContent = new GUIContent(nameObjectToSelect, nameObjectToSelect + ": Select and Ping");
                    if (GUILayout.Button(buttonSelectContent, ExtGUIStyles.microButton))
                    {
                        ForceSelection(_saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon[i]);
                    }
                    GUIContent buttonDeletContent = new GUIContent("x", "Delete object from list");
                    if (GUILayout.Button(buttonDeletContent, ExtGUIStyles.microButton, GUILayout.Width(17)))
                    {
                        UnityEngine.Object toRemove = _saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon[i];
                        _saveLastSelectionsEditorWindow.SelectedObjects.Remove(toRemove, true);
                        _saveLastSelectionsEditorWindow.SelectedObjectsWithoutDoublon.Remove(toRemove);
                    }
                }
            }

            GUI.color = Color.white;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUI.BeginDisabledGroup(_saveLastSelectionsEditorWindow.SelectedObjects.Count == 0);
            {
                if (GUILayout.Button("clear", ExtGUIStyles.microButton))
                {
                    _saveLastSelectionsEditorWindow.Reset();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}