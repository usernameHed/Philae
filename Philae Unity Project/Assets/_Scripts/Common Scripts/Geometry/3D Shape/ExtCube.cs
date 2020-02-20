using hedCommon.extension.runtime;
using hedCommon.extension.runtime.range;
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
    public struct ExtCube
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
        [SerializeField, ReadOnly]
        private Matrix4x4 _cubeMatrix;

        [SerializeField, ReadOnly]
        private Vector3 _p1;   public Vector3 P1 { get { return (_p1); } }
        [SerializeField, ReadOnly]
        private Vector3 _p2;   public Vector3 P2 { get { return (_p2); } }
        [SerializeField, ReadOnly]
        private Vector3 _p3;   public Vector3 P3 { get { return (_p3); } }
        [SerializeField, ReadOnly]
        private Vector3 _p4;   public Vector3 P4 { get { return (_p4); } }
        [SerializeField, ReadOnly]
        private Vector3 _p5;   public Vector3 P5 { get { return (_p5); } }
        [SerializeField, ReadOnly]
        private Vector3 _p6;   public Vector3 P6 { get { return (_p6); } }
        [SerializeField, ReadOnly]
        private Vector3 _p7;   public Vector3 P7 { get { return (_p7); } }
        [SerializeField, ReadOnly]
        private Vector3 _p8;   public Vector3 P8 { get { return (_p8); } }

        [SerializeField, ReadOnly]
        private Vector3 _v41;
        [SerializeField, ReadOnly]
        private Vector3 _v51;
        [SerializeField, ReadOnly]
        private Vector3 _v21;

        [SerializeField, ReadOnly]
        private float _v41Squared;
        [SerializeField, ReadOnly]
        private float _v51Squared;
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
        private float _wP1;
        [SerializeField, ReadOnly]
        private float _wP5;


        public ExtCube(Vector3 position, Quaternion rotation, Vector3 localScale) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;

            UpdateMatrix();
        }

        ///
        ///      6 ------------ 7
        ///    / |            / |
        ///  5 ------------ 8   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |  2 ----------|-- 3
        ///  |/             | /
        ///  1 ------------ 4
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

        public void Draw(Color color, bool drawFace, bool drawPoints)
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DrawLocalCube(_p1, _p2, _p3, _p4, _p5, _p6, _p7, _p8, color, drawFace, drawPoints);
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
        /// 
        ///      6 ------------ 7
        ///    / |            / |
        ///  5 ------------ 8   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |  2 ----------|-- 3
        ///  |/             | /
        ///  1 ------------ 4
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
        ///      6 ------------ 7
        ///    / |            / |
        ///  5 ------------ 8   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |  2 ----------|-- 3
        ///  |/             | /
        ///  1 ------------ 4
        ///  
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            Vector3 vK1 = k - _p1;
            float tx = ExtVector3.DotProduct(vK1, _v41) / _v41Squared;
            float ty = ExtVector3.DotProduct(vK1, _v51) / _v51Squared;
            float tz = ExtVector3.DotProduct(vK1, _v21) / _v21Squared;

            tx = ExtMathf.SetBetween(tx, 0, 1);
            ty = ExtMathf.SetBetween(ty, 0, 1);
            tz = ExtMathf.SetBetween(tz, 0, 1);

            Vector3 closestPoint = tx * _v41
                                    + ty * _v51
                                    + tz * _v21
                                    + _p1;

            return (closestPoint);
        }

        ///
        ///      6 ------------ 7
        ///    / |    3       / |
        ///  5 ------------ 8   |       
        ///  |   |          |   |      
        ///  | 5 |     6    | 2 |     ------8-----  
        ///  |   |   1      |   |                   
        ///  |  2 ----------|-- 3                   
        ///  |/       4     | /     |       3      | 
        ///  1 ------------ 4                       
        ///                                         
        ///          6 ------6----- 5 ------2----- 8 -----10----- 7       -       
        ///          |              |              |              |               
        ///          |              |              |              |               
        ///          5      5       1       1      3       2      11       6       |
        ///          |              |              |              |               
        ///          |              |              |              |               
        ///          2 ------7----- 1 ------4----- 4 ------12---- 3       -
        ///                                         
        ///                                         
        ///                         |       4      |  
        ///                                         
        ///                                         
        ///                           ------9-----       
        public bool GetClosestPointIfWeCan(Vector3 k, GravityOverrideCube gravityOverrideCube, out Vector3 closestPoint)
        {

            float tx = ExtVector3.DotProduct(k - _p1, _v41) / _v41Squared;
            float ty = ExtVector3.DotProduct(k - _p1, _v51) / _v51Squared;
            float tz = ExtVector3.DotProduct(k - _p1, _v21) / _v21Squared;

            tx = tx < 0 ? 0 : tx > 1 ? 1 : tx;
            ty = ty < 0 ? 0 : ty > 1 ? 1 : ty;
            tz = tz < 0 ? 0 : tz > 1 ? 1 : tz;

            closestPoint = tx * _v41 + ty * _v51 + tz * _v21 + _p1;

            bool canApply = CanApplyFaceCube(closestPoint, gravityOverrideCube);
            return (canApply);
        }

        private bool CanApplyFaceCube(Vector3 T, GravityOverrideCube cube)
        {
            if (!cube.Face1 && CanApplyFaceZ(T, _p1, _p5, _p4)) { return (false); } //Face 1        (Z)
            if (!cube.Face2 && CanApplyFaceX(T, _p4, _p8, _p3)) { return (false); } //Face 2 (X)
            if (!cube.Face3 && CanApplyFaceY(T, _p5, _p6, _p8)) { return (false); } //Face 3    (Y)
            if (!cube.Face4 && CanApplyFaceY(T, _p2, _p1, _p3)) { return (false); } //Face 4    (Y)
            if (!cube.Face5 && CanApplyFaceX(T, _p2, _p6, _p1)) { return (false); } //Face 5 (X)
            if (!cube.Face6 && CanApplyFaceZ(T, _p3, _p7, _p2)) { return (false); } //Face 6        (Z)

            if (!cube.Point1 && T == _p1) { return (false); } //Point 1
            if (!cube.Point2 && T == _p2) { return (false); } //Point 2
            if (!cube.Point3 && T == _p3) { return (false); } //Point 3
            if (!cube.Point4 && T == _p4) { return (false); } //Point 4
            if (!cube.Point5 && T == _p5) { return (false); } //Point 5
            if (!cube.Point6 && T == _p6) { return (false); } //Point 6
            if (!cube.Point7 && T == _p7) { return (false); } //Point 7
            if (!cube.Point8 && T == _p8) { return (false); } //Point 8

            if (!cube.Line1 && CanApplyLineZ(T, _p1, _p5)) { return (false); }   //line 1           (Z)
            if (!cube.Line2 && CanApplyLineY(T, _p5, _p8)) { return (false); }   //line 2       (Y)
            if (!cube.Line3 && CanApplyLineZ(T, _p8, _p4)) { return (false); }   //line 3           (Z)
            if (!cube.Line4 && CanApplyLineY(T, _p1, _p4)) { return (false); }   //line 4       (Y)
            if (!cube.Line5 && CanApplyLineZ(T, _p2, _p6)) { return (false); }   //line 5           (Z)
            if (!cube.Line6 && CanApplyLineX(T, _p6, _p5)) { return (false); }   //line 6   (X)
            if (!cube.Line7 && CanApplyLineX(T, _p1, _p2)) { return (false); }   //line 7   (X)
            if (!cube.Line8 && CanApplyLineY(T, _p7, _p6)) { return (false); }   //line 8       (Y)
            if (!cube.Line9 && CanApplyLineY(T, _p3, _p2)) { return (false); }   //line 9       (Y)
            if (!cube.Line10 && CanApplyLineX(T, _p7, _p8)) { return (false); }   //line 10 (X)
            if (!cube.Line11 && CanApplyLineZ(T, _p3, _p7)) { return (false); }   //line 11         (Z)
            if (!cube.Line12 && CanApplyLineX(T, _p4, _p3)) { return (false); }   //line 12 (X)

            return (true);
        }

        private bool CanApplyLineX(Vector3 T, Vector3 CA, Vector3 CB)
        {
            if (T.z == CB.z || T.z == CA.z) { return (false); }
            return (true);
        }

        private bool CanApplyLineY(Vector3 T, Vector3 CA, Vector3 CB)
        {
            if (T.x == CB.x || T.x == CA.x) { return (false); }
            return (true);
        }

        private bool CanApplyLineZ(Vector3 T, Vector3 CA, Vector3 CB)
        {
            if (T.y == CB.y || T.y == CA.y) { return (false); }
            return (true);
        }

        private bool CanApplyFaceZ(Vector3 T, Vector3 CA, Vector3 CB, Vector3 CC)
        {
            //Debug.Log("Face 1, T:" + T + ", CA: " + CA + ", CB: " + CB + ", CC: " + CC);
            //ExtDrawGuizmos.DebugWireSphere(T, 0.1f);
            //ExtDrawGuizmos.DebugWireSphere(CA, 0.1f);
            //ExtDrawGuizmos.DebugWireSphere(CB, 0.1f);
            //ExtDrawGuizmos.DebugWireSphere(CC, 0.1f);

            if      (T.y == CA.y || T.y == CB.y) { return (false); }
            else if (T.x == CA.x || T.x == CC.x) { return (false); }
            return (true);
        }

        private bool CanApplyFaceY(Vector3 T, Vector3 CA, Vector3 CB, Vector3 CC)
        {
            if      (T.x == CA.x || T.x == CC.x) { return (false); }
            else if (T.z == CA.z || T.z == CB.z) { return (false); }
            return (true);
        }

        private bool CanApplyFaceX(Vector3 T, Vector3 CA, Vector3 CB, Vector3 CC)
        {
            if      (T.y == CA.y || T.y == CB.y) { return (false); }
            else if (T.z == CA.z || T.z == CC.z) { return (false); }
            return (true);
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

        /// <summary>
        /// from a given point in space, order the face, from the closest to the farrest from the point
        /// 
        ///         cube
        ///      6 ------------ 7
        ///    / |    3       / |
        ///  5 ------------ 8   | 
        ///  |   |          |   |   -----------  point
        ///  | 5 |     6    | 2 | 
        ///  |   |   1      |   | 
        ///  |  2 ----------|-- 3 
        ///  |/       4     | /   
        ///  1 ------------ 4     
        /// </summary>
        /// <returns></returns>
        public static FloatRange[] GetOrdersOfFaceFromPoint(ExtCube cube, Vector3 point)
        {
            FloatRange[] faceDistance = new FloatRange[6];

            faceDistance[0] = new FloatRange(1, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P1, cube.P5, cube.P8, cube.P4), point));
            faceDistance[1] = new FloatRange(2, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P4, cube.P8, cube.P7, cube.P3), point));
            faceDistance[2] = new FloatRange(3, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P5, cube.P6, cube.P7, cube.P8), point));
            faceDistance[3] = new FloatRange(4, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P1, cube.P2, cube.P3, cube.P4), point));
            faceDistance[4] = new FloatRange(5, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P2, cube.P6, cube.P5, cube.P1), point));
            faceDistance[5] = new FloatRange(6, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P3, cube.P7, cube.P6, cube.P2), point));

            faceDistance = FloatRange.Sort(faceDistance);
            return (faceDistance);
        }

        //end class
    }
    //end nameSpace
}