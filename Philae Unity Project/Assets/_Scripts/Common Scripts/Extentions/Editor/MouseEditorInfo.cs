using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public class MouseEditorInfo
    {
        private Vector2 _mousePosition = new Vector2();
        private float deltaX;
        private float deltaY;

        public bool IsGoingRight()
        {
            return (deltaX < 0);
        }
        public bool IsGoingLeft()
        {
            return (deltaX > 0);
        }

        public void CustomUpdate()
        {
            Vector2 newPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            if (newPosition.x != _mousePosition.x)
            {
                deltaX = newPosition.x - _mousePosition.x;
            }
            if (newPosition.y != _mousePosition.y)
            {
                deltaY = newPosition.y - _mousePosition.y;
            }
            _mousePosition = newPosition;
        }
    }
}