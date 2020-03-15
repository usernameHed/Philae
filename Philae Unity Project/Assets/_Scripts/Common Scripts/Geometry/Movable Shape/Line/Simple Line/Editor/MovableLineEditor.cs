using hedCommon.extension.editor;
using hedCommon.geometry.movable;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor.line
{
    [CustomEditor(typeof(MovableLine), true)]
    public class MovableLineEditor : MovableShapeEditor
    {
        protected const string PROPERTY_EXT_LINE_3D = "_line3d";
        private MovableLine _attractorLine;
        private MovableLineGenericEditor _attractorLinesGeneric;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public MovableLineEditor()
            : base(false, "Line")
        {
            _attractorLinesGeneric = new MovableLineGenericEditor();
        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorLine = (MovableLine)GetTarget<MovableShape>();

            _attractorLinesGeneric.OnCustomEnable(this, _attractorLine.gameObject, LinesHasBeenUpdated, ConstructLines);
        }

        private void ConstructLines()
        {
            SerializedProperty line3d = this.GetPropertie(PROPERTY_EXT_LINE_3D);
            SerializedProperty matrix = line3d.GetPropertie("_linesMatrix");
            SerializedProperty line = line3d.GetPropertie("_line");
            SerializedProperty lineLocal = line3d.GetPropertie("_lineLocalPosition");

            List<PointInLines> lines = new List<PointInLines>(2)
            {
                new PointInLines(0, 0, lineLocal.GetPropertie("_p1"), lineLocal.GetPropertie("_p2"), line.GetPropertie("_p1"), line.GetPropertie("_p2")),
                new PointInLines(0, 1, lineLocal.GetPropertie("_p1"), lineLocal.GetPropertie("_p2"), line.GetPropertie("_p1"), line.GetPropertie("_p2"))
            };

            _attractorLinesGeneric.ConstructLines(matrix, lines);
        }

        /// <summary>
        /// called when lines points has been updated.
        /// determine what to do (update delta & deltaSquared cached ?)
        /// REMEMBER TO APPLY MODIFICATION AFTER !!!
        /// </summary>
        private void LinesHasBeenUpdated()
        {
            ExtShapeSerializeProperty.UpdateLineFromSerializeProperties(this.GetPropertie(PROPERTY_EXT_LINE_3D).GetPropertie("_line"));
            ExtShapeSerializeProperty.UpdateLineFromSerializeProperties(this.GetPropertie(PROPERTY_EXT_LINE_3D).GetPropertie("_lineLocalPosition"));
            this.ApplyModification();
        }

        public override void OnCustomDisable()
        {
            base.OnCustomDisable();
            _attractorLinesGeneric.OnCustomDisable();
        }

        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();
            _attractorLinesGeneric.ShowTinyEditorContent();
        }

        protected override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);
            _attractorLinesGeneric.CustomOnSceneGUI(sceneview);
        }
    }
}