using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.gravityOverride;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
    [Serializable]
    public struct ExtPolyLines
    {
        private Vector3 _position;
        public Vector3 Position { get { return (_position); } }
        private Quaternion _rotation;
        public Quaternion Rotation { get { return (_rotation); } }
        private Vector3 _localScale;

        [SerializeField]
        private List<ExtLine> _listLines;

        private Matrix4x4 _polyLinesMatrix;

        public ExtPolyLines(Vector3 position,
            Quaternion rotation,
            Vector3 localScale) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _polyLinesMatrix = Matrix4x4.TRS(_position, _rotation, _localScale);
            Debug.Log("move ??");
            for (int i = 0; i < _listLines.Count; i++)
            {
                _listLines[i].MoveShape(
                    _polyLinesMatrix.MultiplyPoint3x4(_listLines[i].P1),
                    _polyLinesMatrix.MultiplyPoint3x4(_listLines[i].P2));
            }
            
        }

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            for (int i = 0; i < _listLines.Count; i++)
            {
                //Vector3 realP1 = _polyLinesMatrix.MultiplyPoint3x4(_listLines[i].P1);
                //Vector3 realP2 = _polyLinesMatrix.MultiplyPoint3x4(_listLines[i].P2);
                //Debug.DrawLine(realP1, realP2);
                _listLines[i].Draw(color);
            }
        }
#endif

        private float MaxXY(Vector3 size)
        {
            return (Mathf.Max(size.x, size.z));
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            UpdateMatrix();
        }

        /// <summary>
        /// Return the closest point on the surface of the cylinder
        /// https://diego.assencio.com/?index=ec3d5dfdfc0b6a0d147a656f0af332bd
        ///   
        /// </summary>
        public bool GetClosestPoint(Vector3 k, out Vector3 closestPoint)
        {
            closestPoint = Vector3.zero;
            return (false);
        }
        //end class
    }
    //end nameSpace
}