using hedCommon.extension.editor.screenCapture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtMouse
    {
        private static bool _wasMouseDown;
        private static Vector2 _clickDownPosition;

        /// <summary>
        /// Get pixel color under mouse position
        /// Usage: Color color = ExtMouse.GetColorUnderMousePosition();
        /// </summary>
        /// <returns>Pixel color under mouse position</returns>
        public static Color GetColorUnderMousePosition()
        {
            Vector2 realMousePosition = ExtMouse.GetRealMousePosition();
            return (ExtScreenCapture.PickColorAtPosition(realMousePosition));
        }

        public static Vector2 GetRealMousePosition()
        {
            return (GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
        }

        #region mouse event editor
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

        /// <summary>
        /// DOESN'T WORK IN ONE SHOOT, need to be called at least twice,
        /// once on mouseDown, and once on mouseUp
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static bool IsClickOnSceneView(Event current)
        {
            if (ExtMouse.IsLeftMouseDown(current))
            {
                _clickDownPosition = current.mousePosition;
                _wasMouseDown = true;
                return (false);
            }
            if (!_wasMouseDown)
            {
                return (false);
            }
            if (current.type == EventType.Used && current.mousePosition == _clickDownPosition && current.delta == Vector2.zero)
            {
                _wasMouseDown = false;
                return (true);
            }
            return (false);
        }

        #endregion
    }
}