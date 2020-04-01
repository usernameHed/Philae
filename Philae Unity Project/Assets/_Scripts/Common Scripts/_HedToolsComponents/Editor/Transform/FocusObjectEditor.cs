using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace extUnityComponents.transform
{
    [CustomEditor(typeof(FocusObject))]
    public class FocusObjectEditor : DecoratorComponentsEditor
    {
        private FocusObject _focusObject;

        public FocusObjectEditor()
            : base(showExtension: false, tinyEditorName: "Focus gameObject")
        {

        }

        public override void OnCustomEnable()
        {
            _focusObject = GetTarget<FocusObject>();
            if (_focusObject == null)
            {
                return;
            }
            if (base.TinyEditorWindow == null)
            {
                return;
            }
            base.TinyEditorWindow.NameEditorWindow = _focusObject.MessageInfo;
        }

        public override void ShowTinyEditorContent()
        {
            if (!_focusObject.EnableVisual)
            {
                return;
            }

            if (GUILayout.Button("Focus " + _focusObject.MessageInfo))
            {
                Selection.activeGameObject = _focusObject.ToFocus;
            }
        }
    }
}