using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.gravityOverride;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape2d
{
    /// <summary>
    ///    __-----__
    ///   /         \
    ///  |     1     |
    ///   \         /
    ///    --_____-- 
    /// </summary>
    [Serializable]
    public struct ExtCircle
    {
        [SerializeField]
        private ExtPlane _plane;
        public Vector3 Point { get { return (_plane.Point); } }
        public Vector3 Normal { get { return (_plane.Normal); } }
        [SerializeField]
        private float _radius;
        public bool AllowBottom { get { return (_plane.AllowBottom); } }

        public float Radius { get { return (_radius); } }

        private float _radiusSquared;
        

        public ExtCircle(Vector3 position,
            Vector3 normal,
            float radius) : this()
        {
            _plane.Point = position;
            _plane.Normal = normal;
            _radius = radius;
            _radiusSquared = _radius * _radius;
        }

        public void Draw(Color color, bool displayNormal = false, string index = "1")
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DrawCircle(this, color, _radius, displayNormal, index);
#endif
        }

        public void DrawWithExtraSize(Color color, float extraSize, bool displayNormal = false, string index = "1")
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DrawCircle(this, color, _radius + extraSize, displayNormal, index);
#endif
        }

        public void MoveSphape(Vector3 position, Vector3 normal)
        {
            _plane.MoveShape(position, normal);
        }

        public void MoveSphape(Vector3 position, Vector3 normal, float radius)
        {
            _radius = radius;
            _radiusSquared = _radius * _radius;
            MoveSphape(position, normal);
        }

        public void ChangeRadius(float radius)
        {
            _radius = radius;
            _radiusSquared = _radius * _radius;
        }

        /// <summary>
        /// assume k is NOT on the same plane as the circle, we first do a projection
        /// √( | xp−xc |² +| yp−yc |²) < r
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool IsInsideShapeWithProjection(Vector3 k)
        {
            if (!AllowBottom && !_plane.IsAbove(k))
            {
                return (false);
            }

            Vector3 kProjected = ExtPlane.ProjectPointInPlane(_plane, k);
            float distSquared = (kProjected - _plane.Point).sqrMagnitude;
            return (distSquared < _radiusSquared);
        }

        public bool IsInsideShape(Vector3 k)
        {
            if (!AllowBottom && !_plane.IsAbove(k))
            {
                return (false);
            }

            float distSquared = (k - _plane.Point).sqrMagnitude;
            return (distSquared < _radiusSquared);
        }

        /// <summary>
        /// Return the closest point on the disc from k
        /// </summary>
        public Vector3 GetClosestPointOnDisc(Vector3 k, out bool canApplyGravity)
        {
            canApplyGravity = true;
            if (!AllowBottom && !_plane.IsAbove(k))
            {
                canApplyGravity = false;
            }

            //project point to a plane
            Vector3 kProjected = ExtPlane.ProjectPointInPlane(_plane, k);

            //if dist² < radius², this projected point is inside the circle
            float distSquared = (kProjected - _plane.Point).sqrMagnitude;
            bool isInsideShape = distSquared < _radiusSquared;
            if (isInsideShape)
            {
                return (kProjected);
            }
            //return the closest point on the circle
            Vector3 pointExtremity = _plane.Point + (kProjected - _plane.Point).FastNormalized() * _radius;
            return (pointExtremity);
        }

        /// <summary>
        /// return the closest point on the disc from K, if the GravityInfo permit it
        /// </summary>
        /// <param name="k"></param>
        /// <param name="canApplyGravity"></param>
        /// <param name="gravityDisc"></param>
        /// <returns></returns>
        public Vector3 GetClosestPointOnDiscIfWeCan(Vector3 k, out bool canApplyGravity, GravityOverrideDisc gravityDisc)
        {
            if (!gravityDisc.CanApplyGravity || (!AllowBottom && !_plane.IsAbove(k)))
            {
                canApplyGravity = false;
                return (k);
            }

            canApplyGravity = true;

            //project point to a plane
            Vector3 kProjected = ExtPlane.ProjectPointInPlane(_plane, k);

            //if dist² < radius², this projected point is inside the circle
            float distSquared = (kProjected - _plane.Point).sqrMagnitude;
            bool isInsideShape = distSquared < _radiusSquared;
            if (isInsideShape)
            {
                if (!gravityDisc.Face)
                {
                    canApplyGravity = false;
                    return (k);
                }
                return (kProjected);
            }

            if (!gravityDisc.Borders)
            {
                canApplyGravity = false;
                return (k);
            }

            //return the closest point on the circle
            Vector3 pointExtremity = _plane.Point + (kProjected - _plane.Point).FastNormalized() * _radius;
            return (pointExtremity);
        }

        /// <summary>
        /// return the closest point on the circle from k
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector3 GetClosestPointOnCircle(Vector3 k)
        {
            //project point to a plane
            Vector3 kProjected = ExtPlane.ProjectPointInPlane(_plane, k);

            //return the closest point on the circle
            Vector3 pointExtremity = _plane.Point + (kProjected - _plane.Point).FastNormalized() * _radius;
            return (pointExtremity);
        }

        //end class
    }
    //end nameSpace
}