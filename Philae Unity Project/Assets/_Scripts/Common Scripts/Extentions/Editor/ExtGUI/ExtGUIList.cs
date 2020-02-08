using hedCommon.extension.runtime;
using System;
using MoveMouseEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.editor
{
    public static class ExtGUIList
    {
        public delegate void MethodToCallWhenDivid<T>(T item);

        /// <summary>
        /// Display a list, divide it into chunks
        /// 
        /// use:
        /// ExtGUIList.DivideListAndDisplay<RCamera>(_rCamerasUpdater.RCamerasLister.Rcameras.ToArray(), SelectCamera);
        /// 
        /// private void SelectCamera(RCamera camera)
        /// {
        ///     if (ExtGUIButtons.Button(camera.name))
        ///     {
        ///         Selection.activeGameObject = camera.gameObject;
        ///     }
        /// }
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="method"></param>
        public static void DivideListAndDisplay<T>(T[] list, MethodToCallWhenDivid<T> method, int maxElementPerColomn = 10)
        {
            int numberOfDivision = GetNumberDivision(list.Length, maxElementPerColomn);
            DivideIn(numberOfDivision, list, method);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int GetNumberDivision(int sizeArray, int maxElementPerColomn = 10)
        {
            if (sizeArray <= 0 || maxElementPerColomn <= 0)
            {
                return (1);
            }

            int numberDivision = sizeArray / maxElementPerColomn;
            if (numberDivision <= 0)
            {
                return (1);
            }
            return (numberDivision);
        }

        private static void DivideIn<T>(int numberVericalScope, T[] lockcams, MethodToCallWhenDivid<T> method)
        {
            int indexInList = 0;

            int numberInEachList = lockcams.Length / numberVericalScope;


            for (int k = 0; k < numberVericalScope; k++)
            {
                int start = 0 + (k * numberInEachList);
                int end = numberInEachList * (k + 1);

                if (end <= start)
                {
                    continue;
                }

                using (VerticalScope VerticalScope = new VerticalScope())
                {
                    for (int i = start; i < end; i++)
                    {
                        method(lockcams[indexInList]);
                        indexInList++;
                    }
                    if (k + 1 >= numberVericalScope && indexInList < lockcams.Length)
                    {
                        method(lockcams[indexInList]);
                    }
                }
            }
        }


        public static bool DeleteInList<T>(List<T> list, T itemToDelete, bool askBeforeDelete)
        {
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                if (askBeforeDelete && !ExtGUI.DrawDisplayDialog("Delete item in list", "are you sure you want to delete this ?"))
                {
                    GUI.backgroundColor = previous;
                    return (false);
                }

                list.Remove(itemToDelete);
                GUI.backgroundColor = previous;
                return (true);
            }
            GUI.backgroundColor = previous;
            return (false);
        }

        /// <summary>
        /// move up/down elements inside a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">list where the elements are</param>
        /// <param name="itemToMove">item to move</param>
        /// <param name="displayBlankSpaceIfNothing">if we can't move up/down, display a blank display insead of ▲/▼</param>
        /// <param name="moveMouse">move the mouse if we move items</param>
        /// <param name="amountMoveMouse">offset for the movement of the mouse</param>
        /// <param name="upRestriction">if 2, the element with index 0, 1, 2 can't go upper</param>
        /// <param name="downRestriction">if 2, the element at the end - 2 of the list can't go downer</param>
        /// <returns>true if we moved something</returns>
        public static bool DisplayMoveInList<T>(List<T> list, T itemToMove, bool displayBlankSpaceIfNothing = true, bool moveMouse = true, float amountMoveMouse = 23, int upRestriction = 0, int downRestriction = 0)
        {
            if (!list.Contains(itemToMove))
            {
                if (displayBlankSpaceIfNothing)
                {
                    ExtGUI.DisplayBlankSpace(2, 20);
                }
                return (false);
            }
            int index = ExtList.GetFirstIndexOfElementInList(list, itemToMove);
            if (index < 0 || index >= list.Count)
            {
                if (displayBlankSpaceIfNothing)
                {
                    ExtGUI.DisplayBlankSpace(2, 20);
                }
                return (false);
            }


            if (index > 0 + upRestriction)
            {
                if (GUILayout.Button("▲", GUILayout.Width(20)))
                {
                    list.Move(index, index - 1);
                    Win32Mouse.AddToMousePosition(new Vector2(0, -amountMoveMouse));
                    return (true);
                }
            }
            else
            {
                if (displayBlankSpaceIfNothing)
                {
                    ExtGUI.DisplayBlankSpace(1, 20);
                }
            }
            if (index < (list.Count - 1) + downRestriction)
            {
                if (GUILayout.Button("▼", GUILayout.Width(20)))
                {
                    list.Move(index, index + 1);
                    Win32Mouse.AddToMousePosition(new Vector2(0, amountMoveMouse));
                    return (true);
                }
            }
            else
            {
                if (displayBlankSpaceIfNothing)
                {
                    ExtGUI.DisplayBlankSpace(1, 20);
                }
            }
            return (false);
        }

        //end class
    }
    //end nameSpace
}