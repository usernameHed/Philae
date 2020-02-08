using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
    [Serializable]
    public struct ExtSphereTransform
    {
        public float Radius;
        public Transform Transform;

        public ExtSphereTransform(Transform transform, float radius)
        {
            Transform = transform;
            Radius = radius;
        }

        public void Draw(Color color)
        {
            ExtDrawGuizmos.DebugWireSphere(Transform.position, color, Radius);
        }

        /// <summary>
        /// return true if the position is inside the sphape
        /// </summary>
        /// <param name="otherPosition">position to test</param>
        /// <returns>true if inside the shape</returns>
        public bool IsInsideShape(Vector3 otherPosition)
        {
            return (ExtVector3.Distance(otherPosition, Transform.position) <= Radius);
        }
    }
}