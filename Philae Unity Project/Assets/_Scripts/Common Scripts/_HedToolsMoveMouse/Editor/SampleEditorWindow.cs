/// <summary>
/// MIT License
/// Copyright(c) 2019 usernameHed
/// 
///  Permission is hereby granted, free of charge, to any person obtaining a copy
///  of this software and associated documentation files (the "Software"), to deal
///  in the Software without restriction, including without limitation the rights
///  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
///  copies of the Software, and to permit persons to whom the Software is
///  furnished to do so, subject to the following conditions:
/// 
///  The above copyright notice and this permission notice shall be included in all
///  copies or substantial portions of the Software.
/// 
///  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
///  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
///  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
///  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
///  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
///  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
///  SOFTWARE.
/// </summary>

using hedCommon.extension.editor;
using UnityEditor;
using UnityEngine;

namespace hedCommon.MoveMouseEditor
{
    /// <summary>
    /// exemple of an EditorWindow
    /// </summary>
    public class SampleEditorWindow : EditorWindow
    {
        [MenuItem("TOOLS/Mouse Editor/Sample")]
        static void Init()
        {
            SampleEditorWindow window = (SampleEditorWindow)EditorWindow.GetWindow(typeof(SampleEditorWindow));
            window.Show();
        }

        /// <summary>
        /// display all the GUI inside the editorWindow
        /// </summary>
        private void OnGUI()
        {
            DisplayButtons();
        }

        /// <summary>
        /// display the core of the editorWindow
        /// </summary>
        private void DisplayButtons()
        {
            DisplaySaveButton();
            GUILayout.Label("");
            DisplayLoadButton();
        }

        /// <summary>
        /// display save button & execute the saving position if button pressed
        /// </summary>
        private void DisplaySaveButton()
        {
            if (GUILayout.Button("Save Mouse Position"))
            {
                Win32Mouse.SavePosition();
                Debug.Log("mouse position saved !");
            }
        }

        /// <summary>
        /// display load button & execute the loading position if button pressed
        /// </summary>
        private void DisplayLoadButton()
        {
            if (GUILayout.Button("Load previously saved position"))
            {
                Win32Mouse.LoadPreviouslySavedPosition();
                Debug.Log("mouse moved to the previously saved position !");
            }
        }
    }
}

