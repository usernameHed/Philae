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
    public struct ExtDisc
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

        private float _realRadius;
        private float _realSquaredRadius;
        public float RealRadius { get { return (_realRadius); } }

        private Matrix4x4 _discMatrix;

        public ExtDisc(Vector3 position,
            Quaternion rotation,
            Vector3 localScale,
            float radius) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _radius = radius;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _discMatrix = Matrix4x4.TRS(_position, _rotation, _localScale * _radius);
            _circle.MoveSphape(_position, _discMatrix.UpFast(), _realRadius);
        }

        private float MaxXY(Vector3 size)
        {
            return (Mathf.Max(size.x, size.z));
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale, float radius)
        {
            _radius = radius;
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
        public bool GetClosestPoint(Vector3 k, out Vector3 closestPoint)
        {
            return (_circle.GetClosestPointOnDisc(k, out closestPoint));
        }

        public bool GetClosestPointIfWeCan(Vector3 k, GravityOverrideDisc gravityOverride, out Vector3 closestPoint)
        {
            closestPoint = Vector3.zero;
            if (!gravityOverride.CanApplyGravity)
            {
                return (false);
            }
            return (_circle.GetClosestPointOnDiscIfWeCan(k, gravityOverride, out closestPoint));
        }

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            _circle.Draw(color, true, "");
        }
#endif

#if UNITY_EDITOR
        public void DrawWithExtraSize(Color color, Vector3 extraSize)
        {
            if (extraSize.Maximum() <= 1f)
            {
                return;
            }

            Matrix4x4 cylinderMatrix = Matrix4x4.TRS(_position, _rotation, (_localScale + extraSize) * _radius);
            float realRadius = _radius * MaxXY(_localScale + extraSize);
            ExtCircle circle = new ExtCircle(_position, -cylinderMatrix.UpFast(), realRadius);
            circle.Draw(color, true, "");
        }
#endif
        //end class
    }
    //end nameSpace
}