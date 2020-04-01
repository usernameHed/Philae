using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace extUnityComponents.collider
{
    public class FitBox2D
    {
        private BoxCollider2D[] _currentTargets = null;
        private DecoratorComponentsEditor _currentEditor;
        private SpriteRenderer[] _spriteRenderer;

        public void Init(BoxCollider2D[] parent, DecoratorComponentsEditor current)
        {
            _currentTargets = parent;
            _currentEditor = current;

            SetupMeshFilter();
        }

        private void SetupMeshFilter()
        {
            _spriteRenderer = new SpriteRenderer[_currentTargets.Length];
            for (int i = 0; i < _currentTargets.Length; i++)
            {
                _spriteRenderer[i] = _currentTargets[i].GetComponent<SpriteRenderer>();
            }
        }

        public bool IsNullOrEmptyOrEmptyContent()
        {
            if (_spriteRenderer == null)
            {
                return (true);
            }
            if (_spriteRenderer.Length == 0)
            {
                return (true);
            }
            for (int i = 0; i < _spriteRenderer.Length; i++)
            {
                if (_spriteRenderer[i] == null)
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
                GUILayout.Label("Fit collider to the sprite:");

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
                        GUILayout.Button("No spriteRenderer");
                    }
                    EditorGUI.EndDisabledGroup();
                }

            }
        }

        private void ApplyFitMeshs()
        {
            for (int k = 0; k < _currentTargets.Length; k++)
            {
                Undo.RecordObject(_currentTargets[k], "datas Box Collider");
                ExtColliders.AutoSizeCollider2d(_spriteRenderer[k], _currentEditor.GetTargetIndex<BoxCollider2D>(k));
            }
        }
    }
}