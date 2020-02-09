using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
    [Serializable]
    public struct ExtCube
    {
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _localScale;

        private Matrix4x4 _cubeMatrix;
        private Vector3 _p1;
        private Vector3 _p2;
        private Vector3 _p3;
        private Vector3 _p4;
        private Vector3 _p5;
        private Vector3 _p6;
        private Vector3 _p7;
        private Vector3 _p8;

        private Vector3 _v41;
        private Vector3 _v51;
        private Vector3 _v21;

        private float _v41Squared;
        private float _v51Squared;
        private float _v21Squared;

        private float _uP1;
        private float _uP2;
        private float _vP1;
        private float _vP4;
        private float _wP1;
        private float _wP5;


        public ExtCube(Vector3 position, Quaternion rotation, Vector3 localScale) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;

            UpdateMatrix();
        }

        ///      6 - 7
        ///    /   /
        ///  5 - 8
        /// 
        ///     2 - 3
        ///   /   /
        ///  1 - 4
        private void UpdateMatrix()
        {
            _cubeMatrix = ExtMatrix.GetMatrixTRS(_position, _rotation, _localScale);

            Vector3 size = Vector3.one;

            _p1 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((-size) * 0.5f));
            _p2 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, -size.y, size.z) * 0.5f));
            _p3 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            _p4 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            _p5 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            _p6 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, size.z) * 0.5f));
            _p7 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((size) * 0.5f));
            _p8 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, size.y, -size.z) * 0.5f));

            _v41 = (_p4 - _p1);
            _v21 = (_p2 - _p1);
            _v51 = (_p5 - _p1);

            _v41Squared = _v41.LengthSquared();
            _v21Squared = _v21.LengthSquared();
            _v51Squared = _v51.LengthSquared();

            _uP1 = ExtVector3.DotProduct(-_v21, _p1);
            _uP2 = ExtVector3.DotProduct(-_v21, _p2);
            _vP1 = ExtVector3.DotProduct(-_v41, _p1);
            _vP4 = ExtVector3.DotProduct(-_v41, _p4);
            _wP1 = ExtVector3.DotProduct(-_v51, _p1);
            _wP5 = ExtVector3.DotProduct(-_v51, _p5);
        }

        public void Draw(Color color)
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DrawLocalCube(_p1, _p2, _p3, _p4, _p5, _p6, _p7, _p8, color);
#endif
        }

        public void DrawWithExtraSize(Color color, Vector3 extraSize)
        {
#if UNITY_EDITOR
            Matrix4x4 cubeMatrix = ExtMatrix.GetMatrixTRS(_position, _rotation, _localScale + extraSize);

            Vector3 size = Vector3.one;

            Vector3 p1 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((-size) * 0.5f));
            Vector3 p2 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, -size.y, size.z) * 0.5f));
            Vector3 p3 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 p4 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            Vector3 p5 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 p6 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, size.z) * 0.5f));
            Vector3 p7 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((size) * 0.5f));
            Vector3 p8 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, size.y, -size.z) * 0.5f));
            ExtDrawGuizmos.DrawLocalCube(p1, p2, p3, p4, p5, p6, p7, p8, color);
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
        /// https://math.stackexchange.com/questions/1472049/check-if-a-point-is-inside-a-rectangular-shaped-area-3d
        /// 
        ///      6 - 7
        ///    /   /
        ///  5 - 8
        /// 
        ///     2 - 3
        ///   /   /
        ///  1 - 4
        ///  
        /// perpendiculare:         not perpendiculare:
        /// u = 1 - 2               u = (1 - 4) × (1 - 5)
        /// v = 1 - 4               v = (1 - 2) × (1 - 5)
        /// w = 1 - 5               w = (1 - 2) × (1 - 4)
        /// 
        /// </summary>
        public bool IsInsideShape(Vector3 k)
        {
#if UNITY_EDITOR
            if (_p1 == _p2 && _p1 == _p4 && _p1 == _p5)
            {
                return (false);
            }
#endif

            float ux = ExtVector3.DotProduct(-_v21, k);
            float vx = ExtVector3.DotProduct(-_v41, k);
            float wx = ExtVector3.DotProduct(-_v51, k);

            bool isUBetween = ux.IsBetween(_uP2, _uP1);
            bool isVBetween = vx.IsBetween(_vP4, _vP1);
            bool isWBetween = wx.IsBetween(_wP5, _wP1);

            bool isInside = isUBetween && isVBetween && isWBetween;
            return (isInside);
        }

        /// <summary>
        /// Return the closest point on the surface of the cube, from a given point x
        /// 
        ///      6 - 7
        ///    /   /
        ///  5 - 8
        /// 
        ///     2 - 3
        ///   /   /
        ///  1 - 4
        ///  
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            float tx = ExtVector3.DotProduct(k - _p1, _v41) / _v41Squared;
            float ty = ExtVector3.DotProduct(k - _p1, _v51) / _v51Squared;
            float tz = ExtVector3.DotProduct(k - _p1, _v21) / _v21Squared;

            tx = tx < 0 ? 0 : tx > 1 ? 1 : tx;
            ty = ty < 0 ? 0 : ty > 1 ? 1 : ty;
            tz = tz < 0 ? 0 : tz > 1 ? 1 : tz;

            Vector3 closestPoint = tx * _v41 + ty * _v51 + tz * _v21 + _p1;

            return (closestPoint);
        }

        //Returns true if a line segment (made up of linePoint1 and linePoint2) is fully or partially in a rectangle
        //made up of RectA to RectD. The line segment is assumed to be on the same plane as the rectangle. If the line is 
        //not on the plane, use ProjectPointOnPlane() on linePoint1 and linePoint2 first.
        public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
        {
            bool pointAInside = false;
            bool pointBInside = false;

            pointAInside = IsPointIn2DRectangle(linePoint1, rectA, rectC, rectB, rectD);

            if (!pointAInside)
            {
                pointBInside = IsPointIn2DRectangle(linePoint2, rectA, rectC, rectB, rectD);
            }

            //none of the points are inside, so check if a line is crossing
            if (!pointAInside && !pointBInside)
            {
                bool lineACrossing = ExtLine.AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
                bool lineBCrossing = ExtLine.AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
                bool lineCCrossing = ExtLine.AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
                bool lineDCrossing = ExtLine.AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);

                if (lineACrossing || lineBCrossing || lineCCrossing || lineDCrossing)
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            else
            {
                return (true);
            }
        }

        //Returns true if "point" is in a rectangle mad up of RectA to RectD. The line point is assumed to be on the same 
        //plane as the rectangle. If the point is not on the plane, use ProjectPointOnPlane() first.
        public static bool IsPointIn2DRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
        {
            Vector3 vector;
            Vector3 linePoint;

            //get the center of the rectangle
            vector = rectC - rectA;
            float size = -(vector.magnitude / 2f);
            vector = ExtVector3.AddVectorLength(vector, size);
            Vector3 middle = rectA + vector;

            Vector3 xVector = rectB - rectA;
            float width = xVector.magnitude / 2f;

            Vector3 yVector = rectD - rectA;
            float height = yVector.magnitude / 2f;

            linePoint = ExtLine.ProjectPointOnLine(middle, xVector.normalized, point);
            vector = linePoint - point;
            float yDistance = vector.magnitude;

            linePoint = ExtLine.ProjectPointOnLine(middle, yVector.normalized, point);
            vector = linePoint - point;
            float xDistance = vector.magnitude;

            if ((xDistance <= width) && (yDistance <= height))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }
        //end class
    }
    //end nameSpace
}