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
    [ExecuteInEditMode]
    public abstract class MovableShape : MonoBehaviour
    {
        public Vector3 Position { get { return (transform.position); } }
        public Quaternion Rotation { get { return (transform.rotation); } }
        public Vector3 LocalScale { get { return (transform.localScale); } }

        public Color ColorShape = Color.white;

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

        /// <summary>
        /// draw the shape
        /// </summary>
        public abstract void Draw(Color color);

        private void OnDrawGizmos()
        {
            Draw(ColorShape);
        }
    }
}