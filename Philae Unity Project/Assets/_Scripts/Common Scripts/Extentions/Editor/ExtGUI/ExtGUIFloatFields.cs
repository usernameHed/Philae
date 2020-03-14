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
    public static class ExtGUIFloatFields
    {
        public static int IntField(float valueToModify,
            UnityEngine.Object objToRecord,
            out bool valueHasChanged,
            string label = "",
            string toolTipText = "",
            float min = -9999,
            float max = 9999,
            bool createHorizontalScope = true,
            bool delayedFloatField = false,
            float defaultWidth = 40,
            params GUILayoutOption[] options)
        {
            float floatValue = FloatField(valueToModify, objToRecord, out valueHasChanged, label, toolTipText, min, max, createHorizontalScope, delayedFloatField, defaultWidth, options);
            return ((int)Mathf.Round(floatValue));
        }

        public static float FloatField(float valueToModify, string text = "")
        {
            return (FloatField(valueToModify, null, out bool valueHasChanged, text));
        }

        public static float FloatField(float valueToModify, UnityEngine.Object objToRecord, string text = "")
        {
            return (FloatField(valueToModify, objToRecord, out bool valueHasChanged, text));
        }

        public static float FloatField(float valueToModify, UnityEngine.Object objToRecord, string text, out bool valueHasChanged)
        {
            return (FloatField(valueToModify, objToRecord, out valueHasChanged, text));
        }

        /// <summary>
        /// draw a float field with a slider to move
        /// must be called in OnSceneGUI
        /// </summary>
        /// <param name="valueToModify"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float FloatField(float valueToModify,
            UnityEngine.Object objToRecord,
            out bool valueHasChanged,
            string label = "",
            string toolTipText = "",
            float min = -9999,
            float max = 9999,
            bool createHorizontalScope = true,
            bool delayedFloatField = false,
            float defaultWidth = 40,
            params GUILayoutOption[] options)
        {
            float oldValue = valueToModify;

            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");
            }

            float newValue = valueToModify;

            if (createHorizontalScope)
            {
                using (HorizontalScope horizontalScopeSlider = new HorizontalScope(GUILayout.MaxWidth(50)))
                {
                    newValue = FloatFieldWithSlider(valueToModify, label, toolTipText, min, max, delayedFloatField, defaultWidth, options);
                }
            }
            else
            {
                newValue = FloatFieldWithSlider(valueToModify, label, toolTipText, min, max, delayedFloatField, defaultWidth, options);
            }

            valueHasChanged = oldValue != newValue;


            if (valueHasChanged && Event.current.control/* && Event.current.button == 0 && Event.current.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseDown*/)
            {
                newValue = ExtMathf.Round(newValue, 1);
            }
            /*
            else if (valueHasChanged && Event.current.shift)
            {
                Debug.Log(Event.current);

                Debug.Log("shift");
                float diff = (oldValue - newValue) / 100;
                newValue = oldValue - diff;
            }
            */
            else if (valueHasChanged && Event.current.alt)
            {
                float diff = (oldValue - newValue) * 100;
                newValue = oldValue - diff;
            }




            if (valueHasChanged && objToRecord != null)
            {
                EditorUtility.SetDirty(objToRecord);
            }

            return (newValue);
        }

        public static float FloatFieldWithSlider(float value, string label = "", string toolTipText = "", float min = -9999, float max = 9999, bool delayedFloatField = false, float defaultWidth = 40, params GUILayoutOption[] options)
        {
            if (!label.IsNullOrEmpty())
            {
                GUIContent gUIContent = new GUIContent(label, toolTipText);
                GUILayout.Label(gUIContent, options);
            }
            else
            {
                GUIContent gUIContent = new GUIContent("", toolTipText);
                GUILayout.Label(gUIContent);
            }

            Rect newRect = GUILayoutUtility.GetLastRect();
            Rect posRect = new Rect(newRect.x + newRect.width - 2, newRect.y, defaultWidth, 15);
            value = MyFloatFieldInternal(posRect, newRect, value, EditorStyles.numberField);
            value = ExtMathf.SetBetween(value, min, max);

            GUILayout.Label("", GUILayout.Width(30));

            return (value);
        }

        private static float MyFloatFieldInternal(Rect position, Rect dragHotZone, float value, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID("EditorTextField".GetHashCode(), FocusType.Keyboard, position);
            Type editorGUIType = typeof(EditorGUI);

            Type RecycledTextEditorType = Assembly.GetAssembly(editorGUIType).GetType("UnityEditor.EditorGUI+RecycledTextEditor");
            Type[] argumentTypes = new Type[] { RecycledTextEditorType, typeof(Rect), typeof(Rect), typeof(int), typeof(float), typeof(string), typeof(GUIStyle), typeof(bool) };
            MethodInfo doFloatFieldMethod = editorGUIType.GetMethod("DoFloatField", BindingFlags.NonPublic | BindingFlags.Static, null, argumentTypes, null);

            FieldInfo fieldInfo = editorGUIType.GetField("s_RecycledEditor", BindingFlags.NonPublic | BindingFlags.Static);
            object recycledEditor = fieldInfo.GetValue(null);

            object[] parameters = new object[] { recycledEditor, position, dragHotZone, controlID, value, "g7", style, true };

            return (float)doFloatFieldMethod.Invoke(null, parameters);
        }
        //end class
    }
    //end nameSpace
}