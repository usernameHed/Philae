using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
    [Serializable]
    public struct ExtSphere
    {
        public float Radius;
        public Vector3 Position;
        public Vector3 LocalScale;

        [SerializeField]
        private float _realRadius;
        public float RealRadius { get { return (_realRadius); } }

        public ExtSphere(Vector3 position, float radius) : this()
        {
            MoveSphape(position, Vector3.one, radius);
        }

        public ExtSphere(Vector3 position, Vector3 scale, float radius) : this()
        {
            MoveSphape(position, scale, radius);
        }

        public void Draw(Color color)
        {
            ExtDrawGuizmos.DebugWireSphere(Position, color, _realRadius);
        }

        public void DrawWithExtraRadius(Color color, float radius)
        {
            ExtDrawGuizmos.DebugWireSphere(Position, color, _realRadius + radius);
        }

        public void MoveSphape(Vector3 position, Vector3 scale, float radius)
        {
            Position = position;
            LocalScale = scale;
            Radius = radius;
            _realRadius = radius * LocalScale.Maximum();
        }

        public void MoveSphape(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
            _realRadius = radius * LocalScale.Maximum();
        }

        /// <summary>
        /// return true if the position is inside the sphape
        /// </summary>
        /// <param name="otherPosition">position to test</param>
        /// <returns>true if inside the shape</returns>
        public bool IsInsideShape(Vector3 otherPosition)
        {
            return (ExtVector3.Distance(otherPosition, Position) <= Radius);
        }

        public Vector3 GetClosestPoint(Vector3 k)
        {
            return (Position + ((k - Position).FastNormalized() * Radius));
        }
    }
}