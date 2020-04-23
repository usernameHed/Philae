using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.eventEditor
{
    public static class ExtEventOnUpdate
    {
        public static Event LastKeyChange;

        [InitializeOnLoadMethod]
        private static void EditorInit()
        {
            System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)info.GetValue(null);

            value += EditorGlobalKeyPress;

            info.SetValue(null, value);
        }

        private static void EditorGlobalKeyPress()
        {
            LastKeyChange = Event.current;
            //Debug.Log("KEY CHANGE " + LastKeyChange);
        }
    }
}