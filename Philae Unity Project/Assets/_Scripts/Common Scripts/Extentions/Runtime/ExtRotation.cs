using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtRotation
    {
        /// <summary>
        /// Rotate a Point around an axis
        /// usage:
        /// Vector3 position = ExtRotation.RotatePointAroundAxis(pivotPosition, pointToRotate, pivotUp, _rotateAxis * TimeEditor.deltaTime);
        /// 
        /// for a given A, R, U, rotation(x, y, z):
        /// 
        /// 
        ///   AR: vector director  
        ///   A: anchor            
        ///   U: up (AU)                                 [Pic 1]                        [Pic 2]
        ///   R: to rotate                              vector director
        ///   R': R projected                   U              ⭣                   U                   
        ///   R'': R' rotated                   |              ⭣                   |
        ///   Rfinal: R rotated                 |           ⭢⭢⭢⭢R                |              ⭢⭢⭢R
        ///                                     |       ⭧                          |              ⭡    
        ///                                     A⭢⭢⭢⭢⭢⭢⭢⭢⭢⭢⭢R'              A⭢⭢⭢⭢⭢⭢⭢⭢⭢⭢⭢R'
        ///                                    /           ⭡                      /  ⭨   Rf           ⭣
        ///                                   /        projectedAR               /     ⭨ |          ⭩      rotation  (0x, 45y, 0z)
        ///                                  /                                  /        R''     ⭠
        /// </summary>
        /// <param name="pivotPoint"></param>
        /// <param name="pointToRotate"></param>
        /// <param name="upNormalized"></param>
        /// <param name="rotationAxis"></param>
        /// <returns></returns>
        public static Vector3 RotatePointAroundAxis(Vector3 pivotPoint, Vector3 pointToRotate, Vector3 upNormalized, Vector3 rotationAxis)
        {
            Vector3 vectorDirector = pointToRotate - pivotPoint;                                       //get the vector director
            Vector3 finalPoint = RotateWithMatrix(pivotPoint, vectorDirector, upNormalized, rotationAxis);
            return (finalPoint);
        }

        /// <summary>
        /// Rotate a vectorDirector around an axis
        /// usage:
        /// Vector3 vectorDirector = ExtRotation.RotateVectorAroundAxis(pivotPoint, vectorDirector, pivotUp, _rotateAxis * TimeEditor.deltaTime);
        /// </summary>
        public static Vector3 RotateVectorAroundAxis(Vector3 pivotPoint, Vector3 vectorDirector, Vector3 upNormalized, Vector3 rotationAxis)
        {
            Vector3 finalPoint = RotateWithMatrix(pivotPoint, vectorDirector, upNormalized, rotationAxis);
            return (finalPoint - pivotPoint);
        }

        private static Vector3 RotateWithMatrix(Vector3 pivotPoint, Vector3 vectorDirector, Vector3 upNormalized, Vector3 rotationAxis)
        {
            Quaternion constrainRotation = TurretLookRotation(vectorDirector, upNormalized);            //constrain rotation from up !!
            //create a TRS matrix from point & rotation
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(pivotPoint, constrainRotation, Vector3.one);

            Vector3 projectedForward = ExtVector3.ProjectAOnB(vectorDirector, rotationMatrix.ForwardFast());
            Vector3 projectedUp = ExtVector3.ProjectAOnB(vectorDirector, upNormalized);
            float distanceForward = projectedForward.magnitude;
            float distanceUp = projectedUp.magnitude;

            if (ExtVector3.DotProduct(upNormalized, vectorDirector) < 0)
            {
                distanceUp *= -1;
            }

            //display [Pic 1] axis
            Debug.DrawLine(pivotPoint, pivotPoint + vectorDirector, new Color(1, 1, 1, 0.5f), 1.5f);
            ExtDrawGuizmos.DebugArrowConstant(pivotPoint, upNormalized, new Color(0, 1, 0, 0.2f));
            ExtDrawGuizmos.DebugArrowConstant(pivotPoint, rotationMatrix.ForwardFast(), new Color(0, 0, 1, 0.2f));
            ExtDrawGuizmos.DebugArrowConstant(pivotPoint, rotationMatrix.RightFast(), new Color(1, 0, 0, 0.2f));


            ExtDrawGuizmos.DebugArrowConstant(pivotPoint, projectedForward, new Color(1, 0.92f, 0.016f, 0.2f), 0.15f);
            ExtDrawGuizmos.DebugArrowConstant(pivotPoint + projectedForward, projectedUp, new Color(1, 0.92f, 0.016f, 0.2f), 0.15f);

            //rotate matrix in x, y & z
            rotationMatrix = Matrix4x4.TRS(pivotPoint, constrainRotation * Quaternion.Euler(rotationAxis), Vector3.one);
            Vector3 finalPoint = rotationMatrix.MultiplyPoint3x4(new Vector3(0, distanceUp, distanceForward));

            Debug.DrawLine(pivotPoint, finalPoint, Color.green);
            ExtDrawGuizmos.DebugWireSphere(finalPoint, Color.green, 0.1f);

            ExtDrawGuizmos.DebugArrowConstant(pivotPoint, rotationMatrix.ForwardFast() * distanceForward, Color.yellow, 0.15f);
            ExtDrawGuizmos.DebugArrowConstant(pivotPoint + rotationMatrix.ForwardFast() * distanceForward, projectedUp, Color.yellow, 0.15f);

            ExtDrawGuizmos.DebugArrowConstant(pivotPoint, upNormalized, Color.green);
            ExtDrawGuizmos.DebugArrowConstant(pivotPoint, rotationMatrix.ForwardFast(), Color.blue);
            ExtDrawGuizmos.DebugArrowConstant(pivotPoint, rotationMatrix.RightFast(), Color.red);

            ExtDrawGuizmos.DebugWireSphere(finalPoint, Color.green, 0.01f, 1.5f);
            return finalPoint;
        }

        public static Quaternion RotateVectorDirectorFromAxis(Vector3 vectorDirector, Vector3 upNormalized, Vector3 rotationAxis)
        {
            Quaternion constrainRotation = TurretLookRotation(vectorDirector, upNormalized);            //constrain rotation from up !!
            return (RotateQuaternonFromAxis(constrainRotation, rotationAxis));
        }

        public static Quaternion RotateQuaternonFromAxis(Quaternion rotation, Vector3 rotationAxis)
        {
            return (rotation * Quaternion.Euler(rotationAxis));
        }

        /// <summary>
        /// From a Line, Get the quaternion representing the Vector p2 - p1
        /// the up vector can be Vector3.up if you have no reference.
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <param name="upNormalized">default is Vector3.up</param>
        /// <returns>Quaternion representing the rotation of p2 - p1</returns>
        public static Quaternion QuaternionFromLine(Vector3 p1, Vector3 p2, Vector3 upNormalized)
        {
            Matrix4x4 rotationMatrix = ExtMatrix.LookAt(p1, p2, upNormalized);
            Quaternion rotation = rotationMatrix.ExtractRotation();
            return (rotation);
        }

        /// <summary>
        /// From a Vector director, Get the quaternion representing this vector
        /// the up vector can be Vector3.up if you have no reference.
        /// </summary>
        /// <param name="vectorDirector"></param>
        /// <param name="upNormalized">default is Vector3.up</param>
        /// <returns>Quaternion representing the rotation of p2 - p1</returns>
        public static Quaternion QuaternionFromVectorDirector(Vector3 vectorDirector, Vector3 upNormalized)
        {
            Matrix4x4 rotationMatrix = ExtMatrix.LookAt(vectorDirector, vectorDirector * 2, upNormalized);
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