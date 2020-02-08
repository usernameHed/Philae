using UnityEditor;
using UnityEngine;

namespace ExtUnityComponents.transform
{
    [CustomEditor(typeof(LockRotationFromParent))]
    public class LockRotationFromParentEditor : DecoratorComponentsEditor
    {
        private LockRotationFromParent _lockRotation;

        public LockRotationFromParentEditor()
            : base(showExtension: false, tinyEditorName: "Lock Rotation From Parent")
        {

        }

        public override void OnCustomEnable()
        {
            _lockRotation = GetTarget<LockRotationFromParent>();
        }

        /// <summary>
        /// get called by the decorator to show a tiny editor
        /// </summary>
        public override void ShowTinyEditorContent()
        {
            _lockRotation.RotateWithTheParent = GUILayout.Toggle(_lockRotation.RotateWithTheParent, "Rotate with the parent");
            GUI.changed = false;
            _lockRotation.OverrideRotationUpGlobal = GUILayout.Toggle(_lockRotation.OverrideRotationUpGlobal, "Override Rotation To Up Global");

            if (GUI.changed)
            {
                if (_lockRotation.OverrideRotationUpGlobal)
                {
                    _lockRotation.RotateUp();
                }
                else
                {
                    _lockRotation.ResetLocalRotation();
                }
            }


            if (GUILayout.Button("Reset local Rotation"))
            {
                _lockRotation.ResetLocalRotation();
            }
        }

    }
}