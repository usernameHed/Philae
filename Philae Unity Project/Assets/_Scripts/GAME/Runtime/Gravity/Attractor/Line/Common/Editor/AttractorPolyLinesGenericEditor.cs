using ExtUnityComponents.transform;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.editor;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.gravityOverride;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor.line
{
    public class AttractorPolyLinesGenericEditor : AttractoLineGenericEditor
    {
        public delegate void DeleteLineAtIndex(int index);
        public delegate void AddLineAtTheEnd();
        public DeleteLineAtIndex DeleteLineByIndex;
        public AddLineAtTheEnd AddedLine;

        private SerializedProperty _listLinesLocal;
        private SerializedProperty _listLinesGlobal;

        public virtual void OnCustomEnable(
            Editor targetEditor,
            GameObject targetGameObject,
            NeedToUpdateLines needToUpdateLines,
            NeedToReConstructLines needToReConstructLines,
            AddLineAtTheEnd addLine,
            DeleteLineAtIndex deletedLine)
        {
            base.OnCustomEnable(targetEditor, targetGameObject, needToUpdateLines, needToReConstructLines);
            AddedLine = addLine;
            DeleteLineByIndex = deletedLine;
        }

        public virtual void ConstructLines(
            SerializedProperty matrix,
            List<PointInLines> pointInLines,
            SerializedProperty arrayLine,
            SerializedProperty arrayLocal)
        {
            base.ConstructLines(matrix, pointInLines);
            _listLinesLocal = arrayLocal;
            _listLinesGlobal = arrayLine;
        }

        public override void CustomOnSceneGUI(SceneView sceneview)
        {
            base.CustomOnSceneGUI(sceneview);
        }

        /// <summary>
        /// called on a Tiny Editor Content
        /// </summary>
        public override void ShowTinyEditorContent()
        {
            base.ShowTinyEditorContent();

            if (EditorOptions.Instance.SetupLinesOfSphape)
            {
                ButtonsSetupsLinesTools();
            }
        }

        protected override void PreventDelete()
        {
            if (EditorOptions.Instance.SetupLinesOfSphape && ExtEventEditor.IsKeyDown(KeyCode.Delete))
            {
                ExtEventEditor.Use();
                DeleteSelectedPoints();
            }
        }

        private void ButtonsSetupsLinesTools()
        {
            if (GUILayout.Button("Add Line"))
            {
                AddLine();
            }
        }

        private void AddLine()
        {
            _targetEditor.UpdateEditor();

            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(0, 0, 0.3f);
            AddLineLocal(p1, p2);

            _targetEditor.ApplyModification();

            AddedLine?.Invoke();
            _needToReConstructLines?.Invoke();
        }

        public void AddLineLocal(Vector3 p1, Vector3 p2)
        {
            _listLinesLocal.arraySize = _listLinesLocal.arraySize + 1;
            _listLinesGlobal.arraySize = _listLinesGlobal.arraySize + 1;
            SerializedProperty lastLineLocal = _listLinesLocal.GetArrayElementAtIndex(_listLinesLocal.arraySize - 1);
            ExtShapeSerializeProperty.MoveLineFromSerializeProperties(lastLineLocal, p1, p2);

            SerializedProperty lastLineGlobal = _listLinesGlobal.GetArrayElementAtIndex(_listLinesGlobal.arraySize - 1);
            ExtShapeSerializeProperty.MoveLineFromSerializeProperties(lastLineGlobal, _polyLineMatrix.MultiplyPoint3x4(p1), _polyLineMatrix.MultiplyPoint3x4(p2));
        }

        private void DeleteSelectedPoints()
        {
            _targetEditor.UpdateEditor();

            List<int> deletedLines = new List<int>(_listLinesLocal.arraySize);
            deletedLines.Clear();
            for (int i = 0; i < PointsSelected.Count; i++)
            {
                //don't delete a line already deleted
                if (deletedLines.Contains(PointsSelected[i].IndexLine))
                {
                    continue;
                }
                int indexInArray = PointsSelected[i].IndexLine - deletedLines.Count;
                DeleteLine(indexInArray);
                deletedLines.Add(PointsSelected[i].IndexLine);
            }

            _targetEditor.ApplyModification();

            _needToReConstructLines?.Invoke();
        }

        private void DeleteLine(int indexLine)
        {
            _listLinesLocal.DeleteArrayElementAtIndex(indexLine);
            _listLinesGlobal.DeleteArrayElementAtIndex(indexLine);
            DeleteLineByIndex?.Invoke(indexLine);
        }
        
    }
}