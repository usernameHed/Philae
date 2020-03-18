﻿using hedCommon.editor.editorWindow;
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

        /// <summary>
        /// override it with "new" keyword
        /// </summary>
        [MenuItem("PERSO/DecoratorWindow/SaveLastSelections")]
        public static SaveLastSelectionsEditorWindow ShowSaveLastSelections()
        {
            Rect position = new Rect(0, 0, 0, 0);
            SaveLastSelectionsEditorWindow window = EditorWindow.GetWindow<SaveLastSelectionsEditorWindow>("SaveLastSelections");
            
            window.InitConstructor();
            window.SetMinSize(new Vector2(0, 0));
            window.SetMaxSize(new Vector2(0, 0));
            window.position = position;

            return (window);
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