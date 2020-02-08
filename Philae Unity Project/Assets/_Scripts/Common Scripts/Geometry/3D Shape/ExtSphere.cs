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

        public ExtSphere(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public void Draw(Color color)
        {
            ExtDrawGuizmos.DebugWireSphere(Position, color, Radius);
        }

        public void MoveSphape(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
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
    }
}