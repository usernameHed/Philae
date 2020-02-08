using ExtUnityComponents;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static philae.gravity.physicsBody.PhysicBody;

namespace hedCommon.tools
{
    [CustomEditor(typeof(LockPositionXYZ), true)]
    public class LockPositionXYZEditor : DecoratorComponentsEditor
    {
        protected LockPositionXYZ _lockPosition;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public LockPositionXYZEditor()
            : base()
        {

        }


        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            _lockPosition = GetTarget<LockPositionXYZ>();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            if (ExtPrefabs.IsEditingInPrefabMode(_lockPosition.gameObject))
            {
                return;
            }
            ConstrainPosition constrain = this.GetValue<ConstrainPosition>("_constrains");
            ConstrainPosition.ApplyConstraint(constrain, _lockPosition.transform);
        }
    }
}