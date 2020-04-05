using hedCommon.extension.propertyAttribute.generic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.sceneWorkflow
{
    [CustomPropertyDrawer(typeof(SceneAssetLister), true)]
    public class SceneAssetListerDrawer : GenericScriptableObjectPropertyDrawer<SceneAssetLister>
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float newY = DrawGUI(position, property, label);
        }
    }
}