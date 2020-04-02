using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace hedCommon.extension.editor
{
    public static class ExtGUISliders
    {
        public delegate void MethodToCallInsideSliderScrub();

        private static float _saveDeltaMiddle = 0;

        public static float HorizontalSlider(float value, float min, float max, out bool valueHasChanged, bool displayMinMax, params GUILayoutOption[] options)
        {
            float newValue = GUILayout.HorizontalSlider(value, min, max, options);
            valueHasChanged = newValue != value;

            if (displayMinMax)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();

                float percentBefore = ExtMathf.Remap(min, min, max, 0, 1);
                float percentAfter = ExtMathf.Remap(max, min, max, 0, 1);

                DrawInsideSliderValue(min, lastRect, percentBefore, 50, 20, 0, 0);
                DrawInsideSliderValue(max, lastRect, percentAfter, 50, 20, 0, 0);
            }

            return (newValue);
        }

        public static int HorizontalSlider(int value, int min, int max, out bool valueHasChanged, bool displayMinMax, params GUILayoutOption[] options)
        {
            int newValue = (int)GUILayout.HorizontalSlider((float)value, (float)min, (float)max, options);
            valueHasChanged = newValue != value;

            if (displayMinMax)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();

                float percentBefore = ExtMathf.Remap(min, min, max, 0, 1);
                float percentAfter = ExtMathf.Remap(max, min, max, 0, 1);

                DrawInsideSliderValue(min, lastRect, percentBefore, 50, 20, 0, 0);
                DrawInsideSliderValue(max, lastRect, percentAfter, 50, 20, 0, 0);
            }

            return (newValue);
        }

        public static float HorizontalSlider(float value, float min, float max, out bool valueHasChanged, MethodToCallInsideSliderScrub methodToCall, float width, float height, float xOffset = 0, float yOffset = 0, bool displayMinMax = false/*, bool loopMouse = false, EditorWindow current = null*/, params GUILayoutOption[] options)
        {
            float newValue = GUILayout.HorizontalSlider(value, min, max, options);

            float percent = ExtMathf.Remap(newValue, min, max, 0, 1);

            Rect lastRect = GUILayoutUtility.GetLastRect();

            DrawInsideSliderValue(methodToCall, lastRect, percent, width, height, xOffset, yOffset);
            valueHasChanged = newValue != value;

            if (displayMinMax)
            {
                float percentBefore = ExtMathf.Remap(min, min, max, 0, 1);
                float percentAfter = ExtMathf.Remap(max, min, max, 0, 1);

                DrawInsideSliderValue(min, lastRect, percentBefore, 50, 20, 10, 0);
                DrawInsideSliderValue(max, lastRect, percentAfter, 50, 20, 10, 0);
            }

            return (newValue);
        }

        private static void DrawInsideSliderValue(MethodToCallInsideSliderScrub methodToCall, Rect lastRect, float percent, float width, float height, float addXOffset, float addYOffset)
        {
            float yOffset = 10 + height / 2;


            lastRect.x -= width / 2;
            lastRect.y -= yOffset;

            lastRect.x += addXOffset;
            lastRect.y += addYOffset;

            float maxWidth = lastRect.width;
            float currentOffset = maxWidth / 1 * percent;

            lastRect.x += currentOffset;
            lastRect.height = height;

            GUI.BeginGroup(lastRect);
            GUILayout.BeginArea(new Rect(0, 0, width, height));

            methodToCall.Invoke();

            GUILayout.EndArea();
            GUI.EndGroup();
        }

        private static void DrawInsideSliderValue(float valueToDisplay, Rect lastRect, float percent, float width, float height, float addXOffset, float addYOffset)
        {
            float yOffset = 10 + height / 2;


            lastRect.x -= width / 2;
            lastRect.y -= yOffset;

            lastRect.x += addXOffset;
            lastRect.y += addYOffset;

            float maxWidth = lastRect.width;
            float currentOffset = maxWidth / 1 * percent;

            lastRect.x += currentOffset;
            lastRect.height = height;

            GUI.BeginGroup(lastRect);
            GUILayout.BeginArea(new Rect(0, 0, width, height));

            ExtGUILabel.Label(valueToDisplay.ToString());

            GUILayout.EndArea();
            GUI.EndGroup();
        }



        public static void HorizontalMinMaxSlider(ref float minValue, ref float maxValue, float min, float max, float minBetweenValues, out bool valuesHasChanged)
        {
            valuesHasChanged = false;

            float previousMin = minValue;
            float previousMax = maxValue;
            EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, min, max);

            if (maxValue - minValue < minBetweenValues)
            {
                if (minValue + minBetweenValues < max)
                {
                    maxValue = minValue + minBetweenValues;
                }
                else
                {
                    minValue = minValue - minBetweenValues;
                }
            }

            if (previousMax != minValue || previousMax != maxValue)
            {
                valuesHasChanged = true;
            }
        }

        public static void HorizontalMinMaxSlider(ref float minValue, ref float maxValue, float min, float max, float minBetweenValues, out bool valuesHasChanged, MethodToCallInsideSliderScrub startFunction, MethodToCallInsideSliderScrub endFunction, float width, float height, float xOffset, float yoffset)
        {
            HorizontalMinMaxSlider(ref minValue, ref maxValue, min, max, minBetweenValues, out valuesHasChanged);

            float percentBefore = ExtMathf.Remap(minValue, min, max, 0, 1);
            float percentAfter = ExtMathf.Remap(maxValue, min, max, 0, 1);

            Rect lastRect = GUILayoutUtility.GetLastRect();

            DrawInsideSliderValue(startFunction, lastRect, percentBefore, width, height, xOffset, yoffset);
            DrawInsideSliderValue(endFunction, lastRect, percentAfter, width, height, xOffset, yoffset);
        }



        public static int HorizontalSlider(int value, int min, int max, out bool valueHasChanged)
        {
            float newValue = GUILayout.HorizontalSlider((float)value, (float)min, (float)max);
            int newValueInt = (int)newValue;
            valueHasChanged = newValue != value;
            return (newValueInt);
        }

        public static float SquaredSlider(
            float currentValue,
            float min,
            float max,
            out bool valueHasChanged,
            Vector2 horizontalOffset,
            Rect rectOfScope,
            Color colorBackGround,
            Color colorSlider,
            EditorWindow currentEditorWindow,
            ref bool isDragging,
            float height,
            params GUILayoutOption[] options)
        {
            valueHasChanged = false;

            //currentPositionInSlider = 0 - 100;
            //float widthSlider = rectOfScope.width

            Rect layoutRectangle = GUILayoutUtility.GetRect(0, rectOfScope.width, height, height);

            float zeroAbsolute = layoutRectangle.x + horizontalOffset.x;
            float maxAbsolute = layoutRectangle.x + layoutRectangle.width - horizontalOffset.x - horizontalOffset.y;

            Rect layoutAbsoluteRectangle = new Rect(zeroAbsolute, layoutRectangle.y, maxAbsolute - zeroAbsolute, layoutRectangle.height);
            GUI.DrawTexture(layoutAbsoluteRectangle, ExtTexture.GetTexture(colorBackGround));


            float currentAbsolutePositionInSlider = ExtMathf.Remap(currentValue, min, max, zeroAbsolute, maxAbsolute);

            Rect positionSlider = new Rect(currentAbsolutePositionInSlider, rectOfScope.y, 1, layoutRectangle.height);
            GUI.DrawTexture(positionSlider, ExtTexture.GetTexture(colorSlider));

            if (!isDragging && Event.current.type == EventType.MouseDown && Event.current.button == 0 && rectOfScope.Contains(Event.current.mousePosition))
            {
                isDragging = true;
                //Event.current.Use();
            }
            if (isDragging && Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown))
            {
                float newWidth = ExtMathf.SetBetween(Event.current.mousePosition.x, zeroAbsolute, maxAbsolute);
                if (newWidth != currentAbsolutePositionInSlider)
                {
                    currentValue = ExtMathf.Remap(newWidth, zeroAbsolute, maxAbsolute, min, max);
                    valueHasChanged = true;
                    currentEditorWindow.Repaint();
                }
                ExtEventEditor.Use();
            }
            if (isDragging && Event.current.type == EventType.MouseUp)
            {
                isDragging = false;
            }

            return (currentValue);
        }

        public static void SquaredSliderMonMax(
           ref float currentLeftValue,
           ref float currentRightValue,
           float minBetweenBothValues,
           float min,
           float max,
           out bool valueHasChanged,
           Vector2 horizontalOffset,
           Rect rectOfScope,
           Color colorBackGround,
           Color colorSlider,
           EditorWindow currentEditorWindow,
           ref bool isDraggingLeft,
           ref bool isDraggingMiddle,
           ref bool isDraggingRight,
           float height,
           params GUILayoutOption[] options)
        {
            valueHasChanged = false;

            //currentPositionInSlider = 0 - 100;
            //float widthSlider = rectOfScope.width

            Rect layoutRectangle = GUILayoutUtility.GetRect(0, rectOfScope.width, height, height);

            float zeroAbsolute = layoutRectangle.x + horizontalOffset.x;
            float maxAbsolute = layoutRectangle.x + layoutRectangle.width - horizontalOffset.x - horizontalOffset.y;

            float currentAbsolutePositionInSliderLeft = ExtMathf.Remap(currentLeftValue, min, max, zeroAbsolute, maxAbsolute);
            float currentAbsolutePositionInSliderRight = ExtMathf.Remap(currentRightValue, min, max, zeroAbsolute, maxAbsolute);

            Rect layoutAbsoluteRectangle = new Rect(currentAbsolutePositionInSliderLeft, layoutRectangle.y, currentAbsolutePositionInSliderRight - currentAbsolutePositionInSliderLeft, layoutRectangle.height);
            GUI.DrawTexture(layoutAbsoluteRectangle, ExtTexture.GetTexture(colorBackGround));

            Rect positionSliderLeft = new Rect(currentAbsolutePositionInSliderLeft, rectOfScope.y, 1, layoutRectangle.height);
            GUI.DrawTexture(positionSliderLeft, ExtTexture.GetTexture(colorSlider));

            Rect positionSliderRight = new Rect(currentAbsolutePositionInSliderRight, rectOfScope.y, 1, layoutRectangle.height);
            GUI.DrawTexture(positionSliderRight, ExtTexture.GetTexture(colorSlider));

            float sizeRect = 5f;

            Rect dragZoneLeft = new Rect(positionSliderLeft.x - sizeRect, positionSliderLeft.y, sizeRect * 2, positionSliderLeft.height);
            Rect dragZoneMiddle = new Rect(layoutAbsoluteRectangle.x + sizeRect, layoutAbsoluteRectangle.y, layoutAbsoluteRectangle.width - sizeRect * 2, layoutAbsoluteRectangle.height);
            Rect dragZoneRight = new Rect(positionSliderRight.x - sizeRect, positionSliderRight.y, sizeRect * 2, positionSliderRight.height);

            if (!isDraggingMiddle && !isDraggingRight)
            {
                if (isDraggingLeft)
                {
                    EditorGUIUtility.AddCursorRect(new Rect(0, 0, 100000, 100000), MouseCursor.ResizeHorizontal);
                }
                else
                {
                    EditorGUIUtility.AddCursorRect(dragZoneLeft, MouseCursor.ResizeHorizontal);
                }
            }
            if (!isDraggingLeft && !isDraggingRight)
            {
                if (isDraggingMiddle)
                {
                    EditorGUIUtility.AddCursorRect(new Rect(0, 0, 100000, 100000), MouseCursor.MoveArrow);
                }
                else
                {
                    EditorGUIUtility.AddCursorRect(dragZoneMiddle, MouseCursor.MoveArrow);
                }
            }
            if (!isDraggingLeft && !isDraggingMiddle)
            {
                if (isDraggingRight)
                {
                    EditorGUIUtility.AddCursorRect(new Rect(0, 0, 100000, 100000), MouseCursor.ResizeHorizontal);
                }
                else
                {
                    EditorGUIUtility.AddCursorRect(dragZoneRight, MouseCursor.ResizeHorizontal);
                }
            }

            bool isNotDragging = (!isDraggingLeft && !isDraggingMiddle && !isDraggingRight);

            if (isNotDragging && Event.current.button == 0 && Event.current.type == EventType.MouseDown/* && rectOfScope.Contains(Event.current.mousePosition)*/)
            {
                if (dragZoneLeft.Contains(Event.current.mousePosition))
                {
                    isDraggingLeft = true;
                    isDraggingMiddle = isDraggingRight = false;
                }
                else if (dragZoneMiddle.Contains(Event.current.mousePosition))
                {
                    isDraggingMiddle = true;
                    _saveDeltaMiddle = currentAbsolutePositionInSliderLeft - Event.current.mousePosition.x;
                    isDraggingLeft = isDraggingRight = false;
                }
                else if (dragZoneRight.Contains(Event.current.mousePosition))
                {
                    isDraggingRight = true;
                    isDraggingMiddle = isDraggingLeft = false;
                }
                else if (layoutRectangle.Contains(Event.current.mousePosition))
                {
                    isDraggingMiddle = true;
                    _saveDeltaMiddle = currentAbsolutePositionInSliderLeft - Event.current.mousePosition.x;
                    isDraggingLeft = isDraggingRight = false;
                }
            }

            bool isDragging = (isDraggingLeft || isDraggingMiddle || isDraggingRight);

            if (isDragging && Event.current.button == 0 && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown))
            {
                if (isDraggingLeft)
                {
                    float newLeft = ExtMathf.SetBetween(Event.current.mousePosition.x, zeroAbsolute, maxAbsolute);
                    if (newLeft != currentAbsolutePositionInSliderLeft)
                    {
                        newLeft = ExtMathf.Remap(newLeft, zeroAbsolute, maxAbsolute, min, max);
                        if (newLeft > currentRightValue - minBetweenBothValues)
                        {
                            newLeft = currentRightValue - minBetweenBothValues;
                        }
                        currentLeftValue = newLeft;

                        valueHasChanged = true;
                        currentEditorWindow.Repaint();
                    }
                }

                if (isDraggingMiddle)
                {
                    float newMiddle = ExtMathf.SetBetween(Event.current.mousePosition.x, zeroAbsolute, maxAbsolute);
                    if (newMiddle != currentAbsolutePositionInSliderLeft - _saveDeltaMiddle)
                    {
                        newMiddle = ExtMathf.Remap(newMiddle + _saveDeltaMiddle, zeroAbsolute, maxAbsolute, min, max);
                        float diffBetween2 = currentRightValue - currentLeftValue;

                        if (newMiddle < min)
                        {
                            newMiddle = min;
                        }
                        if (newMiddle > max - diffBetween2)
                        {
                            newMiddle = max - diffBetween2;
                        }
                        currentLeftValue = newMiddle;
                        currentRightValue = currentLeftValue + diffBetween2;

                        valueHasChanged = true;
                        currentEditorWindow.Repaint();
                        
                    }
                }

                if (isDraggingRight)
                {
                    float newRight = ExtMathf.SetBetween(Event.current.mousePosition.x, zeroAbsolute, maxAbsolute);
                    if (newRight != currentAbsolutePositionInSliderRight)
                    {
                        newRight = ExtMathf.Remap(newRight, zeroAbsolute, maxAbsolute, min, max);
                        if (newRight < currentLeftValue + minBetweenBothValues)
                        {
                            newRight = currentLeftValue + minBetweenBothValues;
                        }
                        currentRightValue = newRight;

                        valueHasChanged = true;
                        currentEditorWindow.Repaint();
                    }
                }

                ExtEventEditor.Use();
            }
            if ((isDraggingLeft || isDraggingMiddle || isDraggingRight) && Event.current.type == EventType.MouseUp)
            {
                isDraggingLeft = isDraggingMiddle = isDraggingRight = false;
            }
        }

        //end class
    }

    //end nameSpace
}