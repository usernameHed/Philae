using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.gravityOverride;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace hedCommon.geometry.shape3d
{
    [Serializable]
    public struct ExtDonut
    {
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
        private ExtCircle _circle;

        [SerializeField]
        private float _radius;
        public float Radius { get { return (_radius); } }
        private float _radiusSquared;
        [SerializeField]
        private float _thickNess;
        [SerializeField]
        private float _realThickNess;
        public float ThickNess { get { return (_thickNess); } }
        [SerializeField]
        private float _realRadius;
        [SerializeField]
        private float _realSquaredRadius;
        public float RealRadius { get { return (_realRadius); } }

        private Matrix4x4 _donutMatrix;

        public ExtDonut(Vector3 position,
            Quaternion rotation,
            Vector3 localScale,
            float radius,
            float thickNess) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _radius = radius;
            _thickNess = thickNess;
            _realThickNess = _thickNess * MaxXY(_localScale);
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _donutMatrix = Matrix4x4.TRS(_position, _rotation, _localScale * _radius);
            _circle.MoveSphape(_position, _donutMatrix.UpFast(), _realRadius);
        }

        public float MaxXY(Vector3 size)
        {
            return (Mathf.Max(size.x, size.z));
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _realThickNess = _thickNess * MaxXY(_localScale);
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale, float radius, float thickNess)
        {
            _radius = radius;
            _thickNess = thickNess;
            MoveSphape(position, rotation, localScale);
        }

        public void ChangeRadius(float radius)
        {
            _radius = radius;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public void ChangeThickNess(float thickNess)
        {
            _thickNess = thickNess;
            _realThickNess = _thickNess * MaxXY(_localScale);
            UpdateMatrix();
        }

        /// <summary>
        /// return true if the position is inside the sphape
        /// </summary>
        public bool IsInsideShape(Vector3 k)
        {
            return (_circle.IsInsideShape(k));
        }

        /// <summary>
        /// Return the closest point on the surface of the cylinder
        /// https://diego.assencio.com/?index=ec3d5dfdfc0b6a0d147a656f0af332bd
        ///
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            Vector3 closestPoint = _circle.GetClosestPointOnCircle(k);
            return (closestPoint + (k - closestPoint).FastNormalized() * _realThickNess);
        }

        /*
        public bool GetClosestPointIfWeCan(Vector3 k, GravityOverrideDisc gravityOverride, out Vector3 closestPoint)
        {
            closestPoint = Vector3.zero;
            if (!gravityOverride.CanApplyGravity)
            {
                return (false);
            }
            return (_circle.GetClosestPointOnDiscIfWeCan(k, gravityOverride, out closestPoint));
        }
        */

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            _circle.DrawWithExtraSize(color, _realThickNess, false, "");
            _circle.DrawWithExtraSize(color, -_realThickNess, false, "");

            _circle.DrawWithExtraOffset(color, new Vector3(0, _thickNess * 2f, 0), false, "");
            _circle.DrawWithExtraOffset(color, new Vector3(0, -_thickNess * 2f, 0), false, "");

            ExtDrawGuizmos.DrawCircle(_circle.Point + _donutMatrix.RightNormalized() * _realRadius, _donutMatrix.ForwardFast(), color, _realThickNess, false, "");
            ExtDrawGuizmos.DrawCircle(_circle.Point + _donutMatrix.LeftNormalized() * _realRadius, _donutMatrix.ForwardFast(), color, _realThickNess, false, "");
            ExtDrawGuizmos.DrawCircle(_circle.Point + _donutMatrix.ForwardNormalized() * _realRadius, _donutMatrix.RightFast(), color, _realThickNess, false, "");
            ExtDrawGuizmos.DrawCircle(_circle.Point + _donutMatrix.BackwardNormalized() * _realRadius, _donutMatrix.RightFast(), color, _realThickNess, false, "");

            Vector3 right = (_donutMatrix.RightNormalized() + _donutMatrix.ForwardNormalized()).FastNormalized();
            Vector3 forward = (_donutMatrix.ForwardNormalized() + _donutMatrix.LeftNormalized()).FastNormalized();

            ExtDrawGuizmos.DrawCircle(_circle.Point + right * _realRadius, forward, color, _realThickNess, false, "");
            ExtDrawGuizmos.DrawCircle(_circle.Point - right * _realRadius, forward, color, _realThickNess, false, "");
            ExtDrawGuizmos.DrawCircle(_circle.Point + forward * _realRadius, right, color, _realThickNess, false, "");
            ExtDrawGuizmos.DrawCircle(_circle.Point - forward * _realRadius, right, color, _realThickNess, false, "");
        }
#endif
        //end class
    }
    //end nameSpace
}