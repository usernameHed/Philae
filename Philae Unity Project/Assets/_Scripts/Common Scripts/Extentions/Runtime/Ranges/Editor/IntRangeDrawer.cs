using hedCommon.extension.runtime.range;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.runtime.range
{
    [CustomPropertyDrawer(typeof(IntRange))]
    public class IntRangeDrawer : PropertyDrawer
    {
        private const int LABEL_LENGTH = 35;
        private int _currentMinValue;
        private int _currentMaxValue;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            //permit to remove all weird spaces when drawing a field next to another
            RemoveIndents();

            SerializedProperty propertyMinInt = property.FindPropertyRelative(nameof(IntRange.Min));
            SerializedProperty propertyMaxInt = property.FindPropertyRelative(nameof(IntRange.Max));

            _currentMinValue = propertyMinInt.intValue;
            _currentMaxValue = propertyMaxInt.intValue;

            DrawMinIntField(ref position, propertyMinInt);
            DrawMaxIntField(ref position, propertyMaxInt);

            EditorGUIUtility.labelWidth = 0f;
        }

        private void RemoveIndents()
        {
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 1f;
        }

        private void DrawMinIntField(ref Rect position, SerializedProperty propertyMinInt)
        {
            GUIContent minLabel = new GUIContent("min");
            DrawPropertyLabel(ref position, minLabel, propertyMinInt);

            float totalWidth = EditorGUIUtility.currentViewWidth - position.x - 5;
            float spaceLeft = totalWidth - 2 * LABEL_LENGTH;
            position.width = (spaceLeft / 2f);

            int newValue = EditorGUI.IntField(position, propertyMinInt.intValue);
            if(newValue > _currentMaxValue)
            {
                newValue = _currentMaxValue;
            }
            propertyMinInt.intValue = newValue;

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

        private void DrawMaxIntField(ref Rect position, SerializedProperty propertyMaxInt)
        {
            GUIContent maxLabel = new GUIContent("max");
            int newValue = EditorGUI.IntField(position, propertyMaxInt.intValue);
            if (newValue < _currentMinValue)
            {
                newValue = _currentMinValue;
            }
            propertyMaxInt.intValue = newValue;

            position.x += position.width;
            DrawPropertyLabel(ref position, maxLabel, propertyMaxInt);
        }
    }
}