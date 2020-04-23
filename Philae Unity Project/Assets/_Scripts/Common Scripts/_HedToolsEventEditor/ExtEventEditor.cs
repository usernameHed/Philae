using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace hedCommon.eventEditor
{
    public static class ExtEventEditor
    {
        public static void Use()
        {
            if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
            {
                Event.current.Use();
            }
        }

        public static bool IsKeyUp(Event current, KeyCode keyCode)
        {
            if (current == null)
            {
                return (false);
            }

            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            EventType eventType = current.GetTypeForControl(controlId);
            bool pressed = current.keyCode == keyCode && eventType == EventType.KeyUp;
            return (pressed);
        }

        public static bool IsKeyDown(KeyCode keyCode)
        {
            if (Event.current == null)
            {
                return (false);
            }
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            EventType eventType = Event.current.GetTypeForControl(controlId);
            bool pressed = Event.current.keyCode == keyCode && eventType == EventType.KeyDown;
            return (Event.current.keyCode == keyCode);
        }
    }
}