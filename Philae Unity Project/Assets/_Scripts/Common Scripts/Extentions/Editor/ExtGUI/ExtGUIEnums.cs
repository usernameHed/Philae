using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.editor
{
    public static class ExtGUIEnums
    {

        public static T EnumPopup<T>(T enumReference, params T[] redsValues) where T : Enum
        {
            return (EnumPopup(enumReference, redsValues.ToList()));
        }

        public static T EnumPopup<T>(T enumReference, List<T> redValues, params GUILayoutOption[] options) where T : Enum
        {
            if (redValues.Contains(enumReference))
            {
                GUI.backgroundColor = Color.red;
            }
            T item = EnumPopup(enumReference, null, out bool valueHasChanged, options);
            GUI.backgroundColor = Color.white;
            return (item);
        }

        public static T EnumPopup<T>(T enumReference, T redValue, params GUILayoutOption[] options) where T : Enum
        {
            return (EnumPopup(enumReference, new List<T>() { redValue }, options));
        }

        public static T EnumPopup<T>(T enumReference, UnityEngine.Object objToRecord, out bool valueHasChanged, params GUILayoutOption[] options) where T : Enum
        {
            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");    //record changes
            }

            T oldValue = enumReference;
            enumReference = (T)EditorGUILayout.EnumPopup(enumReference, options);
            valueHasChanged = !EqualityComparer<T>.Default.Equals(oldValue, enumReference);

            if (objToRecord != null && valueHasChanged)
            {
                EditorUtility.SetDirty(objToRecord);
            }

            return (enumReference);
        }

        public static int EnumMaskOnTheFly(int mask, string[] list)
        {
            int maskKeys = ExtGUIMasks.MaskField(mask, list, out bool valueHasChanged);
            return (maskKeys);
        }

        public static string[] CreateListIndex(int numberOfItems, string before = "", string after = "")
        {
            string[] listToShow = new string[numberOfItems];
            for (int i = 0; i < listToShow.Length; i++)
            {
                listToShow[i] = before + i.ToString() + after;
            }
            return (listToShow);
        }

        //end class
    }
    //end nameSpace
}