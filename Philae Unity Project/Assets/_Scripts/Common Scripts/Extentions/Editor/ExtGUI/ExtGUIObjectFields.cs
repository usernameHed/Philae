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
    public static class ExtGUIObjectFields
    {

        public static T ObjectField<T>(
            T objectField,
            bool allowSceneObject = true,
            string text = "",
            params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            return (ObjectField<T>(objectField, null, text, out bool valueHasChanged, allowSceneObject, true, options));
        }

        public static T ObjectField<T>(
            T objectField,
            bool allowSceneObject,
            string text,
            out bool valueHasChanged,
            params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            return (ObjectField<T>(objectField, null, text, out valueHasChanged, allowSceneObject, true, options));
        }

        public static T ObjectField<T>(
            T objectField,
            UnityEngine.Object objToRecord,
            string text,
            out bool valueHasChanged,
            bool allowSceneObject = true,
            bool createHorizontalScope = true,
            params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");    //record changes
            }

            T oldValue = objectField;

            if (createHorizontalScope)
            {
                using (HorizontalScope horizontalScope = new HorizontalScope())
                {
                    if (!text.IsNullOrEmpty())
                    {
                        GUILayout.Label(text, options);
                    }
                    objectField = (T)EditorGUILayout.ObjectField(objectField, typeof(T), allowSceneObject);
                }
            }
            else
            {
                if (!text.IsNullOrEmpty())
                {
                    GUILayout.Label(text, options);
                }
                objectField = (T)EditorGUILayout.ObjectField(objectField, typeof(T), allowSceneObject);
            }

            valueHasChanged = oldValue != objectField;

            if (objToRecord != null && valueHasChanged)
            {
                EditorUtility.SetDirty(objToRecord);
            }
            return (objectField);
        }

        public static Transform TransformField(Transform valueToModify, UnityEngine.Object objToRecord, out bool valueHasChanged, bool allowSceneObject = true, params GUILayoutOption[] options)
        {
            Transform oldValue = valueToModify;

            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");
            }

            valueToModify = (Transform)EditorGUILayout.ObjectField(valueToModify, typeof(Transform), allowSceneObject);

            valueHasChanged = oldValue != valueToModify;
            if (valueHasChanged && objToRecord != null)
            {
                EditorUtility.SetDirty(objToRecord);
            }

            return (valueToModify);
        }
        //end class
    }
    //end nameSpace
}