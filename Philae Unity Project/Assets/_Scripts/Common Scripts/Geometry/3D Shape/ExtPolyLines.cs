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
        private ExtLine[] _listLinesLocal;

        [SerializeField]
        private ExtLine[] _listLines;
        [SerializeField]
        private Matrix4x4 _polyLinesMatrix;

#if UNITY_EDITOR
        [Serializable]
        public struct PointsInfo
        {
            public bool IsAttached;
            public List<int> AttachedIndex;
            public bool IsSelected;
        }
        [SerializeField]
        private List<PointsInfo> _pointsInfos;
#endif

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
            
            for (int i = 0; i < _listLinesLocal.Length; i++)
            {
                _listLines[i].MoveShape(
                    _polyLinesMatrix.MultiplyPoint3x4(_listLinesLocal[i].P1),
                    _polyLinesMatrix.MultiplyPoint3x4(_listLinesLocal[i].P2));
            }
            
        }

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            for (int i = 0; i < _listLines.Length; i++)
            {
                _listLines[i].Draw(color);
            }
        }
#endif

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            UpdateMatrix();
        }

        /// <summary>
        /// Return the closest point from all lines
        ///   
        /// </summary>
        public bool GetClosestPoint(Vector3 k, out Vector3 closestPoint)
        {
            closestPoint = ExtLine.GetClosestPointFromLines(k, _listLines);
            return (true);
        }
        //end class
    }
    //end nameSpace
}