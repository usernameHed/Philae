using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace hedCommon.geometry.movable
{
    [ExecuteInEditMode]
    public abstract class MovableShape : MonoBehaviour
    {
        public Vector3 Position { get { return (transform.position); } }
        public Quaternion Rotation { get { return (transform.rotation); } }
        public Vector3 LocalScale { get { return (transform.localScale); } }

        public bool IsMovableOnPlay { get; private set; }

        public Color ColorShape = Color.white;
        private Vector3 _oldScale = new Vector3(-1, -1, -1);
        public UnityAction HasMoved;
        public UnityAction HasScaled;

        /// <summary>
        /// must be called the first time the shape is created (at spawn, or from editor)
        /// </summary>
        [Button]
        public abstract void InitOnCreation();

        /// <summary>
        /// must be called after ANY change in the shape stucture
        /// </summary>
        public abstract void ChangeShapeStucture();

        /// <summary>
        /// move the shape
        /// </summary>
        public abstract void Move();

        public void CustomUpdateIfCanMove()
        {
            bool canMove = !Application.isPlaying
                             || (Application.isPlaying && IsMovableOnPlay);

            if (canMove && transform.hasChanged)
            {
                if (transform.localScale != _oldScale)
                {
                    _oldScale = transform.localScale;
                    HasScaled?.Invoke();
                }
                else
                {
                    HasMoved?.Invoke();
                }
                Move();
                transform.hasChanged = false;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// draw the shape
        /// </summary>
        public abstract void Draw(Color color);
#endif
    }
}