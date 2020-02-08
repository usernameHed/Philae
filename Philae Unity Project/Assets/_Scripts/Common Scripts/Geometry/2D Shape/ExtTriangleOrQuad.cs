using UnityEngine;

namespace hedCommon.geometry.shape2d
{

    /// <summary>
    /// a perfect quad OR triangle (3 points)
    /// </summary>
    public struct ExtTriangleOrQuad
    {
        public bool unidirectionnal;
        public bool inverseDirection;
        public bool noGravityBorders;
        public bool isQuad;

        public ExtQuad quad;
        public ExtTriangle triangle;

        public ExtTriangleOrQuad(Vector3 a, Vector3 b, Vector3 c,
            bool _unidirectionnal, bool _inverseDirection,
            bool _noGravityBorders, bool _calculateAB, bool _calculateBC, bool _calculateCA, bool _calculateCorner, bool _isQuad)
        {
            unidirectionnal = _unidirectionnal;
            inverseDirection = _inverseDirection;
            noGravityBorders = _noGravityBorders;
            isQuad = _isQuad;

            quad = new ExtQuad(a, b, c, _unidirectionnal, _inverseDirection, _noGravityBorders);
            triangle = new ExtTriangle(a, b, c, _unidirectionnal, _inverseDirection, _noGravityBorders, _calculateAB, _calculateBC, _calculateCA, _calculateCorner);
        }

        public Vector3 GetNormal()
        {
            if (isQuad)
                return (quad.TriNormNormalize);
            return (triangle.TriNormNormalize);
        }

        public Vector3 ClosestPointTo(Vector3 p)
        {
            if (isQuad)
                return (quad.ClosestPtPointRect(p));
            return (triangle.ClosestPointTo(p));
        }
    }
}