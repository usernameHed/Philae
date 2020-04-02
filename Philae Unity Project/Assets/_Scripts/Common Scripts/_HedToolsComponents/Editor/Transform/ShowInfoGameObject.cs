using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.extension.editor.sceneView;

namespace extUnityComponents.transform
{
    public class ShowInfoGameObject
    {
        private Transform[] _currentTargets = null;
        private Editor _currentEditor;
        private TransformHiddedTools[] _transformHiddedTools;

        public void Init(Transform[] targets,
            Editor currentEditor,
            TransformHiddedTools[] transformHiddedTools)
        {
            _currentTargets = targets;
            _currentEditor = currentEditor;
            _transformHiddedTools = transformHiddedTools;
            SetHideTools();
        }

        public void SetHideTools()
        {
            for (int i = 0; i < _transformHiddedTools.Length; i++)
            {
                if (_transformHiddedTools[i] == null)
                {
                    continue;
                }
                Tools.hidden = _transformHiddedTools[i].HideHandle;
            }
        }

        public void CustomDisable()
        {
            Tools.hidden = false;
        }

        public void CustomOnInspectorGUI()
        {
            ShowName();
            AllowHandle();
        }

        private void ShowName()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("show name: ");
                _transformHiddedTools[0].ShowName = ExtGUIToggles.Toggle(_transformHiddedTools[0].ShowName, _transformHiddedTools[0], "", "", out bool showNameHasChanged);
                _transformHiddedTools[0].ColorText = ExtGUIColor.ColorPicker(_transformHiddedTools[0].ColorText, _transformHiddedTools[0], "Color name: ", "Color of the text present in the SceneView", out bool colorHasChanged);
                if (showNameHasChanged || colorHasChanged)
                {
                    for (int i = 1; i < _transformHiddedTools.Length; i++)
                    {
                        _transformHiddedTools[i].ShowName = _transformHiddedTools[0].ShowName;
                        _transformHiddedTools[i].ColorText = _transformHiddedTools[0].ColorText;
                    }
                }
            }
        }

        private void AllowHandle()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Hide Handle: ");
                _transformHiddedTools[0].HideHandle = ExtGUIToggles.Toggle(_transformHiddedTools[0].HideHandle, _transformHiddedTools[0], "", "", out bool hideHandleHasChanged);
                if (hideHandleHasChanged)
                {
                    bool changeChildrenToo = false;
                    if (_transformHiddedTools[0].transform.childCount > 0)
                    {
                        changeChildrenToo = ExtGUI.DrawDisplayDialog("Change children too ?", "Do you want to apply this setting to all children ?", "Yes", "No", false);
                    }

                    if (changeChildrenToo)
                    {
                        HideHandleInAllChildren(_transformHiddedTools[0].HideHandle, _transformHiddedTools[0]);
                    }

                    for (int i = 1; i < _transformHiddedTools.Length; i++)
                    {
                        _transformHiddedTools[i].HideHandle = _transformHiddedTools[0].HideHandle;
                        if (changeChildrenToo)
                        {
                            HideHandleInAllChildren(_transformHiddedTools[0].HideHandle, _transformHiddedTools[i]);
                        }
                    }
                    SetHideTools();
                }
            }
        }

        private void HideHandleInAllChildren(bool hideHandle, TransformHiddedTools parent)
        {
            TransformHiddedTools[] allChild = _transformHiddedTools[0].transform.GetExtComponentsInChildrens<TransformHiddedTools>(99, true);
            for (int i = 0; i < allChild.Length; i++)
            {
                allChild[i].HideHandle = hideHandle;
            }
        }

        // This function is called for each instance of "spawnPoint" in the scene. 
        // Make sure to pass the correct class in the first argument. In this case ItemSpawnPoint
        // Make sure it is a "static" function
        // name it whatever you want
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        public static void DrawHandles(TransformHiddedTools spawnPoint, GizmoType gizmoType)
        {
            if (!spawnPoint.ShowName)
            {
                return;
            }

            bool selected = gizmoType == (GizmoType.Active | GizmoType.InSelectionHierarchy | GizmoType.Selected);

            GUIStyle style = new GUIStyle(); // This is optional
            style.normal.textColor = spawnPoint.ColorText;

            //Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.Label(spawnPoint.transform.position, spawnPoint.name, style);

            //Handles.zTest = UnityEngine.Rendering.CompareFunction.;
            Handles.color = spawnPoint.ColorText;
            Handles.CircleHandleCap(0, spawnPoint.transform.position, ExtSceneView.GetSceneViewCameraTransform().rotation, HandleUtility.GetHandleSize(spawnPoint.transform.position) * 0.05f, EventType.Repaint);
            ExtHandle.DrawSphereArrow(spawnPoint.ColorText, spawnPoint.transform.position, 0.05f);
        }

        public void CustomOnSceneGUI()
        {

        }
    }
}