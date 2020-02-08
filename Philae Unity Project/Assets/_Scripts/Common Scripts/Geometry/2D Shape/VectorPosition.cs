using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape2d
{
    public struct VectorPosition
    {
        public Vector3 Position;
        public Vector3 Force;

        public VectorPosition(Vector3 position, Vector3 direction)
        {
            Position = position;
            Force = direction;
        }
    }
}