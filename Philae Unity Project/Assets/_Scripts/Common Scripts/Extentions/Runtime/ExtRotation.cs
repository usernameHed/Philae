using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtRotation
    {
        /// <summary>
        /// ⯇ ⯈ ⯅ ⯆ 🠴 🠶 🠵 🠷 ⭦ ⭧ ⭨ ⭩⭠ ⭢ ⭡ ⭣
        /// 
        ///   AR: vector director  
        ///   A: anchor            
        ///   U: up (AU)                                 [Pic 1]                        [Pic 2]
        ///   R: to rotate                              vector director
        ///                                     U              ⭣                   U                   
        ///                                     |              ⭣                   |
        ///                                     |           ⭢⭢⭢⭢R                |              ⭢⭢⭢R
        ///                                     |       ⭧                          |              ⭡    
        ///                                     A⭢⭢⭢⭢⭢⭢⭢⭢⭢⭢⭢R'              A⭢⭢⭢⭢⭢⭢⭢⭢⭢⭢⭢R'
        ///                                    /           ⭡                      /  ⭨   R'''        ⭣
        ///                                   /        projectedAR               /     ⭨ |          ⭩   Rotate  (0x, 45y, 0z)
        ///                                  /                                  /        R''     ⭠
        /// </summary>
        /// <param name="pointAnchor"></param>
        /// <param name="pointToRotate"></param>
        /// <param name="up"></param>
        /// <param name="rotationAxis"></param>
        /// <returns></returns>
        public static Vector3 RotatePointAroundAxis(Vector3 pointAnchor, Vector3 pointToRotate, Vector3 up, Vector3 rotationAxis)
        {
            Vector3 A = pointAnchor;
            Vector3 R = pointToRotate;

            Vector3 AR = R - A;                       //get the vector director

            Quaternion rotation = QuaternionFromVectorDirector(-AR, up);    //transform vector director to a quaternion
            Quaternion constrainRotation = TurretLookRotation(rotation, up);            //constrain rotation from up !!

            //create a TRS matrix from point & rotation
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(pointAnchor, constrainRotation, Vector3.one);

            Vector3 projectedAR = ExtVector3.GetProjectionOfAOnB(AR, rotationMatrix.ForwardFast(), up);
            float distance = projectedAR.magnitude;

            //display [Pic 1] axis
            Debug.DrawRay(pointAnchor, rotationMatrix.ForwardFast(), Color.blue);
            Debug.DrawRay(pointAnchor, rotationMatrix.RightFast(), Color.red);
            Debug.DrawRay(pointAnchor, rotationMatrix.UpFast(), Color.green);

            //rotate matrix in x, y & z
            rotationMatrix = Matrix4x4.TRS(pointAnchor, constrainRotation * Quaternion.Euler(rotationAxis), Vector3.one);

            //display [Pic 2] axis
            Debug.DrawRay(pointAnchor, rotationMatrix.ForwardFast() * distance, Color.red);
            Debug.DrawRay(pointAnchor, rotationMatrix.RightFast() * distance, Color.red);
            Debug.DrawRay(pointAnchor, rotationMatrix.UpFast() * distance, Color.red);

            //find R''
            Vector3 newPosition = pointAnchor + rotationMatrix.ForwardFast() * distance;
            ExtDrawGuizmos.DebugWireSphere(newPosition, 0.1f);

            
            Vector3 newToOldPosition = pointToRotate - newPosition;
            Vector3 projectUp = up;
            Vector3 projected = ExtVector3.GetProjectionOfAOnB(newToOldPosition, projectUp, projectUp);
            Debug.DrawRay(newPosition, newToOldPosition);
            Debug.DrawRay(newPosition, projectUp);

            Debug.DrawRay(newPosition, projected, Color.green);

            
            return (newPosition + projected);
        }

        public static Matrix4x4 RotationMatrixAroundAxis(Ray axis, float rotation)
        {
            return Matrix4x4.TRS(-axis.origin, Quaternion.AngleAxis(rotation, axis.direction), Vector3.one)
                 * Matrix4x4.TRS(axis.origin, Quaternion.identity, Vector3.one);
        }

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
        /// <param name="vectorDirector"></param>
        /// <param name="up">default is Vector3.up</param>
        /// <returns>Quaternion representing the rotation of p2 - p1</returns>
        public static Quaternion QuaternionFromVectorDirector(Vector3 vectorDirector, Vector3 up)
        {
            Matrix4x4 rotationMatrix = ExtMatrix.LookAt(vectorDirector, vectorDirector * 2, up);
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

        /// <summary>
        /// Turret lock rotation
        /// https://gamedev.stackexchange.com/questions/167389/unity-smooth-local-rotation-around-one-axis-oriented-toward-a-target/167395#167395
        /// 
        /// Vector3 relativeDirection = mainReferenceObjectDirection.right * dirInput.x + mainReferenceObjectDirection.forward * dirInput.y;
        /// Vector3 up = objectToRotate.up;
        /// Quaternion desiredOrientation = TurretLookRotation(relativeDirection, up);
        ///objectToRotate.rotation = Quaternion.RotateTowards(
        ///                         objectToRotate.rotation,
        ///                         desiredOrientation,
        ///                         turnRate* Time.deltaTime
        ///                        );
        /// </summary>
        public static Quaternion TurretLookRotation(Vector3 approximateForward, Vector3 exactUp)
        {
            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -approximateForward);
            Quaternion rotateYToZ = Quaternion.Euler(90f, 0f, 0f);

            return rotateZToUp * rotateYToZ;
        }
        public static Quaternion TurretLookRotation(Quaternion approximateForward, Vector3 exactUp)
        {
            Vector3 forwardQuaternion = approximateForward.GetForwardVector();

            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -forwardQuaternion);
            Quaternion rotateYToZ = Quaternion.Euler(90f, 0f, 0f);

            return rotateZToUp * rotateYToZ;
        }
        public static Vector3 TurretLookRotationVector(Vector3 approximateForward, Vector3 exactUp)
        {
            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -approximateForward);
            Quaternion rotateYToZ = Quaternion.Euler(90f, 0f, 0f);

            return (rotateZToUp * rotateYToZ) * Vector3.forward;
        }

        public static Quaternion TurretLookRotation2D(Vector3 approximateForward, Vector3 exactUp)
        {
            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, approximateForward);
            Quaternion rotateYToZ = Quaternion.Euler(0, 0f, 0f);

            return rotateZToUp * rotateYToZ;
        }
        public static Vector3 TurretLookRotation2DVector(Vector3 approximateForward, Vector3 exactUp)
        {
            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, approximateForward);
            Quaternion rotateYToZ = Quaternion.Euler(0, 0f, 0f);

            return (rotateZToUp * rotateYToZ) * Vector3.forward;
        }

        public static Quaternion SmoothTurretLookRotation(Vector3 approximateForward, Vector3 exactUp,
            Quaternion objCurrentRotation, float maxDegreesPerSecond)
        {
            Quaternion desiredOrientation = TurretLookRotation(approximateForward, exactUp);
            Quaternion smoothOrientation = Quaternion.RotateTowards(
                                        objCurrentRotation,
                                        desiredOrientation,
                                        maxDegreesPerSecond * Time.deltaTime
                                     );
            return (smoothOrientation);
        }
    }
}