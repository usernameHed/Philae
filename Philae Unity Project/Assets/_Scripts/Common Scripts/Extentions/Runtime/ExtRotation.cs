using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtRotation
    {
        /// <summary>
        /// From a Line, Get the quaternion representing the Vector p2 - p1
        /// the up vector can be Vector3.up if you have no reference.
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <param name="up">default is Vector3.up</param>
        /// <returns>Quaternion representing the rotation of p2 - p1</returns>
        public static Quaternion QuaternionFromLine(Vector3 p1, Vector3 p2, Vector3 up)
        {
            Matrix4x4 rotationMatrix = ExtMatrix.LookAt(p1, p2, up);
            Quaternion rotation = rotationMatrix.ExtractRotation();
            return (rotation);
        }

        /// <summary>
        /// From a Vector director, Get the quaternion representing this vector
        /// the up vector can be Vector3.up if you have no reference.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="up">default is Vector3.up</param>
        /// <returns>Quaternion representing the rotation of p2 - p1</returns>
        public static Quaternion QuaternionFromVectorDirector(Vector3 normal, Vector3 up)
        {
            Matrix4x4 rotationMatrix = ExtMatrix.LookAt(normal, normal * 2, up);
            Quaternion rotation = rotationMatrix.ExtractRotation();
            return (rotation);
        }

        /// <summary>
        /// rotate a given quaternion in x, y and z
        /// </summary>
        /// <param name="currentQuaternion">current quaternion to rotate</param>
        /// <param name="axis">axis of rotation in degree</param>
        /// <returns>new rotated quaternion</returns>
        public static Quaternion RotateQuaternion(Quaternion currentQuaternion, Vector3 axis)
        {
            return (currentQuaternion * Quaternion.Euler(axis));
        }
    }
}