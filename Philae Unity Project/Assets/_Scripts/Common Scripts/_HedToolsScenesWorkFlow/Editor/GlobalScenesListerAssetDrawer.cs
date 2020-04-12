using hedCommon.extension.propertyAttribute.generic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.sceneWorkflow
{
    [CustomPropertyDrawer(typeof(GlobalScenesListerAsset), true)]
    public class GlobalScenesListerAssetDrawer : GenericScriptableObjectPropertyDrawer<GlobalScenesListerAsset>
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