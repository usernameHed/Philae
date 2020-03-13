using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace hedCommon.extension.runtime
{
    public static class ExtEventEditor
    {
        public enum Modifier
        {
            NONE = 0,
            CONTROL = 10,
            SHIFT = 20,
            ALT = 30,
        }

        public enum ButtonType
        {
            NONE = 0,
            LEFT = 10,
            RIGHT = 20,
            MIDDLE = 30,
        }

        public static void Use()
        {
            if (Event.current.type != EventType.Layout)
            {
                Event.current.Use();
            }
        }

        private static List<Modifier> SetupModifiers(Event current)
        {
            List<Modifier> modifiers = new List<Modifier>();
            if (current.control)
            {
                modifiers.Add(Modifier.CONTROL);
            }
            if (current.alt)
            {
                modifiers.Add(Modifier.ALT);
            }
            if (current.shift)
            {
                modifiers.Add(Modifier.SHIFT);
            }
            return (modifiers);
        }

        /// <summary>
        /// warning: current.button return 0 if nothing is pressed !! don't use alone
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private static ButtonType GetButtonType(Event current)
        {
            if (current.button == 0)
            {
                return (ButtonType.LEFT);
            }
            else if (current.button == 1)
            {
                return (ButtonType.RIGHT);
            }
            else if (current.button == 2)
            {
                return (ButtonType.MIDDLE);
            }
            return (ButtonType.NONE);
        }


        public static bool IsScrolling(Event current, out Vector2 delta)
        {
            delta = Vector2.zero;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            EventType eventType = current.GetTypeForControl(controlId);
            if (eventType == EventType.ScrollWheel)
            {
                delta = current.delta;
                return (true);
            }
            return (false);
        }

        public static bool IsScrollingUp(Event current, out float delta)
        {
            delta = 0;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            EventType eventType = current.GetTypeForControl(controlId);
            if (eventType == EventType.ScrollWheel)
            {
                delta = current.delta.y;
                return (delta > 0);
            }
            return (false);
        }

        public static bool IsScrollingDown(Event current, out float delta)
        {
            delta = 0;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            EventType eventType = current.GetTypeForControl(controlId);
            if (eventType == EventType.ScrollWheel)
            {
                delta = current.delta.y;
                return (delta < 0);
            }
            return (false);
        }

        public static bool IsLeftMouseUp(Event current)
        {
            bool clicked = IsMouseClicked(current, out EventType eventType, out List<Modifier> modifiers, out ButtonType buttonType);
            return (clicked && eventType == EventType.MouseUp && buttonType == ButtonType.LEFT);
        }

        public static bool IsLeftMouseDown(Event current)
        {
            bool clicked = IsMouseClicked(current, out EventType eventType, out List<Modifier> modifiers, out ButtonType buttonType);
            return (clicked && eventType == EventType.MouseDown && buttonType == ButtonType.LEFT);
        }

        public static bool IsRightMouseUp(Event current, bool returnTrueIfMarkedAsUsed = false)
        {
            bool clicked = IsMouseClicked(current, out EventType eventType, out List<Modifier> modifiers, out ButtonType buttonType);

            if (returnTrueIfMarkedAsUsed && eventType == EventType.Used && buttonType == ButtonType.RIGHT)
            {
                return (true);
            }

            return (clicked && eventType == EventType.MouseUp && buttonType == ButtonType.RIGHT);
        }
        public static bool IsRightMouseDown(Event current, bool returnTrueIfMarkedAsUsed = false)
        {
            bool clicked = IsMouseClicked(current, out EventType eventType, out List<Modifier> modifiers, out ButtonType buttonType);
            if (returnTrueIfMarkedAsUsed && eventType == EventType.Used && buttonType == ButtonType.RIGHT)
            {
                return (true);
            }

            return (clicked && eventType == EventType.MouseDown && buttonType == ButtonType.RIGHT);
        }

        public static bool IsMiddleMouseUp(Event current)
        {
            bool clicked = IsMouseClicked(current, out EventType eventType, out List<Modifier> modifiers, out ButtonType buttonType);
            return (clicked && eventType == EventType.MouseUp && buttonType == ButtonType.MIDDLE);
        }
        public static bool IsMiddleMouseDown(Event current)
        {
            bool clicked = IsMouseClicked(current, out EventType eventType, out List<Modifier> modifiers, out ButtonType buttonType);
            return (clicked && eventType == EventType.MouseDown && buttonType == ButtonType.MIDDLE);
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

        /// <summary>
        /// return true if the left mouse button is Down
        /// </summary>
        /// <param name="current">Event.current (only in a OnGUI() loop)</param>
        /// <returns>true if mouse is Down</returns>
        public static bool IsMouseClicked(Event current, out EventType eventType, out List<Modifier> modifiers, out ButtonType buttonType)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            modifiers = new List<Modifier>();
            eventType = current.GetTypeForControl(controlId);

            buttonType = GetButtonType(current);

            if (eventType == EventType.MouseUp)
            {
                eventType = EventType.MouseUp;
                modifiers = SetupModifiers(current);
                return (true);
            }

            if (eventType == EventType.MouseDown)
            {
                eventType = EventType.MouseDown;
                modifiers = SetupModifiers(current);
                return (true);
            }

            return (false);
        }

    }
}