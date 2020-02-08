using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtGUIColor
    {
        public static Color ColorPicker(Color color, UnityEngine.Object objToRecord, string colorName, string toolTipText, out bool valueHasChanged, params GUILayoutOption[] options)
        {
            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");    //record changes
            }

            GUIContent toolTip = new GUIContent(colorName, toolTipText);

            Color newValue = EditorGUILayout.ColorField(toolTip, color, options);

            valueHasChanged = color != newValue;

            if (objToRecord != null && valueHasChanged)
            {
                EditorUtility.SetDirty(objToRecord);
            }
            return (newValue);
        }
        //end class
    }
    //end nameSpace
}