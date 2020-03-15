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

        [SerializeField]
        protected bool _isMovableOnPlay = true;

        [SerializeField]
        private Vector3 _oldScale = new Vector3(-1, -1, -1);

        private void OnEnable()
        {
            if (_movableShape == null)
            {
                _movableShape = GetComponent<MovableShape>();
            }
        }

        public void CustomUpdateIfCanMove()
        {
            bool canMove =  !Application.isPlaying
                             || (Application.isPlaying && _isMovableOnPlay);

            if (canMove && transform.hasChanged)
            {
                if (transform.localScale != _oldScale)
                {
                    _oldScale = transform.localScale;
                }
                _movableShape.Move();
                transform.hasChanged = false;
            }
        }

        private void Update()
        {
            CustomUpdateIfCanMove();
        }
    }
}