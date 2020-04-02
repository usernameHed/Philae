using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using hedCommon.extension.runtime;
using hedCommon.extension.editor;
using hedCommon.extension.editor.sceneView;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace extUnityComponents.transform
{
    /// <summary>
    /// Move, rotate & scale a parent, whild keeping children still
    /// </summary>
    public class LockChildren
    {
        /// <summary>
        /// Save state of a child
        /// </summary>
        public struct SaveChildPosition
        {
            public Transform Child;
            public Transform ChildTmp;
            public Vector3 PreviousPosition;
            public Quaternion PreviousRotation;
            public Vector3 PreviousScale;
        }

        private struct SaveStateParent
        {
            public Vector3 InitialPosition;
            public Quaternion InitialRotation;
            public Vector3 InitialScale;

            public SaveStateParent(Vector3 position, Quaternion rotation, Vector3 scale)
            {
                InitialPosition = position;
                InitialRotation = rotation;
                InitialScale = scale;
            }
        }

        //all child saved
        private List<SaveChildPosition> _allChilds = new List<SaveChildPosition>();
        private Vector3 _parentPosition;
        private Quaternion _parentRotation;
        private Vector3 _parentScale;
        private SaveStateParent _initialStateParent;

        private GameObject _parentTmpCreated = null;
        private List<GameObject> _allChildsTmps = new List<GameObject>();

        private bool _lockChildrenPosition = false;  //keep this value outside of the inspected gameObject
        private PivotMode _cachedPivotMode;
        private bool _hasBeenInternallyInit = false;
        private Transform _currentTarget = null;

        private TinyEditorWindowSceneView _tinyLockChildrenWindow;
        private readonly string KEY_EDITOR_PREF_LOCK_CHILDREN_WINDOW = "KEY_EDITOR_PREF_LOCK_CHILDREN_WINDOW";
        private Editor _currentEditor;

        public void Init(Transform parent, Editor current)
        {
            _currentTarget = parent;
            _currentEditor = current;
            _lockChildrenPosition = false;
            _hasBeenInternallyInit = false;
        }

        public void InitOnFirstOnSceneGUI()
        {
            InitTinyEditorWindow();
        }

        private void InitTinyEditorWindow()
        {
            _tinyLockChildrenWindow = new TinyEditorWindowSceneView();
            _tinyLockChildrenWindow.TinyInit(KEY_EDITOR_PREF_LOCK_CHILDREN_WINDOW, "Move Parent Tool", TinyEditorWindowSceneView.DEFAULT_POSITION.MIDDLE_RIGHT);
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
                    GUILayout.Label("Lock Children positions:");

                    if (DisplayToggle(_currentTarget))
                    {
                        LockStateChanged();
                    }
                }
                if (_lockChildrenPosition && IsParentAPrefabs(_currentTarget))
                {
                    GUI.color = Color.yellow;
                    GUILayout.Label("/!\\ Can't lock scaling on a prefab");
                }
            }
        }

        private bool DisplayToggle(Transform target)
        {
            bool disable = false;
            string messageButton = (_lockChildrenPosition) ? "Unlock [L]" : "Lock [L]";
            bool previousState = _lockChildrenPosition;

            if (!IsTargetHaveChild(target))
            {
                disable = true;
                messageButton = "no child to lock";
                _lockChildrenPosition = previousState = false;
            }

            EditorGUI.BeginDisabledGroup(disable);
            {
                _lockChildrenPosition = GUILayout.Toggle(_lockChildrenPosition, messageButton, EditorStyles.miniButton);
            }
            EditorGUI.EndDisabledGroup();

            return (previousState != _lockChildrenPosition);
        }

        private bool IsTargetHaveChild(Transform target)
        {
            return (target.childCount != 0);
        }

        public bool IsParentAPrefabs(Transform parent)
        {
            return (PrefabUtility.IsPartOfAnyPrefab(parent.gameObject));
        }

        /// <summary>
        /// called when the lock state changed
        /// </summary>
        private void LockStateChanged()
        {
            if (_lockChildrenPosition)
            {
                _tinyLockChildrenWindow.IsClosed = false;
                if (!_hasBeenInternallyInit)
                {
                    InitLock();
                }
            }
            else
            {
                CustomDisable();
            }
        }

        /// <summary>
        /// called for initializing the parent state and all his children state
        /// </summary>
        /// <param name="parent"></param>
        private void InitLock()
        {
            _cachedPivotMode = Tools.pivotMode;
            Tools.pivotMode = PivotMode.Pivot;
            InitTmpGameObjects(_currentTarget);
            SaveChildsPosition(_currentTarget);
            _parentPosition = _currentTarget.position;
            _initialStateParent = new SaveStateParent(_currentTarget.position, _currentTarget.rotation, _currentTarget.localScale);
            //ExtReflexion.SetSearch(_currentTarget.name);
        }

        private void InitTmpGameObjects(Transform parent)
        {
            if (_parentTmpCreated != null || parent == null)
            {
                return;
            }
            _parentTmpCreated = ExtGameObject.CreateLocalGameObject("invisible - " + parent.name, parent.parent, parent.localPosition, parent.localRotation, parent.localScale, -1, null);
            _parentTmpCreated.hideFlags = HideFlags.HideInHierarchy;
            InitTmpChilds(parent);
        }


        private void InitTmpChilds(Transform parent)
        {
            _allChildsTmps.Clear();
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject child = ExtGameObject.CreateLocalGameObject("invisible - " + parent.GetChild(i).name, _parentTmpCreated.transform, parent.GetChild(i).localPosition, parent.GetChild(i).localRotation, parent.GetChild(i).localScale, -1, null);
                child.hideFlags = HideFlags.HideInHierarchy;
                _allChildsTmps.Add(child);
            }
        }

        /// <summary>
        /// save all childrens in an array, and store there position
        /// </summary>
        /// <param name="parent">parent of all childrens</param>
        public void SaveChildsPosition(Transform parent)
        {
            _allChilds.Clear();
            for (int i = 0; i < parent.childCount; i++)
            {
                SaveChildPosition newChild = new SaveChildPosition()
                {
                    Child = parent.GetChild(i),
                    ChildTmp = _allChildsTmps[i].transform,
                    PreviousPosition = parent.GetChild(i).position,
                    PreviousRotation = parent.GetChild(i).rotation,
                    PreviousScale = parent.GetChild(i).localScale//GetGlobalToLocalScaleFactor(parent.GetChild(i))
                };
                _allChilds.Add(newChild);
            }
        }

        /// <summary>
        /// clear childs and remap original pivot tool
        /// </summary>
        public void CustomDisable()
        {
            if (_parentTmpCreated != null)
            {
                GameObject.DestroyImmediate(_parentTmpCreated);
            }
            if (_allChildsTmps.Count > 0)
            {
                for (int i = _allChildsTmps.Count - 1; i >= 0; i--)
                {
                    if (_allChildsTmps[i] != null)
                    {
                        GameObject.DestroyImmediate(_allChildsTmps[i]);
                    }
                }
            }

            if (_lockChildrenPosition)
            {
                Tools.pivotMode = _cachedPivotMode;
            }

            _hasBeenInternallyInit = false;
            //ExtReflexion.SetSearch("");
        }

        public void CustomOnSceneGUI()
        {
            if (_parentTmpCreated != null)
            {
                for (int i = 0; i < _allChildsTmps.Count; i++)
                {
                    ExtDrawGuizmos.DebugWireSphere(_allChildsTmps[i].transform.position, Color.red, 0.1f);
                    ExtDrawGuizmos.DebugCross(_allChildsTmps[i].transform);
                }
            }

            if (IsPressingL(Event.current))
            {
                _lockChildrenPosition = !_lockChildrenPosition;
                LockStateChanged();
                _currentEditor.Repaint();
            }

            if (_lockChildrenPosition)
            {
                ExtDrawGuizmos.DebugCross(_currentTarget.position, _currentTarget.rotation, 1);
                ShowTinyEditor();
            }
        }

        /// <summary>
        /// return true if we press on J
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private bool IsPressingL(Event current)
        {
            if ((current.keyCode == KeyCode.L && current.type == EventType.KeyUp))
            {
                return (true);
            }
            return (false);
        }


        private void ShowTinyEditor()
        {
            _tinyLockChildrenWindow.ShowEditorWindow(DisplayLockChildrenWindow, SceneView.currentDrawingSceneView, Event.current);
            if (_tinyLockChildrenWindow.IsClosed)
            {
                _lockChildrenPosition = false;
                EditorUtility.SetDirty(_currentTarget);
                LockStateChanged();
            }
        }

        private void DisplayLockChildrenWindow()
        {
            if (GUILayout.Button("Mean position"))
            {
                ExtUndo.Record(_currentTarget, "Tool: lock children move");
                Vector3 mean = ExtVector3.GetMeanOfXPoints(_currentTarget.GetAllChild().ToArray(), out Vector3 sizeBoundingBox, true);
                _currentTarget.position = mean;
            }
            if (GUILayout.Button("Mean Rotation"))
            {
                Quaternion mean = ExtQuaternion.AverageQuaternion(ExtQuaternion.GetAllRotation(_currentTarget.GetAllChild().ToArray()));
                _currentTarget.rotation = mean;
            }
            if (GUILayout.Button("Reset"))
            {
                _currentTarget.position = _initialStateParent.InitialPosition;
                _currentTarget.rotation = _initialStateParent.InitialRotation;
                _currentTarget.localScale = _initialStateParent.InitialScale;
            }
        }


        /// <summary>
        /// called every update of the editor
        /// </summary>
        /// <param name="target">manange only one target</param>
        public void OnEditorUpdate()
        {
            if (_currentTarget == null || !_lockChildrenPosition)
            {
                return;
            }

            if (IsParentChanged(_currentTarget))
            {
                MoveEveryChildToTherePreviousPosition(_currentTarget);
            }

            SaveCurrentParentPosition(_currentTarget);
            SaveChildsPosition(_currentTarget);
        }

        /// <summary>
        /// Say if the parent is moved, in position, rotation or scale
        /// </summary>
        /// <param name="parent">parent to check</param>
        /// <returns>true if the parent moved, rotate or scaled</returns>
        public bool IsParentChanged(Transform parent)
        {
            return (_parentPosition != parent.position
                || _parentRotation != parent.rotation
                || _parentScale != parent.localScale);
        }

        /// <summary>
        /// here move every children to there original state
        /// </summary>
        /// <param name="parent"></param>
        public void MoveEveryChildToTherePreviousPosition(Transform parent)
        {
            _parentTmpCreated.transform.position = parent.transform.position;
            _parentTmpCreated.transform.rotation = parent.transform.rotation;

            for (int i = 0; i < _allChilds.Count; i++)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    MoveChildNotInsidePrefabs(i, parent);
                }
                else
                {
                    if (IsParentAPrefabs(parent))
                    {
                        MoveChildInsidePrefabs(i, parent);
                    }
                    else
                    {
                        MoveChildNotInsidePrefabs(i, parent);
                    }
                }
#else
            MoveChildNotInsidePrefabs(i, parent);
#endif
            }
        }

        private void MoveChildNotInsidePrefabs(int i, Transform parent)
        {
            ExtUndo.Record(_allChilds[i].Child, "Lock tool child lock");

            _allChilds[i].ChildTmp.position = _allChilds[i].PreviousPosition;
            _allChilds[i].ChildTmp.rotation = _allChilds[i].PreviousRotation;
            _allChilds[i].Child.SetParent(_parentTmpCreated.transform);
            _allChilds[i].Child.position = _allChilds[i].PreviousPosition;
            _allChilds[i].Child.rotation = _allChilds[i].PreviousRotation;
            _allChilds[i].Child.localScale = _allChilds[i].ChildTmp.localScale;
            _allChilds[i].Child.SetParent(parent);
        }


        /// <summary>
        /// move object inside the prefabs, we can't manage the scale here
        /// </summary>
        /// <param name="i"></param>
        /// <param name="parent"></param>
        private void MoveChildInsidePrefabs(int i, Transform parent)
        {
            _allChilds[i].ChildTmp.position = _allChilds[i].PreviousPosition;
            _allChilds[i].ChildTmp.rotation = _allChilds[i].PreviousRotation;
            _allChilds[i].Child.position = _allChilds[i].PreviousPosition;
            _allChilds[i].Child.rotation = _allChilds[i].PreviousRotation;
            _allChilds[i].Child.localScale = _allChilds[i].PreviousScale;
        }

        /// <summary>
        /// save parent state for later
        /// </summary>
        /// <param name="parent"></param>
        public void SaveCurrentParentPosition(Transform parent)
        {
            _parentPosition = parent.position;
            _parentRotation = parent.rotation;
            _parentScale = parent.localScale;
        }
    }
}