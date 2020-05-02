using hedCommon.eventEditor;
using hedCommon.extension.editor.editorWindow;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor.screenCapture
{
    [InitializeOnLoad]
    public static class ScreenCaptureEditorWindow
    {
        static ScreenCaptureEditorWindow()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }
            EditorApplication.update += OnCustomUpdate;
        }

        private static void OnCustomUpdate()
        {
            Event lastKeyChange = ExtEventOnUpdate.LastKeyChange;

            if (lastKeyChange != null
                && lastKeyChange.keyCode == KeyCode.S
                && lastKeyChange.control
                && lastKeyChange.alt)
            {
                EditorWindow focusedWindow = ExtEditorWindow.FocusedWindow();
                if (focusedWindow)
                {
                    Texture screenShot = focusedWindow.TakeEditorWindowCapture();
                    Object asset = screenShot.SaveScreenCapture(focusedWindow.GetType().Name);
                    ExtWindowComands.ShowInExplorer(asset);
                }
            }
        }
    }
}