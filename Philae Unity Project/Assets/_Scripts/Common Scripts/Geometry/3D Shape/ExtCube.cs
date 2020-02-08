using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
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


        public ExtCube(Vector3 position, Quaternion rotation, Vector3 localScale) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;

            UpdateMatrix();
        }

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
        }

        public void Draw(Color color)
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DrawLocalCube(_p1, _p2, _p3, _p4, _p5, _p6, _p7, _p8, color);
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
        public bool IsInsideShape(Vector3 x)
        {
#if UNITY_EDITOR
            if (_p1 == _p2 && _p1 == _p4 && _p1 == _p5)
            {
                return (false);
            }
#endif

            Vector3 u = _p1 - _p2;
            Vector3 v = _p1 - _p4;
            Vector3 w = _p1 - _p5;

            float ux = ExtVector3.DotProduct(u, x);
            float vx = ExtVector3.DotProduct(v, x);
            float wx = ExtVector3.DotProduct(w, x);


            float uP1 = ExtVector3.DotProduct(u, _p1);
            float uP2 = ExtVector3.DotProduct(u, _p2);
            float vP1 = ExtVector3.DotProduct(v, _p1);
            float vP4 = ExtVector3.DotProduct(v, _p4);
            float wP1 = ExtVector3.DotProduct(w, _p1);
            float wP5 = ExtVector3.DotProduct(w, _p5);

            bool isUBetween = ux.IsBetween(uP2, uP1);
            bool isVBetween = vx.IsBetween(vP4, vP1);
            bool isWBetween = wx.IsBetween(wP5, wP1);

            bool isInside = isUBetween && isVBetween && isWBetween;
            return (isInside);
        }

        //Returns true if a line segment (made up of linePoint1 and linePoint2) is fully or partially in a rectangle
        //made up of RectA to RectD. The line segment is assumed to be on the same plane as the rectangle. If the line is 
        //not on the plane, use ProjectPointOnPlane() on linePoint1 and linePoint2 first.
        public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
        {
            bool pointAInside = false;
            bool pointBInside = false;

            pointAInside = IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);

            if (!pointAInside)
            {
                pointBInside = IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);
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
        public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
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