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
    public struct ExtSpline
    {
        [Serializable]
        public struct PointsOnSpline
        {
            public Vector3 PointLocal;
            public Vector3 PointGlobal;

            public PointsOnSpline(Vector3 localPoint)
            {
                PointLocal = localPoint;
                PointGlobal = localPoint;
            }

            public PointsOnSpline(Vector3 localPoint, Matrix4x4 matrix) : this()
            {
                MovePoint(localPoint, matrix);
            }

            public void MovePoint(Vector3 localPoint, Matrix4x4 matrix)
            {
                PointLocal = localPoint;
                PointGlobal = matrix.MultiplyPoint3x4(PointLocal);
            }

            public void MovePoint(Matrix4x4 matrix)
            {
                PointGlobal = matrix.MultiplyPoint3x4(PointLocal);
            }
        }
        [SerializeField]
        private bool _closed;

        [SerializeField]
        private Vector3 _position;
        public Vector3 Position { get { return (_position); } }
        [SerializeField]
        private Quaternion _rotation;
        public Quaternion Rotation { get { return (_rotation); } }
        [SerializeField]
        private Vector3 _localScale;
        public Vector3 LocalScale { get { return (_localScale); } }

        [SerializeField]
        private PointsOnSpline[] _listPoints;
        [SerializeField]
        private Matrix4x4 _splinesMatrix;

        public ExtSpline(Vector3 position,
            Quaternion rotation,
            Vector3 localScale) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;

            _listPoints = new PointsOnSpline[3];
            _listPoints[0] = new PointsOnSpline(new Vector3(0, 0, 0));
            _listPoints[1] = new PointsOnSpline(new Vector3(-0.1f, 0, 0.2f));
            _listPoints[2] = new PointsOnSpline(new Vector3(0.1f, 0, 0.4f));
            UpdateMatrix();
        }

        public Vector3 GlobalPointAt(int index)
        {
            return (_listPoints[index].PointGlobal);
        }
        public Vector3 LocalPointAt(int index)
        {
            return (_listPoints[index].PointLocal);
        }

        private void UpdateMatrix()
        {
            _splinesMatrix = Matrix4x4.TRS(_position, _rotation, _localScale);
            UpdateGlobalLineFromLocalOnes();
        }

        private void UpdateGlobalLineFromLocalOnes()
        {
            for (int i = 0; i < _listPoints.Length; i++)
            {
                _listPoints[i].MovePoint(_splinesMatrix);
            }
        }

        public void AddPointLocal(Vector3 p1)
        {
            _listPoints = ExtArray.Add(_listPoints, new PointsOnSpline(p1, _splinesMatrix));
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            UpdateMatrix();
        }

        /// <summary>
        /// Return the closest point from all lines
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            return (Vector3.zero);
            //return (ExtLine.GetClosestPointFromLines(k, _listLines, out int indexLine));
        }

        public bool GetClosestPointIfWeCan(Vector3 k, out Vector3 closestPoint, GravityOverrideLineTopDown[] gravityOverride)
        {
            if (_listPoints.Length == 0)
            {
                closestPoint = Vector3.zero;
                return (false);
            }
            //closestPoint = ExtLine.GetClosestPointFromLines(k, _listLines, out int indexLine);
            //bool canApplyGravity = _listLines[indexLine].GetClosestPointIfWeCan(k, out closestPoint, gravityOverride[indexLine]);
            //return (canApplyGravity);
            closestPoint = Vector3.zero;
            return (false);
        }


#if UNITY_EDITOR
        public void Draw(Color color)
        {
            if (_listPoints == null)
            {
                return;
            }
            for (int i = 1; i < _listPoints.Length; i++)
            {
                Debug.DrawLine(_listPoints[i - 1].PointGlobal, _listPoints[i].PointGlobal, color);
            }
            if (_closed)
            {
                Debug.DrawLine(_listPoints[_listPoints.Length - 1].PointGlobal, _listPoints[0].PointGlobal, color);
            }
        }

        public void DrawWithExtraSize(Color color, float offset)
        {
            for (int i = 1; i < _listPoints.Length; i++)
            {
                ExtDrawGuizmos.DebugCapsuleFromInsidePoint(_listPoints[i - 1].PointGlobal, _listPoints[i].PointGlobal, color, offset, 0, true, false);
            }
        }
#endif
        //end class
    }
    //end nameSpace
}