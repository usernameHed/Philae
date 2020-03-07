using hedCommon.extension.editor;
using hedCommon.geometry.editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor.line
{
    [CustomEditor(typeof(AttractorLine))]
    public class AttractorLineEditor : AttractorEditor
    {
        private AttractorLine _attractorLine;
        private AttractoLineGenericEditor _attractorLinesGeneric;

        
        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public AttractorLineEditor()
            : base(false, "Line")
        {
            _attractorLinesGeneric = new AttractoLineGenericEditor();
        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorLine = (AttractorLine)GetTarget<Attractor>();

            _attractorLinesGeneric.OnCustomEnable(this, _attractorLine.gameObject, LinesHasBeenUpdated);
            ConstructLines();
        }

        private void ConstructLines()
        {
            SerializedProperty line3d = this.GetPropertie("_line3d");
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
            Debug.Log("ici line updated ??");
            ExtShapeSerializeProperty.UpdateLineFromSerializeProperties(this.GetPropertie("_line3d").GetPropertie("_line"));
            ExtShapeSerializeProperty.UpdateLineFromSerializeProperties(this.GetPropertie("_line3d").GetPropertie("_lineLocalPosition"));
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

            //this.UpdateSerializeProperties();
            _attractorLinesGeneric.CustomOnSceneGUI(sceneview);

        }
    }
}