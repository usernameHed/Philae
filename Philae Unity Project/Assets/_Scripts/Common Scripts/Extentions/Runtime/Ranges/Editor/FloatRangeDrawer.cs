﻿using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.runtime.range
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeDrawer : PropertyDrawer
    {
        private const int LABEL_LENGTH = 28;
        private float _currentMinValue;
        private float _currentMaxValue;
        private float _entireWidth;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            _entireWidth = position.width;
            //permit to remove all weird spaces when drawing a field next to another
            RemoveIndents();

            SerializedProperty propertyMinFloat = property.FindPropertyRelative(nameof(FloatRange.Min));
            SerializedProperty propertyMaxFloat = property.FindPropertyRelative(nameof(FloatRange.Max));

            _currentMinValue = propertyMinFloat.floatValue;
            _currentMaxValue = propertyMaxFloat.floatValue;

            DrawMinFloatField(ref position, propertyMinFloat);
            DrawMaxFloatField(ref position, propertyMaxFloat);

            EditorGUIUtility.labelWidth = 0f;
        }

        private void RemoveIndents()
        {
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 1f;
        }

        private void DrawMinFloatField(ref Rect position, SerializedProperty propertyMinFloat)
        {
            GUIContent minLabel = new GUIContent("min");
            DrawPropertyLabel(ref position, minLabel, propertyMinFloat);

            float spaceLeft = _entireWidth - 2 * LABEL_LENGTH;
            position.width = (spaceLeft / 2f);

            float newValue = EditorGUI.FloatField(position, propertyMinFloat.floatValue);
            if(newValue > _currentMaxValue)
            {
                newValue = _currentMaxValue;
            }
            propertyMinFloat.floatValue = newValue;

            position.x += spaceLeft / 2f;
        }

        private void DrawPropertyLabel(ref Rect position, GUIContent label, SerializedProperty associatedProperty)
        {
            position.width = LABEL_LENGTH;
            EditorGUI.BeginProperty(position, label, associatedProperty);
            EditorGUI.LabelField(position, label);
            EditorGUI.EndProperty();
            position.x += LABEL_LENGTH;
        }

        private void DrawMaxFloatField(ref Rect position, SerializedProperty propertyMaxFloat)
        {
            GUIContent maxLabel = new GUIContent("max");
            float newValue = EditorGUI.FloatField(position, propertyMaxFloat.floatValue);
            if (newValue < _currentMinValue)
            {
                newValue = _currentMinValue;
            }
            propertyMaxFloat.floatValue = newValue;

            position.x += position.width;
            DrawPropertyLabel(ref position, maxLabel, propertyMaxFloat);
        }
    }
}