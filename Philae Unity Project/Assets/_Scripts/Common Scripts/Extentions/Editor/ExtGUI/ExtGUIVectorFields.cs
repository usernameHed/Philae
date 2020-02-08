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
    public static class ExtGUIVectorFields
    {
        public static Vector2 Vector2Field(Vector2 valueToModify, UnityEngine.Object objToRecord, out bool valueHasChanged, string label = "", string toolTipText = "", bool createHorizontalScope = true, bool delayedVector2Field = false, string labelX = "X", string labelY = "Y", params GUILayoutOption[] options)
        {
            Vector2 oldValue = valueToModify;

            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");
            }

            if (createHorizontalScope)
            {
                using (HorizontalScope horizontalScope = new HorizontalScope())
                {
                    if (!label.IsNullOrEmpty())
                    {
                        GUIContent gUIContent = new GUIContent(label, toolTipText);
                        GUILayout.Label(gUIContent, options);
                    }
                    valueToModify.x = ExtGUIFloatFields.FloatField(valueToModify.x, null, out bool valueChanged, labelX + ":", "", -9999, 9999, false, delayedVector2Field);
                    valueToModify.y = ExtGUIFloatFields.FloatField(valueToModify.y, null, out valueChanged, labelY + ":", "", -9999, 9999, false, delayedVector2Field);
                }
            }
            else
            {
                if (!label.IsNullOrEmpty())
                {
                    GUILayout.Label(label, options);
                }
                valueToModify.x = ExtGUIFloatFields.FloatField(valueToModify.x, null, out bool valueChanged, labelX + ":", "", -9999, 9999, false, delayedVector2Field);
                valueToModify.y = ExtGUIFloatFields.FloatField(valueToModify.y, null, out valueChanged, labelY + ":", "", -9999, 9999, false, delayedVector2Field);
            }

            valueHasChanged = oldValue != valueToModify;
            if (valueHasChanged && objToRecord != null)
            {
                EditorUtility.SetDirty(objToRecord);
            }
            return (valueToModify);
        }

        public static Vector3 Vector3Field(Vector3 valueToModify)
        {
            return (Vector3Field(valueToModify, null, out bool valueHasChanged, ""));
        }

        public static Vector3 Vector3Field(Vector3 valueToModify, UnityEngine.Object objToRecord)
        {
            return (Vector3Field(valueToModify, objToRecord, out bool valueHasChanged, ""));
        }

        public static Vector3 Vector3Field(Vector3 valueToModify, UnityEngine.Object objToRecord, out bool valueHasChanged, string label = "", bool createHorizontalScope = true, bool delayedVector3Field = false, params GUILayoutOption[] options)
        {
            return (Vector3Field(valueToModify, objToRecord, new Vector3(-9999, -9999, -9999), new Vector3(9999, 9999, 9999), out valueHasChanged, label, createHorizontalScope, delayedVector3Field, options));
        }

        public static Vector3 Vector3Field(Vector3 valueToModify, UnityEngine.Object objToRecord, Vector3 min, Vector3 max, out bool valueHasChanged, string label = "", bool createHorizontalScope = true, bool delayedVector3Field = false, params GUILayoutOption[] options)
        {
            Vector3 oldValue = valueToModify;
            float x = valueToModify.x;
            float y = valueToModify.y;
            float z = valueToModify.z;
            bool valueChanged = false;

            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");
            }

            if (createHorizontalScope)
            {
                using (HorizontalScope horizontalScope = new HorizontalScope())
                {
                    valueChanged = ChangeXYZValues(min, max, label, delayedVector3Field, options, ref x, ref y, ref z);
                }
            }
            else
            {
                valueChanged = ChangeXYZValues(min, max, label, delayedVector3Field, options, ref x, ref y, ref z);
            }

            valueToModify = new Vector3(x, y, z);

            valueHasChanged = oldValue != valueToModify;
            if (valueHasChanged && objToRecord != null)
            {
                EditorUtility.SetDirty(objToRecord);
            }
            return (valueToModify);
        }

        private static bool ChangeXYZValues(Vector3 min, Vector3 max, string label, bool delayedVector3Field, GUILayoutOption[] options, ref float x, ref float y, ref float z)
        {
            bool valueChanged;
            if (!label.IsNullOrEmpty())
            {
                GUILayout.Label(label, options);
            }
            x = ExtGUIFloatFields.FloatField(x, null, out valueChanged, "X:", "", min.x, max.x, false, delayedVector3Field);
            y = ExtGUIFloatFields.FloatField(y, null, out valueChanged, "Y:", "", min.y, max.y, false, delayedVector3Field);
            z = ExtGUIFloatFields.FloatField(z, null, out valueChanged, "Z:", "", min.z, max.z, false, delayedVector3Field);
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(15)))
            {
                x = 0;
                y = 0;
                z = 0;
            }
            return valueChanged;
        }


        //end class
    }
    //end nameSpace
}