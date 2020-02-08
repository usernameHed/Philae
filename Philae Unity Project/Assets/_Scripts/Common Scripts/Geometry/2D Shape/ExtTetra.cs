using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape2d
{

    /// <summary>
    /// a 3D Tetra: 4 points linked together
    /// </summary>
    public struct ExtTetra
    {
        public readonly Vector3 A;
        public readonly Vector3 B;
        public readonly Vector3 C;
        public readonly Vector3 D;

        private Vector3 BA;
        private Vector3 CA;
        private Vector3 DA;

        private Vector3 crossBACA;

        private ExtTriangle triangleA;
        private ExtTriangle triangleB;
        private ExtTriangle triangleC;
        private ExtTriangle triangleD;

        public bool unidirectionnal;
        public bool inverseDirection;
        public bool noGravityBorders;
        public bool precise;

        private readonly Vector3 TriNorm;
        public readonly Vector3 TriNormNormalize;

        public ExtTetra(Vector3 a, Vector3 b, Vector3 c, Vector3 d,
            bool _unidirectionnal, bool _inverseDirection,
            bool _noGravityBorders, bool _precise)
        {
            A = a;
            B = b;
            C = c;
            D = d;

            BA = B - A;
            CA = C - A;
            DA = D - A;

            crossBACA = Vector3.Cross(B - A, C - A);

            TriNorm = ExtVector3.GetMiddleOf2VectorNormalized(Vector3.Cross(a - b, a - c), Vector3.Cross(a - c, a - d));
            TriNormNormalize = TriNorm.normalized;

            precise = _precise;

            if (precise)
            {
                triangleA = new ExtTriangle(a, b, c, false, false, _noGravityBorders, false, false, true, true);
                triangleB = new ExtTriangle(a, c, d, false, false, _noGravityBorders, true, false, false, true);
                triangleC = new ExtTriangle(a, d, b, false, false, _noGravityBorders, false, true, false, true);
                triangleD = new ExtTriangle(b, d, c, false, false, _noGravityBorders, true, false, false, true);
            }
            else
            {
                triangleA = new ExtTriangle(a, b, c, _unidirectionnal, _inverseDirection, _noGravityBorders, false, false, true, false);
                triangleB = new ExtTriangle(c, d, a, _unidirectionnal, _inverseDirection, _noGravityBorders, false, false, true, false);
                triangleC = new ExtTriangle();
                triangleD = new ExtTriangle();
            }


            unidirectionnal = _unidirectionnal;
            inverseDirection = _inverseDirection;
            noGravityBorders = _noGravityBorders;
        }

        // Test if point p lies outside plane through abc
        bool PointOutsideOfPlane(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Dot(p - a, Vector3.Cross(b - a, c - a)) >= 0.0f; // [AP AB AC] >= 0
        }

        // Test if point p and d lie on opposite sides of plane through abc
        bool PointOutsideOfPlane(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            float signp = Vector3.Dot(p - a, Vector3.Cross(b - a, c - a)); // [AP AB AC]
            float signd = Vector3.Dot(d - a, Vector3.Cross(b - a, c - a)); // [AD AB AC]
                                                                           // Points on opposite sides if expression signs are opposite
            return signp * signd < 0.0f;
        }

        public Vector3 CalculateSmothlyFourPlane(Vector3 p)
        {
            // Start out assuming point inside all halfspaces, so closest to itself
            Vector3 closestPt = p;
            float bestSqDist = float.MaxValue;
            bool insideAll = true;
            // If point outside face abc then compute closest point on abc
            if (PointOutsideOfPlane(p, A, B, C))
            {
                insideAll = false;
                Vector3 q = triangleA.ClosestPointTo(p);
                if (!ExtVector3.IsNullVector(q))
                {
                    float sqDist = Vector3.Dot(q - p, q - p);
                    // Update best closest point if (squared) distance is less than current best
                    if (sqDist < bestSqDist)
                    {
                        bestSqDist = sqDist;
                        closestPt = q;
                    }
                }
            }
            // Repeat test for face acd
            if (PointOutsideOfPlane(p, A, C, D))
            {
                insideAll = false;
                Vector3 q = triangleB.ClosestPointTo(p);
                if (!ExtVector3.IsNullVector(q))
                {
                    float sqDist = Vector3.Dot(q - p, q - p);
                    if (sqDist < bestSqDist)
                    {
                        bestSqDist = sqDist;
                        closestPt = q;
                    }
                }
            }

            // Repeat test for face adb
            if (PointOutsideOfPlane(p, A, D, B))
            {
                insideAll = false;
                Vector3 q = triangleC.ClosestPointTo(p);
                if (!ExtVector3.IsNullVector(q))
                {
                    float sqDist = Vector3.Dot(q - p, q - p);
                    if (sqDist < bestSqDist)
                    {
                        bestSqDist = sqDist;
                        closestPt = q;
                    }
                }
            }
            // Repeat test for face bdc
            if (PointOutsideOfPlane(p, B, D, C))
            {
                insideAll = false;
                Vector3 q = triangleD.ClosestPointTo(p);
                if (!ExtVector3.IsNullVector(q))
                {
                    float sqDist = Vector3.Dot(q - p, q - p);
                    if (sqDist < bestSqDist)
                    {
                        bestSqDist = sqDist;
                        closestPt = q;
                    }
                }
            }

            if (unidirectionnal)
                return (GetGoodPointUnidirectionnal(p, closestPt));

            if (noGravityBorders && !insideAll && closestPt == p)
                return (ExtVector3.GetNullVector());

            return closestPt;
        }

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
                //Debug.DrawRay(p, dirPlayer, Color.yellow, 5f);
                //Debug.DrawRay(p, TriNorm.normalized, Color.black, 5f);
                return (ExtVector3.GetNullVector());
            }
        }

        public Vector3 CalculateWithTwoTriangle(Vector3 p)
        {
            Vector3 closestToA = triangleA.ClosestPointTo(p);
            Vector3 closestToB = triangleB.ClosestPointTo(p);

            //aditional test when no Gravity Border
            if (noGravityBorders)
            {
                //if A is nul and not B. return null IF B is from the middle !
                if (ExtVector3.IsNullVector(closestToA) && !ExtVector3.IsNullVector(closestToB))
                {
                    if (triangleB.GetLastType() == ExtTriangle.LastType.CA)
                        return (ExtVector3.GetNullVector());
                }
                //if B is nul and not A. return null IF A is from the middle !
                if (ExtVector3.IsNullVector(closestToB) && !ExtVector3.IsNullVector(closestToA))
                {
                    if (triangleA.GetLastType() == ExtTriangle.LastType.CA)
                        return (ExtVector3.GetNullVector());
                }
            }
            if (unidirectionnal)
            {
                //if A is nul and not B. return null IF B is from the middle !
                if (ExtVector3.IsNullVector(closestToA) && !ExtVector3.IsNullVector(closestToB))
                {
                    if (triangleB.GetLastType() == ExtTriangle.LastType.CA || triangleB.GetLastType() == ExtTriangle.LastType.C || triangleB.GetLastType() == ExtTriangle.LastType.A)
                        return (ExtVector3.GetNullVector());
                }
                //if B is nul and not A. return null IF A is from the middle !
                if (ExtVector3.IsNullVector(closestToB) && !ExtVector3.IsNullVector(closestToA))
                {
                    if (triangleA.GetLastType() == ExtTriangle.LastType.CA || triangleA.GetLastType() == ExtTriangle.LastType.C || triangleA.GetLastType() == ExtTriangle.LastType.A)
                        return (ExtVector3.GetNullVector());
                }
            }

            int indexFound = -1;
            return (ExtMathf.GetClosestPoint(p, new Vector3[] { closestToA, closestToB }, ref indexFound));
        }

        // Return point q on (or in) rect (specified by a, b, and c), closest to given point p
        public Vector3 ClosestPtPointRect(Vector3 p)
        {
            if (precise)
                return (CalculateSmothlyFourPlane(p));
            return (CalculateWithTwoTriangle(p));
        }
    }

}