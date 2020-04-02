using hedCommon.extension.runtime;
using MoveMouseEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.editor
{
    /// <summary>
    /// all of this function must be called in OnSceneGUI
    /// </summary>
    public static class ExtGUI
    {
        
        
        public delegate void MethodToCallInsideBox();

        /// <summary>
        /// from a rect
        /// Rect lastRect = GUILayoutUtility.GetLastRect();
        /// 
        /// transform it into global position relative to the current EditorWindow we are in
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 GUIToGlobalEditorWindowPosition(Rect lastRect, EditorWindow currentEditorWindow)
        {
            Vector2 worldPosition = GUIUtility.GUIToScreenPoint(new Vector2(lastRect.x, lastRect.y));

            Vector2 editorPosition = new Vector2(currentEditorWindow.position.x, currentEditorWindow.position.y);

            return (new Vector2(editorPosition.x - worldPosition.x, editorPosition.y - worldPosition.y));
        }

        

        /// <summary>
        /// use:
        /// RTargetsEditor rtarget = ExtGUI.CreateEditor<RTargetsEditor, UnityEngine.Object>(item);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemWithEditor"></param>
        /// <returns></returns>
        public static T CreateEditor<T, U>(U itemWithEditor)
            where U : UnityEngine.Object
            where T : Editor
                                        
        {
            return ((T)Editor.CreateEditor(itemWithEditor, typeof(T)));
        }

        


        public static void Box(MethodToCallInsideBox methodToCallInsideBox)
        {
            Rect r = (Rect)EditorGUILayout.BeginVertical("Box");
            //if (GUI.Button(r, GUIContent.none))
            //    Debug.Log("Go here");


            methodToCallInsideBox();

            EditorGUILayout.EndVertical();
        }



        public static bool CreateGridZone(/*VerticalScope zoneToResize, ref Rect savedSizeZone*/)
        {
            /*
            Rect zone = new Rect();

            if (Event.current.type == EventType.Repaint)
            {
                zone = zoneToResize.rect;
                Debug.Log(zone + ", " + Event.current.type);
                savedSizeZone = zone;
            }
            */

            //The material to use when drawing with OpenGL.
            // Find the "Hidden/Internal-Colored" shader, and cache it for use.
            Material material = new Material(Shader.Find("Hidden/Internal-Colored"));

            // Reserve GUI space with a width from 10 to 10000, and a fixed height of 200, and 
            // cache it as a rectangle.
            //Rect layoutRectangle = GUILayoutUtility.GetRect(zone.x, zone.y, zone.width, zone.height);
            Rect layoutRectangle = GUILayoutUtility.GetRect(200, 200, 200, 200);

            // If we are currently in the Repaint event, begin to draw a clip of the size of 
            // previously reserved rectangle, and push the current matrix for drawing.
            GUI.BeginClip(layoutRectangle);

            GL.PushMatrix();
            // Clear the current render buffer, setting a new background colour, and set our
            // material for rendering.
            GL.Clear(true, false, Color.black);
            material.SetPass(0);

            {
                // Start drawing in OpenGL Quads, to draw the background canvas. Set the
                // colour black as the current OpenGL drawing colour, and draw a quad covering
                // the dimensions of the layoutRectangle.
                GL.Begin(GL.QUADS);
                GL.Color(Color.black);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(layoutRectangle.width, 0, 0);
                GL.Vertex3(layoutRectangle.width, layoutRectangle.height, 0);
                GL.Vertex3(0, layoutRectangle.height, 0);
                GL.End();
            }

            {
                // Start drawing in OpenGL Lines, to draw the lines of the grid.
                GL.Begin(GL.LINES);

                // Store measurement values to determine the offset, for scrolling animation,
                // and the line count, for drawing the grid.
                int offset = 0;
                int count = (int)(layoutRectangle.width / 10) + 20;

                for (int i = 0; i < count; i++)
                {
                    // For every line being drawn in the grid, create a colour placeholder; if the
                    // current index is divisible by 5, we are at a major segment line; set this
                    // colour to a dark grey. If the current index is not divisible by 5, we are
                    // at a minor segment line; set this colour to a lighter grey. Set the derived
                    // colour as the current OpenGL drawing colour.
                    Color lineColour = (i % 5 == 0)
                        ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.2f, 0.2f, 0.2f);
                    GL.Color(lineColour);

                    // Derive a new x co-ordinate from the initial index, converting it straight
                    // into line positions, and move it back to adjust for the animation offset.
                    float x = i * 10 - offset;

                    if (x >= 0 && x < layoutRectangle.width)
                    {
                        // If the current derived x position is within the bounds of the
                        // rectangle, draw another vertical line.
                        GL.Vertex3(x, 0, 0);
                        GL.Vertex3(x, layoutRectangle.height, 0);
                    }

                    if (i < layoutRectangle.height / 10)
                    {
                        // Convert the current index value into a y position, and if it is within
                        // the bounds of the rectangle, draw another horizontal line.
                        GL.Vertex3(0, i * 10, 0);
                        GL.Vertex3(layoutRectangle.width, i * 10, 0);
                    }
                }
            }

            // End lines drawing.
            GL.End();

            // Pop the current matrix for rendering, and end the drawing clip.
            GL.PopMatrix();

            GUI.EndClip();

            return (false);
        }

        /// <summary>
        /// create an horizontal line
        /// </summary>
        public static void HorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        public static string DivideLineIntoMultipleLines(string text, int maxLetterPerLines)
        {
            if (text.Length < maxLetterPerLines)
            {
                return (text);
            }

            bool foundSpace = false;
            int indexLastFoundSpace = -1;

            int indexLine = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    foundSpace = true;
                    indexLastFoundSpace = i;
                }

                indexLine++;
                if (indexLine >= maxLetterPerLines)
                {
                    indexLine = 0;
                    if (foundSpace)
                    {
                        text = ExtString.ChangeIndex(text, indexLastFoundSpace, '\n');
                    }
                    else
                    {
                        text = ExtString.ChangeIndex(text, i, '\n');
                    }
                    indexLastFoundSpace = -1;
                    foundSpace = false;
                }
            }
            return (text);
        }

        /// <summary>
        /// create an horizontal line
        /// </summary>
        public static void MainTitleWithLine(string title, float maxWidth = 200)
        {
            using (HorizontalScope horizontalScope = new HorizontalScope(GUILayout.Width(maxWidth)))
            {
                using (VerticalScope verticalScope = new VerticalScope())
                {
                    HorizontalLineThickness(Color.gray, 2, 1, 1, 0.1f, 0.1f, 50);
                }
                using (VerticalScope verticalScope = new VerticalScope())
                {
                    GUILayout.Label(title);
                }
                using (VerticalScope verticalScope = new VerticalScope())
                {
                    HorizontalLineThickness(Color.gray, 2, 1, 1, 0.1f, 0.1f, 50);
                }
            }
        }

        /// <summary>
        /// Create an horizontal line
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness">2 pixels</param>
        /// <param name="paddingTop">1 = 10 pixels</param>
        /// <param name="paddingBottom">1 = 10 pixels</param>
        /// <param name="paddingLeft">percent of the width clamped 0-1</param>
        /// <param name="paddingRight">percent of the width clamped 0-1</param>
        public static void HorizontalLineThickness(Color color,
            int thickness = 2,
            float paddingTop = 1,
            float paddingBottom = 1,
            float paddingLeft = 0.1f,
            float paddingRight = 0.1f,
            float autoWidth = -1)
        {
            paddingTop *= 10;
            paddingBottom *= 10;

            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(paddingBottom + thickness));
            float width = r.width;
            if (autoWidth > 0)
            {
                width = autoWidth;
            }

            paddingLeft = ExtMathf.SetBetween(paddingLeft, 0, 1);
            paddingRight = ExtMathf.SetBetween(paddingRight, 0, 1);

            paddingLeft = paddingLeft * width / 1f;
            paddingRight = paddingRight * width / 1f;



            r.height = thickness;
            r.y += paddingTop / 2;
            r.x -= 2 - paddingLeft;
            r.width += 6 - paddingLeft - paddingRight;
            EditorGUI.DrawRect(r, color);
        }

        /// <summary>
        /// This function have to be called at the end of your GUI.Window() function.
        /// It eat the clic event, so it doesn't propagate thought things below.
        /// </summary>
        /// <param name="current"></param>
        public static void PreventClicGoThought(Event current)
        {
            if ((current.type == EventType.MouseDrag || current.type == EventType.MouseDown)
                && current.button == 0)
            {
                ExtEventEditor.Use();
            }
        }

        /// <summary>
        /// apply a return line
        /// </summary>
        public static void CarriageReturn(int numberOfLine = 1, string optionalCharacter = "", params GUILayoutOption[] options)
        {
            for (int i = 0; i < numberOfLine; i++)
            {
                GUILayout.Label(optionalCharacter, options);
            }
        }

        /// <summary>
        /// draw some blank space
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sizeBlankSpace"></param>
        public static void DisplayBlankSpace(int amount, float sizeBlankSpace)
        {
            for (int i = 0; i < amount; i++)
            {
                GUILayout.Label("", GUILayout.Width(sizeBlankSpace));
            }
        }

        public static bool DrawDisplayDialog(
                string mainTitle = "main title",
                string content = "what's the dialog box say",
                string accept = "Yes",
                string no = "Get me out of here",
                bool replaceMousePositionAtTheEnd = true)
        {
            if (replaceMousePositionAtTheEnd)
            {
                Win32Mouse.SavePosition();
                //Win32Mouse.SetCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));
            }
            if (!EditorUtility.DisplayDialog(mainTitle, content, accept, no))
            {
                if (replaceMousePositionAtTheEnd)
                {
                    Win32Mouse.SetCursorPosition(Win32Mouse.GetLastSavedPosition());
                }
                return (false);
            }
            if (replaceMousePositionAtTheEnd)
            {
                Win32Mouse.SetCursorPosition(Win32Mouse.GetLastSavedPosition());
            }
            return (true);
        }

        public static bool DeleteDisplay(bool askBeforeDelete, string message = "Are you sure you want to delete this ?")
        {
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                GUI.backgroundColor = Color.white;
                if (askBeforeDelete && !ExtGUI.DrawDisplayDialog("Delete", message))
                {
                    GUI.backgroundColor = previous;
                    return (false);
                }
                GUI.backgroundColor = previous;
                return (true);
            }
            GUI.backgroundColor = previous;
            return (false);
        }

        //end class
    }
    //end nameSpace
}