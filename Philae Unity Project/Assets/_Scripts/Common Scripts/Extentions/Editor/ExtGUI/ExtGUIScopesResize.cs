using hedCommon.eventEditor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.editor
{
    public static class ExtGUIScopesResize
    {
        private static float _diffMousePosition = 0;

        /// <summary>
        /// /// resize a VerticalScope, use:
        /// 
        /////////////////   private variable:
        ///
        /// private Rect _saveResizableZone = new Rect();
        /// private bool _isResizing = false;
        /// private float _widthScope = 200;
        /// 
        /////////////////   inside a function
        ///
        /// VerticalScope leftZone;
        /// using (leftZone = ExtGUIScopes.Verti(Color.magenta, GUILayout.MaxWidth(_widthScope)))
        /// {
        ///     //ExtGUILabel.Label("content");
        /// }
        /// ExtGUI.ResizeVerticalScopeRight(
        ///             leftZone,
        ///             currentEditorWindow: editorWindow,
        ///             ref _saveResizableZone,
        ///             ref _isResizing,
        ///             ref _widthScope,
        ///             color: Color.black,
        ///             widthScroll: 10f,
        ///             widthDisplay: 1f,
        ///             minWidth: 20f,
        ///             maxWidth: 300f);
        ///            
        /// </summary>
        /// <param name="zoneToResize">VerticalScope to resize</param>
        /// <param name="currentEditorWindow">current EditorWindow where the VerticalScope is</param>
        /// <param name="savedSizeZone">used to save saved size</param>
        /// <param name="isResizing">used to save resizing state</param>
        /// <param name="widthScope">used to save width resize</param>
        /// <param name="color">Color of the handle resize</param>
        /// <param name="widthScroll">size of the scale area</param>
        /// <param name="widthDisplay">size of the visible scale area</param>
        /// <param name="minWidth">widh min for the scale</param>
        /// <param name="maxWidth">width max for the scale</param>
        public static void ResizeVerticalScopeRight(
            VerticalScope zoneToResize,
            EditorWindow currentEditorWindow,
            ref Rect savedSizeZone,
            ref bool isResizing,
            ref float widthScope,
            Color color,
            float widthScroll = 5f,
            float widthDisplay = 3f,
            float minWidth = 20,
            float maxWidth = 300)
        {
            savedSizeZone = zoneToResize.rect;

            float xScroll = savedSizeZone.x + savedSizeZone.width - widthScroll / 2;
            float xDisplay = savedSizeZone.x + savedSizeZone.width - widthDisplay;
            float y = savedSizeZone.y;
            float height = savedSizeZone.height;

            Rect cursorChangeRect = new Rect(xScroll, y, widthScroll, height);
            Rect displayZone = new Rect(xDisplay, y, widthDisplay, height);

            GUI.DrawTexture(displayZone, ExtTexture.GetTexture(color));

            if (!isResizing)
            {
                EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeHorizontal);
            }
            else
            {
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, 100000, 100000), MouseCursor.ResizeHorizontal);
            }

            if (Event.current.type == EventType.MouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
            {
                isResizing = true;
                _diffMousePosition = Event.current.mousePosition.x - widthScope;
                ExtEventEditor.Use();
            }
            if (isResizing && Event.current.type == EventType.MouseDrag)
            {
                float newWidth = ExtMathf.SetBetween(Event.current.mousePosition.x - _diffMousePosition, minWidth, maxWidth);
                if (newWidth != widthScope)
                {
                    widthScope = newWidth;
                    currentEditorWindow.Repaint();
                }
                ExtEventEditor.Use();
            }
            if (isResizing && Event.current.type == EventType.MouseUp)
            {
                isResizing = false;
            }
        }
        //end class
    }

    //end nameSpace
}