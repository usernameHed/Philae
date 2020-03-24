using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtQuaternion
    {
        public enum TurnType
        {
            X,
            Y,
            Z,
            ALL,
        }

        #region Transform
        /// <summary>
        /// Two quaternions can represent different rotations that lead to the same final orientation (one rotating around Axis with Angle, the other around -Axis with 2Pi-Angle). In this case, the quaternion == operator will return false. This method will return true.
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static bool SameOrientation(this Quaternion q1, Quaternion q2)
        {
            return Mathf.Abs((float)Quaternion.Dot(q1, q2)) > 0.999998986721039;
        }

        /// <summary>
        /// Two quaternions can represent different rotations that lead to the same final orientation (one rotating around Axis with Angle, the other around -Axis with 2Pi-Angle). In this case, the quaternion != operator will return true. This method will return false.
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static bool DifferentOrientation(this Quaternion q1, Quaternion q2)
        {
            return Mathf.Abs((float)Quaternion.Dot(q1, q2)) <= 0.999998986721039;
        }

        /// <summary>
        /// Create a LookRotation for a non-standard 'forward' axis.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="forwardAxis"></param>
        /// <returns></returns>
        public static Quaternion AltForwardLookRotation(Vector3 dir, Vector3 forwardAxis, Vector3 upAxis)
        {
            //return Quaternion.LookRotation(dir, upAxis) * Quaternion.FromToRotation(forwardAxis, Vector3.forward);
            return Quaternion.LookRotation(dir) * Quaternion.Inverse(Quaternion.LookRotation(forwardAxis, upAxis));
        }

        /// <summary>
        /// Get the rotated forward axis based on some base forward.
        /// </summary>
        /// <param name="rot">The rotation</param>
        /// <param name="baseForward">Forward with no rotation</param>
        /// <returns></returns>
        public static Vector3 GetAltForward(Quaternion rot, Vector3 baseForward)
        {
            return rot * baseForward;
        }

        /// <summary>
        /// Returns a rotation of up attempting to face in the general direction of forward.
        /// </summary>
        /// <param name="up"></param>
        /// <param name="targForward"></param>
        /// <returns></returns>
        public static Quaternion FaceRotation(Vector3 forward, Vector3 up)
        {
            forward = ExtVector3.GetForwardTangent(forward, up);
            return Quaternion.LookRotation(forward, up);
        }

        //Returns the forward vector of a quaternion
        public static Vector3 GetForwardVector(this Quaternion q)
        {
            return q * Vector3.forward;
        }

        //This is an alternative for Quaternion.LookRotation. Instead of aligning the forward and up vector of the game 
        //object with the input vectors, a custom direction can be used instead of the fixed forward and up vectors.
        //alignWithVector and alignWithNormal are in world space.
        //customForward and customUp are in object space.
        //Usage: use alignWithVector and alignWithNormal as if you are using the default LookRotation function.
        //Set customForward and customUp to the vectors you wish to use instead of the default forward and up vectors.
        public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
        {

            //Set the rotation of the destination
            Quaternion rotationA = Quaternion.LookRotation(alignWithVector, alignWithNormal);

            //Set the rotation of the custom normal and up vectors. 
            //When using the default LookRotation function, this would be hard coded to the forward and up vector.
            Quaternion rotationB = Quaternion.LookRotation(customForward, customUp);

            //Calculate the rotation
            gameObjectInOut.transform.rotation = rotationA * Quaternion.Inverse(rotationB);
        }

        //Returns the up vector of a quaternion
        public static Vector3 GetUpVector(Quaternion q)
        {
            return q * Vector3.up;
        }

        //Returns the right vector of a quaternion
        public static Vector3 GetRightVector(Quaternion q)
        {
            return q * Vector3.right;
        }

        public static void GetAngleAxis(this Quaternion q, out Vector3 axis, out float angle)
        {
            if (q.w > 1) q = ExtQuaternion.Normalize(q);

            //get as doubles for precision
            var qw = (double)q.w;
            var qx = (double)q.x;
            var qy = (double)q.y;
            var qz = (double)q.z;
            var ratio = System.Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * System.Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }

        public static void GetShortestAngleAxisBetween(Quaternion a, Quaternion b, out Vector3 axis, out float angle)
        {
            var dq = Quaternion.Inverse(a) * b;
            if (dq.w > 1) dq = ExtQuaternion.Normalize(dq);

            //get as doubles for precision
            var qw = (double)dq.w;
            var qx = (double)dq.x;
            var qy = (double)dq.y;
            var qz = (double)dq.z;
            var ratio = System.Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * System.Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }
        #endregion

        /// <summary>
        /// is a quaternion NaN
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static bool IsNaN(Quaternion q)
        {
            return float.IsNaN(q.x * q.y * q.z * q.w);
        }

        public static bool IsClose(this Quaternion q1, Quaternion q2, float range)
        {
            return (ExtVector3.IsClose(q1.eulerAngles, q2.eulerAngles, range));
        }

        /// <summary>
        /// get an array of rotation from a list of transform
        /// </summary>
        public static Quaternion[] GetAllRotation(Transform[] rotations)
        {
            Quaternion[] array = new Quaternion[rotations.Length];
            for (int i = 0; i < rotations.Length; i++)
            {
                array[i] = rotations[i].rotation;
            }
            return (array);
        }

        // assuming qArray.Length > 1
        public static Quaternion AverageQuaternion(Quaternion[] qArray)
        {
            Quaternion qAvg = qArray[0];
            float weight;
            for (int i = 1; i < qArray.Length; i++)
            {
                weight = 1.0f / (float)(i + 1);
                qAvg = Quaternion.Slerp(qAvg, qArray[i], weight);
            }
            return qAvg;
        }

        //Get an average (mean) from more then two quaternions (with two, slerp would be used).
        //Note: this only works if all the quaternions are relatively close together.
        //Usage: 
        //-Cumulative is an external Vector4 which holds all the added x y z and w components.
        //-newRotation is the next rotation to be added to the average pool
        //-firstRotation is the first quaternion of the array to be averaged
        //-addAmount holds the total amount of quaternions which are currently added
        //This function returns the current average quaternion
        public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount)
        {

            float w = 0.0f;
            float x = 0.0f;
            float y = 0.0f;
            float z = 0.0f;

            //Before we add the new rotation to the average (mean), we have to check whether the quaternion has to be inverted. Because
            //q and -q are the same rotation, but cannot be averaged, we have to make sure they are all the same.
            if (!ExtQuaternion.AreQuaternionsClose(newRotation, firstRotation))
            {

                newRotation = ExtQuaternion.InverseSignQuaternion(newRotation);
            }

            //Average the values
            float addDet = 1f / (float)addAmount;
            cumulative.w += newRotation.w;
            w = cumulative.w * addDet;
            cumulative.x += newRotation.x;
            x = cumulative.x * addDet;
            cumulative.y += newRotation.y;
            y = cumulative.y * addDet;
            cumulative.z += newRotation.z;
            z = cumulative.z * addDet;

            //note: if speed is an issue, you can skip the normalization step
            return NormalizeQuaternion(x, y, z, w);
        }

        public static Quaternion OwnSmoothDamp3Axes(Quaternion rot, Quaternion target, ref Vector3 deriv, Vector3 time, Vector3 maxTime, float deltaTime)
        {
            Vector3 lookAtCurrentVector = rot.eulerAngles;// mainCamera.forward;
            Vector3 lookAtWantedVector = target.eulerAngles;

            Vector3 currentDamp = lookAtWantedVector;

            float z = deriv.z;
            float y = deriv.y;
            float x = deriv.x;

            currentDamp.z = ExtQuaternion.SmoothDampAngle(lookAtCurrentVector.z, lookAtWantedVector.z, ref z, time.z, maxTime.z, TimeEditor.DeltaTimeEditorAndRunTime);
            currentDamp.y = ExtQuaternion.SmoothDampAngle(lookAtCurrentVector.y, lookAtWantedVector.y, ref y, time.y, maxTime.y, TimeEditor.DeltaTimeEditorAndRunTime);
            currentDamp.x = ExtQuaternion.SmoothDampAngle(lookAtCurrentVector.x, lookAtWantedVector.x, ref x, time.x, maxTime.x, TimeEditor.DeltaTimeEditorAndRunTime);

            deriv = new Vector3(x, y, z);

            Quaternion dampedRotation = Quaternion.Euler(currentDamp);

            return (dampedRotation);
        }
        private static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            target = current + DeltaAngle(current, target);
            return (ExtVector3.OwnSmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime));
        }

        // Calculates the shortest difference between two given angles.
        public static float DeltaAngle(float current, float target)
        {
            float delta = Mathf.Repeat((target - current), 360.0F);
            if (delta > 180.0F)
                delta -= 360.0F;
            return delta;
        }

        public static Quaternion OwnSmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time, float deltaTime)
        {
            // account for double-cover
            var Dot = Quaternion.Dot(rot, target);
            var Multi = Dot > 0f ? 1f : -1f;
            target.x *= Multi;
            target.y *= Multi;
            target.z *= Multi;
            target.w *= Multi;
            // smooth damp (nlerp approx)
            var Result = new Vector4(
                Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
                Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
                Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
                Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
            ).normalized;
            // compute deriv
            var dtInv = 1f / deltaTime;
            deriv.x = (Result.x - rot.x) * dtInv;
            deriv.y = (Result.y - rot.y) * dtInv;
            deriv.z = (Result.z - rot.z) * dtInv;
            deriv.w = (Result.w - rot.w) * dtInv;
            return new Quaternion(Result.x, Result.y, Result.z, Result.w);
        }

        public static Quaternion NormalizeQuaternion(float x, float y, float z, float w)
        {

            float lengthD = 1.0f / (w * w + x * x + y * y + z * z);
            w *= lengthD;
            x *= lengthD;
            y *= lengthD;
            z *= lengthD;

            return new Quaternion(x, y, z, w);
        }

        //Changes the sign of the quaternion components. This is not the same as the inverse.
        public static Quaternion InverseSignQuaternion(Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, -q.w);
        }

        //Returns true if the two input quaternions are close to each other. This can
        //be used to check whether or not one of two quaternions which are supposed to
        //be very similar but has its component signs reversed (q has the same rotation as
        //-q)
        public static bool AreQuaternionsClose(Quaternion q1, Quaternion q2)
        {

            float dot = Quaternion.Dot(q1, q2);

            if (dot < 0.0f)
            {

                return false;
            }

            else
            {

                return true;
            }
        }

        /// <summary>
        /// Lerp a rotation
        /// </summary>
        /// <param name="currentRotation"></param>
        /// <param name="desiredRotation"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Quaternion LerpRotation(Quaternion currentRotation, Quaternion desiredRotation, float speed)
        {
            return (Quaternion.Lerp(currentRotation, desiredRotation, Time.time * speed));
        }

        /// <summary>
        /// clamp a quaternion around one local axis
        /// </summary>
        /// <param name="q"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <returns></returns>
        public static Quaternion ClampRotationAroundXAxis(Quaternion q, float minX, float maxX)
        {
            if (q.w == 0)
                return (q);

            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, minX, maxX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        public static Quaternion ClampRotationAroundYAxis(Quaternion q, float minY, float maxY)
        {
            if (q.w == 0)
                return (q);

            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

            angleY = Mathf.Clamp(angleY, minY, maxY);

            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

            return q;
        }

        public static Quaternion ClampRotationAroundZAxis(Quaternion q, float minZ, float maxZ)
        {
            if (q.w == 0)
                return (q);

            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);

            angleZ = Mathf.Clamp(angleZ, minZ, maxZ);

            q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

            return q;
        }

        public static bool IsCloseYToClampAmount(Quaternion q, float minY, float maxY, float margin = 2)
        {
            if (q.w == 0)
                return (true);

            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

            if (angleY.IsClose(valueToCompare: minY, precision: margin)
                || angleY.IsClose(valueToCompare: maxY, precision: margin))
            {
                return (true);
            }
            return (false);
        }
        public static bool IsCloseZToClampAmount(Quaternion q, float minZ, float maxZ, float margin = 2)
        {
            if (q.w == 0)
                return (true);

            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);

            if (angleZ.IsClose(minZ, margin)
                || angleZ.IsClose(maxZ, margin))
            {
                return (true);
            }
            return (false);
        }
        public static bool IsCloseXToClampAmount(Quaternion q, float minX, float maxX, float margin = 2)
        {
            if (q.w == 0)
                return (true);

            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            if (angleX.IsClose(minX, margin)
                || angleX.IsClose(maxX, margin))
            {
                return (true);
            }
            return (false);
        }


        /// <summary>
        /// rotate an object in 2D coordonate
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="dir"></param>
        /// <param name="turnRate"></param>
        /// <param name="typeRotation"></param>
        /// <returns></returns>
        public static Quaternion DirObject2d(this Quaternion rotation, Vector2 dir, float turnRate, out Quaternion targetRotation, TurnType typeRotation = TurnType.Z)
        {
            float heading = Mathf.Atan2(-dir.x * turnRate * Time.deltaTime, dir.y * turnRate * Time.deltaTime);

            targetRotation = Quaternion.identity;

            float x = (typeRotation == TurnType.X) ? heading * 1 * Mathf.Rad2Deg : 0;
            float y = (typeRotation == TurnType.Y) ? heading * -1 * Mathf.Rad2Deg : 0;
            float z = (typeRotation == TurnType.Z) ? heading * -1 * Mathf.Rad2Deg : 0;

            targetRotation = Quaternion.Euler(x, y, z);
            rotation = Quaternion.RotateTowards(rotation, targetRotation, turnRate * Time.deltaTime);
            return (rotation);
        }

        /// Rotates a 2D object to face a target
        /// </summary>
        /// <param name="target">transform to look at</param>
        /// <param name="isXAxisForward">when true, the x axis of the transform is aligned to look at the target</param>
        public static void LookAt2D(this Transform transform, Vector2 target, bool isXAxisForward = true)
        {
            target = target - (Vector2)transform.position;
            float currentRotation = transform.eulerAngles.z;
            if (isXAxisForward)
            {
                if (target.x > 0)
                {
                    transform.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan(target.y / target.x) - currentRotation));
                }
                else if (target.x < 0)
                {
                    transform.Rotate(new Vector3(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan(target.y / target.x) - currentRotation));
                }
                else
                {
                    transform.Rotate(new Vector3(0, 0, (target.y > 0 ? 90 : -90) - currentRotation));
                }
            }
            else
            {
                if (target.y > 0)
                {
                    transform.Rotate(new Vector3(0, 0, -Mathf.Rad2Deg * Mathf.Atan(target.x / target.y) - currentRotation));
                }
                else if (target.y < 0)
                {
                    transform.Rotate(new Vector3(0, 0, 180 - Mathf.Rad2Deg * Mathf.Atan(target.x / target.y) - currentRotation));
                }
                else
                {
                    transform.Rotate(new Vector3(0, 0, (target.x > 0 ? 90 : -90) - currentRotation));
                }
            }
        }

        /// <summary>
        /// rotate smoothly selon 2 axe
        /// </summary>
        public static Quaternion DirObject(this Quaternion rotation, float horizMove, float vertiMove, float turnRate, out Quaternion _targetRotation, TurnType typeRotation = TurnType.Z)
        {
            float heading = Mathf.Atan2(horizMove * turnRate * Time.deltaTime, -vertiMove * turnRate * Time.deltaTime);

            //Quaternion _targetRotation = Quaternion.identity;

            float x = (typeRotation == TurnType.X) ? heading * 1 * Mathf.Rad2Deg : 0;
            float y = (typeRotation == TurnType.Y) ? heading * -1 * Mathf.Rad2Deg : 0;
            float z = (typeRotation == TurnType.Z) ? heading * -1 * Mathf.Rad2Deg : 0;

            _targetRotation = Quaternion.Euler(x, y, z);
            rotation = Quaternion.RotateTowards(rotation, _targetRotation, turnRate * Time.deltaTime);
            return (rotation);
        }

        public static Vector3 DirLocalObject(Vector3 rotation, Vector3 dirToGo, float turnRate, TurnType typeRotation = TurnType.Z)
        {
            Vector3 returnRotation = rotation;
            float x = returnRotation.x;
            float y = returnRotation.y;
            float z = returnRotation.z;

            //Debug.Log("Y current: " + y + ", y to go: " + dirToGo.y);



            x = (typeRotation == TurnType.X || typeRotation == TurnType.ALL) ? Mathf.LerpAngle(returnRotation.x, dirToGo.x, Time.deltaTime * turnRate) : x;
            y = (typeRotation == TurnType.Y || typeRotation == TurnType.ALL) ? Mathf.LerpAngle(returnRotation.y, dirToGo.y, Time.deltaTime * turnRate) : y;
            z = (typeRotation == TurnType.Z || typeRotation == TurnType.ALL) ? Mathf.LerpAngle(returnRotation.z, dirToGo.z, Time.deltaTime * turnRate) : z;

            //= Vector3.Lerp(rotation, dirToGo, Time.deltaTime * turnRate);
            return (new Vector3(x, y, z));
        }

        /// <summary>
        /// rotate un quaternion selon un vectir directeur
        /// use: transform.rotation.LookAtDir((transform.position - target.transform.position) * -1);
        /// </summary>
        public static Quaternion LookAtDir(Vector3 dir)
        {
            Quaternion rotation = Quaternion.LookRotation(dir * -1);
            return (rotation);
        }

        /// <summary>
        /// prend un quaternion en parametre, et retourn une direction selon un repère
        /// </summary>
        /// <param name="quat">rotation d'un transform</param>
        /// <param name="up">Vector3.up</param>
        /// <returns>direction du quaternion</returns>
        public static Vector3 QuaternionToDir(Quaternion quat, Vector3 up)
        {
            return ((quat * up).normalized);
        }

        public static Quaternion DirToQuaternion(Vector3 dir)
        {
            return (Quaternion.Euler(dir));
        }

        /// <summary>
        /// Get the rotation that would be applied to 'start' to end up at 'end'.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Quaternion FromToRotation(Quaternion start, Quaternion end)
        {
            return Quaternion.Inverse(start) * end;
        }

        public static Quaternion SpeedSlerp(Quaternion from, Quaternion to, float angularSpeed, float dt, bool bUseRadians = false)
        {
            if (bUseRadians) angularSpeed *= Mathf.Rad2Deg;
            var da = angularSpeed * dt;
            return Quaternion.RotateTowards(from, to, da);
        }

        //caclulate the rotational difference from A to B
        public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
        {
            Quaternion C = Quaternion.Inverse(A) * B;
            return C;
        }

        //Add rotation B to rotation A.
        public static Quaternion AddRotation(Quaternion A, Quaternion B)
        {
            Quaternion C = A * B;
            return C;
        }

        //Same as the build in TransformDirection(), but using a rotation instead of a transform.
        public static Vector3 TransformDirectionMath(Quaternion rotation, Vector3 vector)
        {

            Vector3 output = rotation * vector;
            return output;
        }

        //Same as the build in InverseTransformDirection(), but using a rotation instead of a transform.
        public static Vector3 InverseTransformDirectionMath(Quaternion rotation, Vector3 vector)
        {
            Vector3 output = Quaternion.Inverse(rotation) * vector;
            return output;
        }

        //Rotate a vector as if it is attached to an object with rotation "from", which is then rotated to rotation "to".
        //Similar to TransformWithParent(), but rotating a vector instead of a transform.
        public static Vector3 RotateVectorFromTo(Quaternion from, Quaternion to, Vector3 vector)
        {
            //Note: comments are in case all inputs are in World Space.
            Quaternion Q = SubtractRotation(to, from);              //Output is in object space.
            Vector3 A = InverseTransformDirectionMath(from, vector);//Output is in object space.
            Vector3 B = Q * A;                                      //Output is in local space.
            Vector3 C = TransformDirectionMath(from, B);            //Output is in world space.
            return C;
        }

        #region misc
        /// <summary>
        /// normalise a quaternion
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Quaternion Normalize(Quaternion q)
        {
            var mag = System.Math.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
            q.w = (float)((double)q.w / mag);
            q.x = (float)((double)q.x / mag);
            q.y = (float)((double)q.y / mag);
            q.z = (float)((double)q.z / mag);
            return q;
        }

        ///returns quaternion raised to the power pow. This is useful for smoothly multiplying a Quaternion by a given floating-point value.
        ///transform.rotation = rotateOffset.localRotation.Pow(Time.time);
        public static Quaternion Pow(this Quaternion input, float power)
        {
            float inputMagnitude = input.Magnitude();
            Vector3 nHat = new Vector3(input.x, input.y, input.z).normalized;
            Quaternion vectorBit = new Quaternion(nHat.x, nHat.y, nHat.z, 0)
                .ScalarMultiply(power * Mathf.Acos(input.w / inputMagnitude))
                    .Exp();
            return vectorBit.ScalarMultiply(Mathf.Pow(inputMagnitude, power));
        }

        ///returns euler's number raised to quaternion
        public static Quaternion Exp(this Quaternion input)
        {
            float inputA = input.w;
            Vector3 inputV = new Vector3(input.x, input.y, input.z);
            float outputA = Mathf.Exp(inputA) * Mathf.Cos(inputV.magnitude);
            Vector3 outputV = Mathf.Exp(inputA) * (inputV.normalized * Mathf.Sin(inputV.magnitude));
            return new Quaternion(outputV.x, outputV.y, outputV.z, outputA);
        }

        ///returns the float magnitude of quaternion
        public static float Magnitude(this Quaternion input)
        {
            return Mathf.Sqrt(input.x * input.x + input.y * input.y + input.z * input.z + input.w * input.w);
        }

        ///returns quaternion multiplied by scalar
        public static Quaternion ScalarMultiply(this Quaternion input, float scalar)
        {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }
        #endregion

        #region String

        public static string Stringify(Quaternion q)
        {
            return q.x.ToString() + "," + q.y.ToString() + "," + q.z.ToString() + "," + q.w.ToString();
        }

        public static string ToDetailedString(this Quaternion v)
        {
            return System.String.Format("<{0}, {1}, {2}, {3}>", v.x, v.y, v.z, v.w);
        }

        #endregion
    }
}