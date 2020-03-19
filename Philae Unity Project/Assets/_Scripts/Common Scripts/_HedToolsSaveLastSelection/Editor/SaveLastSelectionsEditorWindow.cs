using hedCommon.editor.editorWindow;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.saveLastSelection
{
    public class SaveLastSelectionsEditorWindow : DecoratorEditorWindow
    {
        public const int NUMBER_SELECTED_OBJECTS = 20;

        public List<UnityEngine.Object> SelectedObjects = new List<UnityEngine.Object>(NUMBER_SELECTED_OBJECTS);
        public UnityEngine.Object LastSelectedObject;
        public List<UnityEngine.Object> SelectedObjectsWithoutDoublon = new List<UnityEngine.Object>(NUMBER_SELECTED_OBJECTS);
        public int CurrentIndex;

        public TinyEditorWindowSceneView TinyEditorWindowSceneView = new TinyEditorWindowSceneView();
        private const string KEY_EDITOR_PREF_SAVE_LAST_SELECTION = "KEY_EDITOR_PREF_SAVE_LAST_SELECTION";


        /// <summary>
        /// override it with "new" keyword
        /// </summary>
        [MenuItem("PERSO/DecoratorWindow/SaveLastSelections")]
        public static SaveLastSelectionsEditorWindow ShowSaveLastSelections()
        {
            Rect position = new Rect(0, 0, 500, 30);
            SaveLastSelectionsEditorWindow window = EditorWindow.GetWindow<SaveLastSelectionsEditorWindow>("SaveLastSelections");

            Debug.Log("Get Window: " + window.name + ", " + window.GetInstanceID());

            window.InitConstructor();
            window.SetMinSize(new Vector2(0, 0));
            window.SetMaxSize(new Vector2(500, 30));
            window.position = position;



            //GUI.skin = = ExtGUIStyles.invisibleEditorWindow;

            window.ConstructTinyEditorWindow();

            return (window);
        }

        public void ConstructTinyEditorWindow()
        {
            TinyEditorWindowSceneView.TinyInit(KEY_EDITOR_PREF_SAVE_LAST_SELECTION, "Save Last Selection", TinyEditorWindowSceneView.DEFAULT_POSITION.UP_LEFT);
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