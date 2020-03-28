using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace ExtUnityComponents.collider
{
    public class FitBox
    {
        private BoxCollider[] _currentTargets = null;
        private DecoratorComponentsEditor _currentEditor;
        private MeshFilter[] _meshFilter;

        public void Init(BoxCollider[] parent, DecoratorComponentsEditor current)
        {
            _currentTargets = parent;
            _currentEditor = current;

            SetupMeshFilter();
        }

        private void SetupMeshFilter()
        {
            _meshFilter = new MeshFilter[_currentTargets.Length];
            for (int i = 0; i < _currentTargets.Length; i++)
            {
                _meshFilter[i] = _currentTargets[i].GetComponent<MeshFilter>();
            }
        }

        public bool IsNullOrEmptyOrEmptyContent()
        {
            if (_meshFilter == null)
            {
                return (true);
            }
            if (_meshFilter.Length == 0)
            {
                return (true);
            }
            for (int i = 0; i < _meshFilter.Length; i++)
            {
                if (_meshFilter[i] == null)
                {
                    return (true);
                }
            }
            return (false);
        }

        public void DisplayFitMesh()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Fit collider to the mesh:");

                if (!IsNullOrEmptyOrEmptyContent())
                {
                    if (GUILayout.Button("Fit"))
                    {
                        ApplyFitMeshs();
                    }
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    {
                        GUILayout.Button("No MeshFilter");
                    }
                    EditorGUI.EndDisabledGroup();
                }

            }
        }

        private void ApplyFitMeshs()
        {
            for (int k = 0; k < _currentTargets.Length; k++)
            {
                List<Vector3> positionChilds = new List<Vector3>();

                Undo.RecordObject(_currentTargets[k], "datas Box Collider");


                MeshFilter parent = _meshFilter[k];
                if (parent == null)
                {
                    continue;
                }

                Bounds bigBounds = parent.sharedMesh.bounds;

                MeshFilter[] allChilds = _currentTargets[k].gameObject.GetExtComponentsInChildrens<MeshFilter>(99, false);
                for (int i = 0; i < allChilds.Length; i++)
                {
                    //Debug.Log(allChilds[i].name);
                    positionChilds.Add(allChilds[i].transform.position);
                    bigBounds.Encapsulate(allChilds[i].sharedMesh.bounds);
                }

                _currentEditor.GetTargetIndex<BoxCollider>(k).size = bigBounds.size;
                _currentEditor.GetTargetIndex<BoxCollider>(k).center = bigBounds.center;
            }
        }
    }
}