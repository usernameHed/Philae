using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using hedCommon.extension.editor.editorWindow;

namespace hedCommon.saveLastSelection
{
    public class SaveLastSelectionEditorWindowShowUtility : DecoratorEditorWindow
    {
        public Action ShowInside;

        /// <summary>
        /// called every frame, 
        /// </summary>
        /// <param name="action"></param>
        public void ShowEditorWindow(Action action)
        {
            ShowInside = action;
        }

        private void OnGUI()
        {
            ShowInside?.Invoke();
        }
    }
}