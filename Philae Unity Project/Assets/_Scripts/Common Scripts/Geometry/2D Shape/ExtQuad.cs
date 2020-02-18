using hedCommon.extension.runtime;
using philae.gravity.attractor.gravityOverride;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape2d
{

    /// <summary>
    /// a perfect 3D Quad, with 3 points
    ///     2 ------------- 3 
    ///   /               /   
    ///  1 ------------ 4  
    /// </summary>
    [Serializable]
    public struct ExtQuad
    {
        [SerializeField, ReadOnly]
        private Vector3 _position;
        public Vector3 Position { get { return (_position); } }
        [SerializeField, ReadOnly]
        private Quaternion _rotation;
        public Quaternion Rotation { get { return (_rotation); } }
        [SerializeField, ReadOnly]
        private Vector3 _localScale;
        public Vector3 LocalScale { get { return (_localScale); } }



        [SerializeField]
        private ExtPlane _plane;
        public bool AllowBottom { get { return (_plane.AllowBottom); }}
        public Vector3 Normal { get { return (_plane.Normal); } }

        [SerializeField, ReadOnly]
        private Matrix4x4 _quadMatrix;

        [SerializeField, ReadOnly]
        private Vector3 _p1; public Vector3 P1 { get { return (_p1); } }
        [SerializeField, ReadOnly]
        private Vector3 _p2; public Vector3 P2 { get { return (_p2); } }
        [SerializeField, ReadOnly]
        private Vector3 _p3; public Vector3 P3 { get { return (_p3); } }
        [SerializeField, ReadOnly]
        private Vector3 _p4; public Vector3 P4 { get { return (_p4); } }

        [SerializeField, ReadOnly]
        private Vector3 _v41; public Vector3 V41 { get { return (_v41); } }
        [SerializeField, ReadOnly]
        private Vector3 _v21; public Vector3 V21 { get { return (_v21); } }
        [SerializeField, ReadOnly]
        private float _v41Squared;
        [SerializeField, ReadOnly]
        private float _v21Squared;

        [SerializeField, ReadOnly]
        private float _uP1;
        [SerializeField, ReadOnly]
        private float _uP2;
        [SerializeField, ReadOnly]
        private float _vP1;
        [SerializeField, ReadOnly]
        private float _vP4;

        [SerializeField, ReadOnly]
        private float _lengthX; public float LenghtX { get { return (_lengthX); } }
        [SerializeField, ReadOnly]
        private float _lengthY; public float LenghtY { get { return (_lengthY); } }

        public ExtQuad(Vector3 position, Quaternion rotation, Vector3 localScale) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;

            UpdateMatrix();
        }

        ///     2 ------------- 3 
        ///   /               /   
        ///  1 ------------ 4  
        public ExtQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) : this()
        {
            _position = ExtVector3.GetMeanOfXPoints(p1, p2, p3, p4);
            Vector3 x = p1 - p2;
            Vector3 y = p1 - p4;

            Vector3 up = ExtVector3.CrossProduct(x, y);
            _rotation = ExtRotation.QuaternionFromVectorDirector(up, x);
            _rotation = ExtRotation.RotateQuaternion(_rotation, new Vector3(-90, 0, 0));
            _localScale = new Vector3(y.magnitude, 1, x.magnitude);
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _quadMatrix = ExtMatrix.GetMatrixTRS(_position, _rotation, _localScale);

            Vector3 size = Vector3.one;

            _p1 = _quadMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, 0, -size.z) * 0.5f));
            _p2 = _quadMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, 0, size.z) * 0.5f));
            _p3 = _quadMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, 0, size.z) * 0.5f));
            _p4 = _quadMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, 0, -size.z) * 0.5f));

            _v41 = (_p4 - _p1);
            _v21 = (_p2 - _p1);

            _lengthX = _v41.magnitude;
            _lengthY = _v21.magnitude;

            _v41Squared = _v41.LengthSquared();
            _v21Squared = _v21.LengthSquared();

            _uP1 = ExtVector3.DotProduct(-_v21, _p1);
            _uP2 = ExtVector3.DotProduct(-_v21, _p2);
            _vP1 = ExtVector3.DotProduct(-_v41, _p1);
            _vP4 = ExtVector3.DotProduct(-_v41, _p4);

            _plane.MoveShape(_position, _quadMatrix.Up());
        }

        public void Draw(Color color, bool drawFace, bool drawPoints)
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DrawLocalQuad(this, color, drawFace, drawPoints, true);
#endif
        }

        public void DrawWithExtraSize(Color color, Vector3 extraSize, bool drawFace, bool drawPoints)
        {
#if UNITY_EDITOR
            Matrix4x4 cubeMatrix = ExtMatrix.GetMatrixTRS(_position, _rotation, _localScale + extraSize);

            Vector3 size = Vector3.one;

            Vector3 p1 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((-size) * 0.5f));
            Vector3 p2 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, -size.y, size.z) * 0.5f));
            Vector3 p3 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 p4 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, -size.z) * 0.5f));
            ExtDrawGuizmos.DrawLocalQuad(p1, p2, p3, p4, color, drawFace, drawPoints);
#endif
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            UpdateMatrix();
        }

        /// <summary>
        /// return true if the position is inside the sphape
        ///     2 ------------- 3 
        ///   /               /   
        ///  1 ------------ 4     
        /// 
        /// </summary>
        public bool IsInsideShape(Vector3 k)
        {
#if UNITY_EDITOR
            if (_p1 == _p2 && _p1 == _p4)
            {
                return (false);
            }
#endif
            if (!_plane.AllowBottom && !_plane.IsAbove(k))
            {
                return (false);
            }

            float ux = ExtVector3.DotProduct(-_v21, k);
            float vx = ExtVector3.DotProduct(-_v41, k);

            bool isUBetween = ux.IsBetween(_uP2, _uP1);
            bool isVBetween = vx.IsBetween(_vP4, _vP1);

            bool isInside = isUBetween && isVBetween;
            return (isInside);
        }

        /// <summary>
        /// return closest point IF point is not bellow with the settings set to false
        /// </summary>
        /// <param name="k"></param>
        /// <param name="canApplyGravity"></param>
        /// <returns></returns>
        public Vector3 GetClosestPoint(Vector3 k, out bool canApplyGravity)
        {
            if (!_plane.AllowBottom && !_plane.IsAbove(k))
            {
                canApplyGravity = false;
                return (k);
            }
            canApplyGravity = true;

            Vector3 vK1 = k - _p1;
            float tx = ExtVector3.DotProduct(vK1, _v41) / _v41Squared;
            float tz = ExtVector3.DotProduct(vK1, _v21) / _v21Squared;

            tx = ExtMathf.SetBetween(tx, 0, 1);
            tz = ExtMathf.SetBetween(tz, 0, 1);

            Vector3 closestPoint = tx * _v41
                                    + tz * _v21
                                    + _p1;

            return (closestPoint);
        }

        public Vector3 GetClosestPointIfWeCan(Vector3 k, out bool canApplyGravity, GravityOverrideQuad gravityQuad)
        {
            if (!_plane.AllowBottom && !_plane.IsAbove(k))
            {
                canApplyGravity = false;
                return (k);
            }
            return (GetClosestPoint(k, out canApplyGravity));
        }

        /*
        private Vector3 GetGoodPointUnidirectionnal(Vector3 p, Vector3 foundPosition)
        {
            //Vector3 projectedOnPlane = TriPlane.Project(EdgeAb.A, TriNorm.normalized, p);
            Vector3 dirPlayer = p - foundPosition;

            float dotPlanePlayer = ExtVector3.DotProduct(dirPlayer.normalized, TriNormNormalize);
            if ((dotPlanePlayer < 0 && !inverseDirection) || dotPlanePlayer > 0 && inverseDirection)
            {
                return (foundPosition);
            }
            else
            {
                return (ExtVector3.GetNullVector());
            }
        }

        // Return point q on (or in) rect (specified by a, b, and c), closest to given point p
        public Vector3 ClosestPtPointRect(Vector3 p)
        {
            Vector3 d = p - A;
            // Start result at top-left corner of rect; make steps from there
            Vector3 q = A;
            // Clamp p’ (projection of p to plane of r) to rectangle in the across direction
            float dist = Vector3.Dot(d, AB);
            //float maxdist = Vector3.Dot(ab, ab);
            if (dist >= maxdistA)
                q += AB;
            else if (dist > 0.0f)
                q += (dist / maxdistA) * AB;
            // Clamp p' (projection of p to plane of r) to rectangle in the down direction
            dist = Vector3.Dot(d, AC);

            if (dist >= maxdistC)
                q += AC;
            else if (dist > 0.0f)
                q += (dist / maxdistC) * AC;

            return (q);
        }
        */
    }

}
