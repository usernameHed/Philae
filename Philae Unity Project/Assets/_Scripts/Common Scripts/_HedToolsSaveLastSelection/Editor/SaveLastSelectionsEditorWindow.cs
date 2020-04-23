using hedCommon.extension.editor.editorWindow;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.saveLastSelection
{
    public class SaveLastSelectionsEditorWindow : DecoratorEditorWindow
    {
        public const int NUMBER_SELECTED_OBJECTS = 1000;
        public const int NUMBER_SHOWN_OBJECTS = 20;

        public List<UnityEngine.Object> SelectedObjects = new List<UnityEngine.Object>(NUMBER_SELECTED_OBJECTS);
        public UnityEngine.Object LastSelectedObject;
        public List<UnityEngine.Object> SelectedObjectsWithoutDoublon = new List<UnityEngine.Object>(NUMBER_SHOWN_OBJECTS);
        public int CurrentIndex;

        private const string KEY_EDITOR_PREF_SAVE_LAST_SELECTION_CLOSED = "KEY_EDITOR_PREF_SAVE_LAST_SELECTION_CLOSED";
        private SaveLastSelectionEditorWindowShowUtility _displayInside = null;
        public bool IsClosed { get { return (_displayInside == null); } }

        private static Rect _positionHidedEditorWindow = new Rect(10000, 10000, 0, 0);

        /// <summary>
        /// override it with "new" keyword
        /// </summary>
        public static SaveLastSelectionsEditorWindow ShowSaveLastSelections()
        {
            SaveLastSelectionsEditorWindow window = EditorWindow.GetWindow<SaveLastSelectionsEditorWindow>("", false, typeof(SceneView));
            window.InitConstructor();
            window.SetMinSize(new Vector2(0, 0));
            window.SetMaxSize(new Vector2(0, 0));
            window.position = _positionHidedEditorWindow;
            return (window);
        }

        public bool IsContainingThisType<T>(out UnityEngine.Object found)
        {
            found = null;

            for (int i = SelectedObjects.Count - 1; i >= 0; i--)
            {
                if (SelectedObjects[i] == null)
                {
                    continue;
                }
                if (typeof(T).IsAssignableFrom(SelectedObjects[i].GetType()))
                {
                    found = SelectedObjects[i];
                    return (true);
                }
            }
            return (false);
        }

        public void ToggleOpenCloseDisplay()
        {
            if (_displayInside)
            {
                _displayInside.Close();
                _displayInside = null;
            }
            else
            {
                _displayInside = EditorWindow.GetWindow<SaveLastSelectionEditorWindowShowUtility>("Save Selection", true);
                //_displayInside = ScriptableObject.CreateInstance(typeof(SaveLastSelectionEditorWindowShowUtility)) as SaveLastSelectionEditorWindowShowUtility;
                _displayInside.Show();
            }
            SaveCloseStatus();
        }

        public void Display(Action action)
        {
            _displayInside?.ShowEditorWindow(action);
        }

        public void ConstructTinyEditorWindow()
        {
            bool isClosed = EditorPrefs.GetBool(KEY_EDITOR_PREF_SAVE_LAST_SELECTION_CLOSED);
            if (!isClosed)
            {
                _displayInside = ScriptableObject.CreateInstance(typeof(SaveLastSelectionEditorWindowShowUtility)) as SaveLastSelectionEditorWindowShowUtility;
                _displayInside.ShowUtility();
            }
        }

        public void SaveCloseStatus()
        {
            EditorPrefs.SetBool(KEY_EDITOR_PREF_SAVE_LAST_SELECTION_CLOSED, _displayInside == null);
        }

        public void Reset()
        {
            SelectedObjects.Clear();
            LastSelectedObject = null;
            SelectedObjectsWithoutDoublon.Clear();
            CurrentIndex = -1;
        }
    }
}