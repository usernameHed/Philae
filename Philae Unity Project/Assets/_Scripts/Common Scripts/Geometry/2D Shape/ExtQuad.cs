using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape2d
{

    /// <summary>
    /// a perfect 3D Quad, with 3 points
    /// </summary>
    public struct ExtQuad
    {
        public readonly Vector3 A;
        public readonly Vector3 B;
        public readonly Vector3 C;
        private Vector3 AB;
        private Vector3 AC;
        private float maxdistA;
        private float maxdistC;

        public bool unidirectionnal;
        public bool inverseDirection;
        public bool noGravityBorders;

        private readonly Vector3 TriNorm;
        public readonly Vector3 TriNormNormalize;

        public ExtQuad(Vector3 a, Vector3 b, Vector3 c,
            bool _unidirectionnal, bool _inverseDirection,
            bool _noGravityBorders)
        {
            A = a;
            B = b;
            C = c;

            AB = B - A;
            AC = C - A;
            maxdistA = Vector3.Dot(AB, AB);
            maxdistC = Vector3.Dot(AC, AC);

            unidirectionnal = _unidirectionnal;
            inverseDirection = _inverseDirection;
            noGravityBorders = _noGravityBorders;

            TriNorm = Vector3.Cross(a - b, a - c);
            TriNormNormalize = TriNorm.normalized;
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

            //if (unidirectionnal)
            //    return (GetGoodPointUnidirectionnal(p, q));

            return (q);
        }
    }

}
