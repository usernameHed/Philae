using UnityEngine;
using static UnityEditor.EditorGUILayout;
using hedCommon.time;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.editor.editorWindow;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ExtUnityComponents
{
    /// <summary>
    /// Move, rotate & scale a parent, whild keeping children still
    /// </summary>
    public class MovePivot
    {
        private Transform _currentTarget = null;
        private MeshFilter _currentTargetMesh = null;
        private bool _isMovingPivot = false;
        private bool _hasBeenInternallyInit = false;
        private MeshFilterHiddedTools _meshFilterHiddedTools;
        private Mesh _mesh;  //mesh of the selected object;
        private EditorChronoWithNoTimeEditor _editorChrono = new EditorChronoWithNoTimeEditor();
        private ExtUtilityEditor.HitSceneView _hitScene = new ExtUtilityEditor.HitSceneView();
        private TinyEditorWindowSceneView _tinyMovePivotWindow;
        private readonly string KEY_EDITOR_PREF_PIVOT_WINDOW = "KEY_EDITOR_PREF_PIVOT_WINDOW";

        private GameObject _invisiblePivot;

        /// <summary>
        /// When we deselect the object, if we changed the pivot, a popup apeare.
        /// If we set cancel: we want to reselect the gameObject, and reset all the previously selected option
        /// </summary>
        private static bool _weQuitButWeDoNotWantedTo = false;
        private static Vector3 _lastLocalPositionPivot;

        public void Init(Transform parent, MeshFilter meshFilter, MeshFilterHiddedTools meshFilterHiddedTools)
        {
            _currentTarget = parent;
            _currentTargetMesh = meshFilter;
            _meshFilterHiddedTools = meshFilterHiddedTools;
            _isMovingPivot = false;
            _hasBeenInternallyInit = false;
            _mesh = meshFilter.sharedMesh;
            Debug.Log("here init ??");

            if (_weQuitButWeDoNotWantedTo)
            {
                _weQuitButWeDoNotWantedTo = false;
                _isMovingPivot = true;
                CreatePivot(_lastLocalPositionPivot);
            }
        }

        private void InitTinyEditorWindow()
        {
            _tinyMovePivotWindow = new TinyEditorWindowSceneView();
            _tinyMovePivotWindow.TinyInit(KEY_EDITOR_PREF_PIVOT_WINDOW, "Pivot Tool", TinyEditorWindowSceneView.DEFAULT_POSITION.UP_LEFT);
        }

        /// <summary>
        /// called for initializing the parent state and all his children state
        /// </summary>
        /// <param name="parent"></param>
        private void InitMovePivot()
        {
            Debug.Log("start pivot mode");
            Tools.hidden = true;
            _weQuitButWeDoNotWantedTo = false;
            _meshFilterHiddedTools.Init();
            //ExtReflexion.SetSearch(_currentTarget.name);
            CreatePivot(_meshFilterHiddedTools.GetPivotReferencePosition());
            //emptyChild.hideFlags = HideFlags.HideInHierarchy;
        }

        private void CreatePivot(Vector3 position)
        {
            _invisiblePivot = ExtGameObject.CreateLocalGameObject("invisible pivot", _currentTarget, position, Quaternion.identity, Vector3.one, -1, null);
        }

        /// <summary>
        /// if we quit the gameObject without saving the pivot, ask to save it
        /// </summary>
        /// <returns></returns>
        private bool AskBeforeQuit()
        {
            //we are not in pivot mode
            if (!_isMovingPivot)
            {
                return (false);
            }

            //we didn't change the pivot
            if (_invisiblePivot.transform.localPosition == _meshFilterHiddedTools.GetPivotReferencePosition())
            {
                return (false);
            }

            int option = EditorUtility.DisplayDialogComplex("Unsaved Changes",
            "Do you want to save the pivot change you made before quitting?",
            "Save",
            "Cancel",
            "Don't Save");

            switch (option)
            {
                // Save.
                case 0:
                    SavePivotChange();
                    Debug.Log("here save and quit");
                    return (false);

                // Cancel.
                case 1:
                    Selection.activeGameObject = _currentTarget.gameObject;
                    Debug.Log("here cancel, and NOT QUIT");
                    _weQuitButWeDoNotWantedTo = true;
                    _lastLocalPositionPivot = _invisiblePivot.transform.localPosition;

                    GameObject.DestroyImmediate(_invisiblePivot);

                    return (true);

                // Don't Save.
                case 2:
                    Debug.Log("here don't save, but quit");
                    return (false);
            }

            return (false);
        }

        /// <summary>
        /// clear childs and remap original pivot tool
        /// </summary>
        public void CustomDisable(bool askForSave)
        {
            if (_meshFilterHiddedTools == null)
            {
                return;
            }

            /*
            //if true, don't quit
            if (askForSave && AskBeforeQuit())
            {

                return;
            }
            */


            _hasBeenInternallyInit = false;
            _isMovingPivot = false;
            Tools.hidden = false;
            if (_invisiblePivot != null)
            {
                GameObject.DestroyImmediate(_invisiblePivot);
            }
        }

        private void SavePivotChange()
        {
            _meshFilterHiddedTools.SaveDefinitivePivot(_invisiblePivot.transform.localPosition, _invisiblePivot.transform.localRotation);
            //ApplyPivotOnMesh();
        }

        private void ApplyPivotOnMesh()
        {
            ExtUndo.Record(_currentTargetMesh, "Tool: record mesh pivot change");
            //ExtDrawGuizmos.DebugWireSphere(GetPivotVector(), Color.red, 0.01f, 5f);


            Vector3[] verts = _mesh.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] += _invisiblePivot.transform.localPosition;
            }
            _mesh.vertices = verts; //Assign the vertex array back to the mesh
            _mesh.RecalculateBounds(); //Recalculate bounds of the mesh, for the renderer's sake
                                       //The 'center' parameter of certain colliders needs to be adjusted
                                       //when the transform position is modified
        }

        private bool DisplayToggle(Transform target)
        {
            string messageButton = (_isMovingPivot) ? "Apply" : "Move Pivot";
            bool previousState = _isMovingPivot;

            _isMovingPivot = GUILayout.Toggle(_isMovingPivot, messageButton, EditorStyles.miniButton);

            return (previousState != _isMovingPivot);
        }

        /// <summary>
        /// called when the lock state changed
        /// </summary>
        private void LockStateChanged(bool saveBeforeQuit)
        {
            if (_isMovingPivot)
            {
                _tinyMovePivotWindow.IsClosed = false;
                if (!_hasBeenInternallyInit)
                {
                    InitMovePivot();
                }
            }
            else
            {
                if (saveBeforeQuit)
                {
                    SavePivotChange();
                }
                CustomDisable(false);
            }
        }

        /// <summary>
        /// display Lock children button
        /// </summary>
        public void CustomOnInspectorGUI()
        {
            using (VerticalScope verticalScope = new VerticalScope(EditorStyles.helpBox))
            {
                using (HorizontalScope horizontalScope = new HorizontalScope())
                {
                    GUILayout.Label("Move Pivot: ");

                    if (DisplayToggle(_currentTarget))
                    {
                        LockStateChanged(true);
                    }
                }
                if (_isMovingPivot)
                {
                    DisplayMoveInformationWhenActive();
                }
            }
        }

        /// <summary>
        /// here display other settings when move pivot is active
        /// </summary>
        private void DisplayMoveInformationWhenActive()
        {
            
        }

        private void SetHitObject()
        {
            _hitScene = ExtUtilityEditor.SetCurrentOverObject(_hitScene, true);
            
            if (_hitScene.objHit != null)
            {
                if (_hitScene.objHit != _currentTarget && _hitScene.objHit != _hitScene.objHit.IsChildOf(_currentTarget))
                {
                    _hitScene.objHit = null;
                }
            }
            //DrawHit();
        }

        private void DrawHit()
        {
            if (_hitScene.objHit == null || _hitScene.Hit.collider == null)
            {
                return;
            }

            Vector3[] normals = _mesh.normals;
            int[] triangles = _mesh.triangles;

            // Extract local space normals of the triangle we hit
            Vector3 n0 = normals[triangles[_hitScene.Hit.triangleIndex * 3 + 0]];
            Vector3 n1 = normals[triangles[_hitScene.Hit.triangleIndex * 3 + 1]];
            Vector3 n2 = normals[triangles[_hitScene.Hit.triangleIndex * 3 + 2]];

            // interpolate using the barycentric coordinate of the hitpoint
            Vector3 baryCenter = _hitScene.Hit.barycentricCoordinate;

            // Use barycentric coordinate to interpolate normal
            Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
            // normalize the interpolated normal
            interpolatedNormal = interpolatedNormal.normalized;

            // Transform local space normals to world space
            Transform hitTransform = _hitScene.Hit.collider.transform;
            interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);

            // Display with Debug.DrawLine
            Debug.DrawRay(_hitScene.Hit.point, interpolatedNormal);
        }

        private bool InvisiblePivotHasChanged()
        {
            return (!ExtVector3.IsClose(_meshFilterHiddedTools.GetPivotReferencePosition(), _invisiblePivot.transform.localPosition, 0.001f)
                || !ExtQuaternion.IsClose(_meshFilterHiddedTools.GetPivotReferenceRotation(), _invisiblePivot.transform.localRotation, 0.001f));
        }

        private bool DisplayApplyButton()
        {
            bool hasChanged = InvisiblePivotHasChanged();

            GUI.backgroundColor = hasChanged ? Color.green : Color.white;

            EditorGUI.BeginDisabledGroup(!hasChanged);
            {
                if (GUILayout.Button("Apply Change"))
                {
                    _isMovingPivot = false;
                    LockStateChanged(true);
                    return (true);
                }
            }
            EditorGUI.EndDisabledGroup();

            GUI.backgroundColor = Color.white;
            return (false);
        }

        public void DisplayPivotWindow()
        {
            if (_invisiblePivot == null)
            {
                return;
            }

            ExtUndo.Record(_invisiblePivot.transform, "Tools: record invisible pivot move");

            if (DisplayApplyButton())
            {
                return;
            }

            


            using (VerticalScope VerticalScope = new VerticalScope())
            {
                GUILayout.Label("Current Pivot:");
                EditorGUI.BeginDisabledGroup(true);
                {
                    Vector3 pivotRef = _meshFilterHiddedTools.GetPivotReferencePosition();
                    ExtGUIVectorFields.Vector3Field(_meshFilterHiddedTools.GetPivotReferencePosition(), null, out bool valueHasChanged2, "Position:", true);
                    ExtGUIVectorFields.Vector3Field(_meshFilterHiddedTools.GetPivotReferenceRotation().eulerAngles, null, out valueHasChanged2, "Rotation:", true);


                    //GUILayout.Label("X:" + ExtMathf.Round(pivotRef.x, 2) + "   Y:" + ExtMathf.Round(pivotRef.y, 2) + "   Z:" + ExtMathf.Round(pivotRef.z, 2));
                }
                EditorGUI.EndDisabledGroup();
            }

            ExtGUI.HorizontalLineThickness(ExtColor.grayInspectorDark, thickness: 2, paddingTop: 2, paddingBottom: 2, paddingLeft: 0.05f, paddingRight: 0.05f);

            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                EditorGUI.BeginDisabledGroup(!InvisiblePivotHasChanged());
                {
                    if (GUILayout.Button("Undo"))
                    {
                        _invisiblePivot.transform.localPosition = _meshFilterHiddedTools.GetPivotReferencePosition();
                        _invisiblePivot.transform.localRotation = _meshFilterHiddedTools.GetPivotReferenceRotation();
                    }
                }
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Center Pivot"))
                {
                    _invisiblePivot.transform.localPosition = Vector3.zero;
                    _invisiblePivot.transform.localRotation = Quaternion.identity;
                }
            }
            Vector3 pivot = ExtGUIVectorFields.Vector3Field(_invisiblePivot.transform.localPosition, _meshFilterHiddedTools, out bool valueHasChanged, "Position:", true);
            if (valueHasChanged)
            {
                _invisiblePivot.transform.localPosition = pivot;
            }
            Vector3 rotationEuler = ExtGUIVectorFields.Vector3Field(_invisiblePivot.transform.localRotation.eulerAngles, _meshFilterHiddedTools, out valueHasChanged, "Rotation:", true);
            if (valueHasChanged)
            {
                _invisiblePivot.transform.localRotation = Quaternion.Euler(rotationEuler);
            }

            ExtGUI.HorizontalLineThickness(Color.white, thickness: 1, paddingTop: 1, paddingBottom: 1, paddingLeft: 0.05f, paddingRight: 0.05f);

            DisplayOptions();
        }

        private void DisplayOptions()
        {
            GUILayout.Label("Option:");
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                _meshFilterHiddedTools.SnapVertex = GUILayout.Toggle(_meshFilterHiddedTools.SnapVertex, "Snap Vertex", EditorStyles.miniButton);
                _meshFilterHiddedTools.LimitBounds = ExtGUIToggles.Toggle(_meshFilterHiddedTools.LimitBounds, null, "Limit to Bounds", out bool valueHasChanged, EditorStyles.miniButton);
            }
        }

        public void InitOnFirstOnSceneGUI()
        {
            Debug.Log("init scene GUI");
            InitTinyEditorWindow();
        }

        public void CustomOnSceneGUI()
        {
            if (_currentTarget == null || !_isMovingPivot)
            {
                return;
            }

            
            SetHitObject();
            if (_meshFilterHiddedTools.LimitBounds)
            {
                ShowBounds();
            }

            if (Tools.current == Tool.Rotate)
            {
                ManageRotationChange();
            }
            else
            {
                ManagePositionChange();
            }

            ShowTinyEditor();
            DrawPivot();
        }


        private void ManageRotationChange()
        {
            EditorGUI.BeginChangeCheck();
            ExtUndo.Record(_meshFilterHiddedTools, "Tools: record rotate pivot of mesh filter");

            if (_invisiblePivot == null)
            {
                CreatePivot(_meshFilterHiddedTools.GetPivotReferencePosition());
            }

            Quaternion rotation = _invisiblePivot.transform.rotation;//_currentTarget.position + _meshFilterHiddedTools.GetTempsPivot();
            using (new Handles.DrawingScope(Color.green))
            {
                rotation = Handles.RotationHandle(_invisiblePivot.transform.rotation, _invisiblePivot.transform.position);
            }
            if (EditorGUI.EndChangeCheck())
            {
                _invisiblePivot.transform.rotation = rotation;
            }
        }

        private void ManagePositionChange()
        {
            EditorGUI.BeginChangeCheck();

            ExtUndo.Record(_meshFilterHiddedTools, "Tools: record move pivot of mesh filter");

            if (_invisiblePivot == null)
            {
                CreatePivot(_meshFilterHiddedTools.GetPivotReferencePosition());
            }

            Vector3 position = _invisiblePivot.transform.position;//_currentTarget.position + _meshFilterHiddedTools.GetTempsPivot();

            using (new Handles.DrawingScope(Color.green))
            {
                position = Handles.PositionHandle(position, (Tools.pivotRotation == PivotRotation.Global) ? Tools.handleRotation : _invisiblePivot.transform.rotation);
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (_meshFilterHiddedTools.SnapVertex)
                {
                    position = SnapVertex(position, _currentTarget.position);
                }
                if (_meshFilterHiddedTools.LimitBounds)
                {
                    position = LimitBounds(position);
                }
                _invisiblePivot.transform.position = position;
                //Vector3 delta = position - _currentTarget.position;
                //Vector3 pivotCenter = Vector3.zero;
                //Vector3 deltaRotated = _currentTarget.rotation * (delta - pivotCenter) + pivotCenter;

            }
        }

        /// <summary>
        /// draw the real pivot
        /// </summary>
        private void DrawPivot()
        {
            if (_invisiblePivot == null)
            {
                return;
            }

            ExtDrawGuizmos.DebugCross(_currentTarget.position + _meshFilterHiddedTools.GetPivotReferencePosition(),
               _currentTarget.rotation * _meshFilterHiddedTools.GetPivotReferenceRotation());


            ExtDrawGuizmos.DebugCross(_invisiblePivot.transform.position, _invisiblePivot.transform.rotation, 0.5f);
        }

        private void ShowTinyEditor()
        {
            _tinyMovePivotWindow.ShowEditorWindow(DisplayPivotWindow, SceneView.currentDrawingSceneView, Event.current);
            if (_tinyMovePivotWindow.IsClosed)
            {
                //_isMovingPivot = false;
                EditorUtility.SetDirty(_currentTarget);
                _lastLocalPositionPivot = _invisiblePivot.transform.localPosition;
                CustomDisable(true);
            }
            else
            {
                //ExtReflexion.PreventUnselect(_currentTarget.gameObject);
            }
        }


        /// <summary>
        /// show the bounds of the mesh
        /// </summary>
        private void ShowBounds()
        {
            Bounds bounds = _mesh.bounds;
            ExtDrawGuizmos.DebugLocalCube(_currentTarget, bounds.size, bounds.center);
            //Gizmos.DrawWireCube(transform.position, transform.renderer.bounds.size);
        }

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }

        Vector3 ClosestPointOnMeshOBB(MeshFilter meshFilter, Vector3 worldPoint)
        {
            // First, we transform the point into the local coordinate space of the mesh.
            var localPoint = meshFilter.transform.InverseTransformPoint(worldPoint);

            // Next, we compare it against the mesh's axis-aligned bounds in its local space.
            var localClosest = meshFilter.sharedMesh.bounds.ClosestPoint(localPoint);

            // Finally, we transform the local point back into world space.
            return meshFilter.transform.TransformPoint(localClosest);
        }

        /// <summary>
        /// limit bounds
        /// </summary>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        private Vector3 LimitBounds(Vector3 newPosition)
        {

            Bounds bounds = _mesh.bounds;
            Debug.DrawLine(newPosition, _currentTarget.position + bounds.center, Color.white, 0.01f);

            Vector3 third = ClosestPointOnMeshOBB(_currentTargetMesh, newPosition);

            ExtDrawGuizmos.DebugWireSphere(_currentTarget.position + third, Color.yellow, 0.2f, 0.2f);

            if (bounds.Contains(newPosition))
            {
                Debug.Log("inside the bounding box");
                return (newPosition);
            }
            Debug.Log("here ouside the bounds !!!");
            return (newPosition);
        }

        /// <summary>
        /// snap to the nearest vertex
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="oldPosition"></param>
        /// <returns></returns>
        private Vector3 SnapVertex(Vector3 newPosition, Vector3 oldPosition)
        {
            if (_hitScene.objHit != null)
            {
                newPosition = _hitScene.pointHit;
            }

            

            Vector3 savePosition = newPosition;


            Vector3 centerObject = Vector3.zero;
            Vector3[] verts = GetRotatedVertex(_mesh, oldPosition, centerObject);

            int indexFound = -1;
            Vector3 closest = ExtMathf.GetClosestPoint(newPosition, verts, ref indexFound);

            float minHit = 0.3f;
            float minWithoutHit = 0.6f;

            if (_hitScene.objHit != null)
            {
                if (indexFound != -1 && Vector3.Distance(closest, newPosition) < minHit)
                {
                    newPosition = closest;
                    ExtDrawGuizmos.DebugWireSphere(newPosition, Color.red, 0.05f);
                }
            }
            else
            {
                if (indexFound != -1 && Vector3.Distance(closest, newPosition) < minWithoutHit)
                {
                    newPosition = closest;
                    ExtDrawGuizmos.DebugWireSphere(newPosition, Color.red, 0.05f);
                }
            }
            return (newPosition);
        }

        

        private Vector3[] GetRotatedVertex(Mesh mesh, Vector3 gameObjectPosition, Vector3 pivotCenter)
        {
            Vector3[] verts = _mesh.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = _currentTarget.rotation * (verts[i] - pivotCenter) + pivotCenter;

                verts[i] += gameObjectPosition;
            }
            return (verts);
        }

        /// <summary>
        /// called every update of the editor
        /// </summary>
        /// <param name="target">manange only one target</param>
        public void CustomOnEditorApplicationUpdate()
        {
            if (_currentTarget == null || !_isMovingPivot)
            {
                return;
            }
        }
    }
}