using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    [ExecuteInEditMode, RequireComponent(typeof(MovableShape))]
    public class ShapeUpdater : MonoBehaviour
    {
        [SerializeField]
        protected MovableShape _movableShape;

        private void OnEnable()
        {
            if (_movableShape == null)
            {
                _movableShape = GetComponent<MovableShape>();
            }
        }

        private void Update()
        {
            _movableShape.CustomUpdateIfCanMove();
        }

        private void OnDrawGizmos()
        {
            _movableShape.Draw(_movableShape.ColorShape);
        }
    }
}