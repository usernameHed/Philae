using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtGUIToggles
    {
        private static Texture _texture;

        /// <summary>
        /// Displays a vertical list of toggles and returns the index of the selected item.
        /// </summary>
        public static int ToggleList(int selected, GUIContent[] items)
        {
            // Keep the selected index within the bounds of the items array
            selected = selected < 0 ? 0 : selected >= items.Length ? items.Length - 1 : selected;

            //GUILayout.BeginVertical();
            for (int i = 0; i < items.Length; i++)
            {
                // Display toggle. Get if toggle changed.
                bool change = GUILayout.Toggle(selected == i, items[i].text);
                // If changed, set selected to current index.
                if (change)
                    selected = i;
            }
            //GUILayout.EndVertical();

            // Return the currently selected item's index
            return selected;
        }

        public static bool ToggleImage(bool valueToModify, string imageName, out bool valueHasChanged, GUIStyle style, params GUILayoutOption[] options)
        {
            _texture = (Texture)EditorGUIUtility.Load("SceneView/" + imageName + ".png");
            bool toggle = GUILayout.Toggle(valueToModify, _texture, style, options);
            valueHasChanged = toggle != valueToModify;
            return (toggle);
        }

        public static bool ToggleImage(bool valueToModify, string imageName, string imageNameFalse, out bool valueHasChanged, string toolTipText, GUIStyle style = null, params GUILayoutOption[] options)
        {
            return (ToggleImage(valueToModify, imageName, imageNameFalse, Color.white, Color.white, out valueHasChanged, toolTipText, style, options));
        }

        public static bool ToggleImage(bool valueToModify, UnityEngine.Object objToRecord, string imageName, string imageNameFalse, out bool valueHasChanged, string toolTipText, GUIStyle style = null, params GUILayoutOption[] options)
        {
            return (ToggleImage(valueToModify, objToRecord, imageName, imageNameFalse, Color.white, Color.white, out valueHasChanged, KeyCode.None, toolTipText, style, options));
        }

        public static bool ToggleImage(bool valueToModify, string imageName, string imageNameFalse, Color colorTrue, Color colorFalse, out bool valueHasChanged, string toolTipText, GUIStyle style = null, params GUILayoutOption[] options)
        {
            return (ToggleImage(valueToModify, null, imageName, imageNameFalse, colorTrue, colorFalse, out valueHasChanged, KeyCode.None, toolTipText, style, options));
        }

        public static bool ToggleImage(bool valueToModify, UnityEngine.Object objToRecord, string imageName, string imageNameFalse, Color colorTrue, Color colorFalse, out bool valueHasChanged, KeyCode keyCode = KeyCode.None, string toolTipText = "", GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");    //record changes
            }

            Color previous = GUI.backgroundColor;
            if (valueToModify)
            {
                _texture = (Texture)EditorGUIUtility.Load("SceneView/" + imageName + ".png");
                GUI.backgroundColor = colorTrue;
            }
            else
            {
                _texture = (Texture)EditorGUIUtility.Load("SceneView/" + imageNameFalse + ".png");
                GUI.backgroundColor = colorFalse;
            }
            bool toggle = false;
            if (style != null)
            {
                GUIContent toolTip = new GUIContent(_texture, toolTipText);
                toggle = GUILayout.Toggle(valueToModify, toolTip, style, options);
            }
            else
            {
                GUIContent toolTip = new GUIContent(_texture, toolTipText);
                toggle = GUILayout.Toggle(valueToModify, toolTip, options);
            }

            if (keyCode != KeyCode.None && ExtEventEditor.IsKeyUp(Event.current, keyCode))
            {
                toggle = !toggle;
            }

            valueHasChanged = toggle != valueToModify;
            GUI.backgroundColor = previous;

            if (objToRecord != null && valueHasChanged)
            {
                EditorUtility.SetDirty(objToRecord);
            }

            return (toggle);
        }
        public static bool Toggle(bool valueToModify, string text = "", params GUILayoutOption[] options)
        {
            return (ExtGUIToggles.Toggle(valueToModify, null, text, out bool valueHasChanged, null, options));
        }

        public static bool Toggle(bool valueToModify, string textOn = "", string textOff = "", GUIStyle gUIStyle = null, params GUILayoutOption[] options)
        {
            return (ExtGUIToggles.Toggle(valueToModify, null, textOn, textOff, out bool valueHasChanged, gUIStyle, options));
        }

        public static bool Toggle(bool valueToModify, string text = "", GUIStyle gUIStyle = null, params GUILayoutOption[] options)
        {
            return (ExtGUIToggles.Toggle(valueToModify, null, text, text, out bool valueHasChanged, gUIStyle, options));
        }

        public static bool Toggle(bool valueToModify, UnityEngine.Object objToRecord, string text = "", params GUILayoutOption[] options)
        {
            return (ExtGUIToggles.Toggle(valueToModify, objToRecord, text, text, out bool valueHasChanged, null, options));
        }

        public static bool Toggle(bool valueToModify, UnityEngine.Object objToRecord, string textOn = "", string textOff = "", params GUILayoutOption[] options)
        {
            return (ExtGUIToggles.Toggle(valueToModify, objToRecord, textOn, textOff, out bool valueHasChanged, null, options));
        }

        public static bool Toggle(bool valueToModify, UnityEngine.Object objToRecord, string text, out bool valueHasChanged, GUIStyle gUIStyle = null, params GUILayoutOption[] options)
        {
            return (Toggle(valueToModify, objToRecord, text, text, out valueHasChanged, gUIStyle, options));
        }

        public static bool Toggle(bool valueToModify, UnityEngine.Object objToRecord, string textOn, string textOff, out bool valueHasChanged, GUIStyle gUIStyle = null, params GUILayoutOption[] options)
        {
            return (Toggle(valueToModify, objToRecord, textOn, textOff, out valueHasChanged, "", gUIStyle, options));
        }

        public static bool Toggle(bool valueToModify, UnityEngine.Object objToRecord, string textOn, string textOff, out bool valueHasChanged, string toolTipText, GUIStyle gUIStyle = null, params GUILayoutOption[] options)
        {
            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");    //record changes
            }

            bool oldValue = valueToModify;
            bool newValue;
            string text = valueToModify ? textOn : textOff;

            GUIContent toolTip = new GUIContent(text, toolTipText);

            if (gUIStyle != null)
            {
                newValue = GUILayout.Toggle(valueToModify, toolTip, gUIStyle, options);                                 //apply toggle
            }
            else
            {
                newValue = GUILayout.Toggle(valueToModify, toolTip, options);                                 //apply toggle
            }

            valueHasChanged = oldValue != newValue;                                                         //say if values has changed

            if (objToRecord != null && valueHasChanged)
            {
                EditorUtility.SetDirty(objToRecord);
            }
            return (newValue);
        }

        /// <summary>
        /// icon name exemple:
        /// Animation.Play
        /// Animation.Record
        /// Preview
        /// Animation.PrevKey,
        /// Animation.NextKey,
        /// Animation.FirstKey,
        /// Animation.LastKey,
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool ToggleIcon(bool valueToModify, Color on, Color off, out bool valueHasChanged, string iconName, string toolTip, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        {
            GUIContent playContent = EditorGUIUtility.TrIconContent(iconName, toolTip);

            return (ToggleIcon(valueToModify, null, on, off, out valueHasChanged, playContent, guiStyle, options));
        }

        public static bool ToggleIcon(bool valueToModify, UnityEngine.Object objToRecord, Color on, Color off, out bool valueHasChanged, string iconName, string toolTip, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        {
            GUIContent playContent = EditorGUIUtility.TrIconContent(iconName, toolTip);

            return (ToggleIcon(valueToModify, objToRecord, on, off, out valueHasChanged, playContent, guiStyle, options));
        }


        public static bool ToggleIcon(bool valueToModify, UnityEngine.Object objToRecord, Color on, Color off, out bool valueHasChanged, GUIContent content, GUIStyle gUIStyle = null, params GUILayoutOption[] options)
        {
            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");    //record changes
            }
            Color oldColor = GUI.backgroundColor;

            bool oldValue = valueToModify;
            bool newValue;

            GUI.backgroundColor = (valueToModify) ? on : off;

            newValue = GUILayout.Toggle(valueToModify, content, gUIStyle, options);                                 //apply toggle


            valueHasChanged = oldValue != newValue;                                                         //say if values has changed

            if (objToRecord != null && valueHasChanged)
            {
                EditorUtility.SetDirty(objToRecord);
            }
            GUI.backgroundColor = oldColor;
            return (newValue);
        }
        //end class
    }
    //end nameSpace
}