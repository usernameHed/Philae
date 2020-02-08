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
    public static class ExtGUIAnimationCurveFields
    {

        public static AnimationCurve AnimationCurveField(
           AnimationCurve animationCurve,
           string text = "",
           params GUILayoutOption[] options)
        {
            return (AnimationCurveField(animationCurve, null, text, true, out bool valueHasChanged, options));
        }

        public static AnimationCurve AnimationCurveField(
           AnimationCurve animationCurve,
           UnityEngine.Object objToRecord,
           string text = "",
           params GUILayoutOption[] options)
        {
            return (AnimationCurveField(animationCurve, objToRecord, text, true, out bool valueHasChanged, options));
        }



        public static AnimationCurve AnimationCurveField(
            AnimationCurve animationCurve,
            UnityEngine.Object objToRecord,
            string text,
            bool createHorizontalScope,
            out bool valueHasChanged,
            params GUILayoutOption[] options)
        {
            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");    //record changes
            }
            AnimationCurve oldValue = animationCurve;

            AnimationCurve newCurve;
            if (createHorizontalScope)
            {
                using (HorizontalScope horizontalScope = new HorizontalScope())
                {
                    if (!text.IsNullOrEmpty())
                    {
                        GUILayout.Label(text, options);
                    }
                    newCurve = EditorGUILayout.CurveField(animationCurve, options);
                }
            }
            else
            {
                if (!text.IsNullOrEmpty())
                {
                    GUILayout.Label(text, options);
                }
                newCurve = EditorGUILayout.CurveField(animationCurve, options);
            }

            valueHasChanged = oldValue != newCurve;

            if (objToRecord != null && valueHasChanged)
            {
                EditorUtility.SetDirty(objToRecord);
            }
            return (newCurve);
        }

        //end class
    }
    //end nameSpace
}