using UnityEngine;
using UnityEditor;

namespace hedCommon.extension.runtime.animationCurve
{

    [CustomPropertyDrawer(typeof(CurveAttribute))]
    public class CurveDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CurveAttribute curve = attribute as CurveAttribute;
            if (property.propertyType == SerializedPropertyType.AnimationCurve)
            {
                EditorGUI.CurveField(position, property, Color.cyan, new Rect(curve.X.Min, curve.Y.Min, curve.X.Max, curve.Y.Max));
            }
        }
    }
}