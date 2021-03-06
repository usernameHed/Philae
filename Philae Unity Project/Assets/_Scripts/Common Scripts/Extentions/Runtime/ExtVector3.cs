﻿using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace hedCommon.extension.runtime
{
    public static class ExtVector3
    {
        public static readonly Vector3[] GeneralDirections = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward, Vector3.left, Vector3.down, Vector3.back };

        public enum Axis2D
        {
            IGNORE_X = 0,
            IGNORE_Y = 1,
            IGNORE_Z = 2,
        }

        public static float Distance2D(Vector3 point1, Vector3 point2, Axis2D ignoreAxis)
        {
            switch (ignoreAxis)
            {
                case Axis2D.IGNORE_X:
                    return (Vector2.Distance(new Vector2(point1.y, point1.z), new Vector2(point2.y, point2.z)));
                case Axis2D.IGNORE_Y:
                    return (Vector2.Distance(new Vector2(point1.x, point1.z), new Vector2(point2.x, point2.z)));
                case Axis2D.IGNORE_Z:
                default:
                    return (Vector2.Distance(new Vector2(point1.x, point1.y), new Vector2(point2.x, point2.y)));
            }
        }

        #region nullVector
        /// <summary>
        /// define a null vector
        /// </summary>
        private static Vector3 wrongVector = new Vector3(0.042f, 0, 0);

        /// <summary>
        /// return a null vector
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetNullVector()
        {
            return (wrongVector);
        }
        public static bool IsNullVector(Vector3 vecToTest)
        {
            return (vecToTest == wrongVector);
        }
        /// <summary>
        /// create/fill an array of size lenght with null vector
        /// </summary>
        public static Vector3[] CreateNullVectorArray(int lenght)
        {
            Vector3[] arrayPoints = new Vector3[lenght];
            FillArrayWithWrongVector(ref arrayPoints);
            return (arrayPoints);
        }
        public static void FillArrayWithWrongVector(ref Vector3[] arrayToFill)
        {
            for (int i = 0; i < arrayToFill.Length; i++)
            {
                arrayToFill[i] = GetNullVector();
            }
        }
        #endregion

        #region misc

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="currentVelocity"></param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster., in 3 separate axis</param>
        /// <param name="maxSpeed"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static Vector3 OwnSmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, Vector3 smoothTime, Vector3 maxSpeed, float deltaTime)
        {
            float z = currentVelocity.z;
            float y = currentVelocity.y;
            float x = currentVelocity.x;
            Vector3 smoothedVector = new Vector3(
                OwnSmoothDamp(current.x, target.x, ref x, smoothTime.x, maxSpeed.x, deltaTime),
                OwnSmoothDamp(current.y, target.y, ref y, smoothTime.y, maxSpeed.y, deltaTime),
                OwnSmoothDamp(current.z, target.z, ref z, smoothTime.z, maxSpeed.z, deltaTime));

            currentVelocity = new Vector3(x, y, z);
            return (smoothedVector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="currentVelocity"></param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
        /// <param name="maxSpeed"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static float OwnSmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = Mathf.Max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
            float change = current - target;
            float originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            change = Mathf.Clamp(change, -maxChange, maxChange);
            target = current - change;

            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            float output = target + (change + temp) * exp;

            // Prevent overshooting
            if (originalTo - current > 0.0F == output > originalTo)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        public static Vector3 SmoothDampUsingLastPastTarget(Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float speed)
        {
            float t = Time.deltaTime * speed;
            Vector3 v = (targetPosition - pastTargetPosition) / t;
            Vector3 f = pastPosition - pastTargetPosition + v;
            return targetPosition - v + f * Mathf.Exp(-t);
        }

        /// <summary>
        /// has a target reach a position in space ?
        /// </summary>
        /// <param name="objectMoving"></param>
        /// <param name="target"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        public static bool HasReachedTargetPosition(Vector3 objectMoving, Vector3 target, float margin = 0)
        {
            float x = objectMoving.x;
            float y = objectMoving.y;
            float z = objectMoving.z;

            return (x > target.x - margin
                && x < target.x + margin
                && y > target.y - margin
                && y < target.y + margin
                && z > target.z - margin
                && z < target.z + margin);
        }


        public static Vector3 ClosestDirectionTo(Vector3 direction1, Vector3 direction2, Vector3 targetDirection)
        {
            return (Vector3.Dot(direction1, targetDirection) > Vector3.Dot(direction2, targetDirection)) ? direction1 : direction2;
        }

        /// <summary>
        /// test if a Vector3 is close to another Vector3 (due to floating point inprecision)
        /// compares the square of the distance to the square of the range as this
        /// avoids calculating a square root which is much slower than squaring the range
        /// </summary>
        /// <param name="val"></param>
        /// <param name="about"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsClose(Vector3 val, Vector3 about, float range)
        {
            float close = (val - about).sqrMagnitude;
            return (close < range * range);
        }

        /// <summary>
        /// Direct speedup of <seealso cref="Vector3.Lerp"/>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 v1, Vector3 v2, float value)
        {
            if (value > 1.0f)
                return v2;
            if (value < 0.0f)
                return v1;
            return new Vector3(v1.x + (v2.x - v1.x) * value,
                               v1.y + (v2.y - v1.y) * value,
                               v1.z + (v2.z - v1.z) * value);
        }
        public static Vector3 Sinerp(Vector3 from, Vector3 to, float value)
        {
            value = Mathf.Sin(value * Mathf.PI * 0.5f);
            return Vector3.Lerp(from, to, value);
        }

        /// <summary>
        /// is a vector considered by unity as NaN
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static bool IsNaN(this Vector3 vec)
        {
            return float.IsNaN(vec.x * vec.y * vec.z);
        }

        #endregion

        #region vector calculation
        
        public static Vector3 Sum(params Vector3[] positions)
        {
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < positions.Length; i++)
            {
                sum += positions[i];
            }
            return (sum);
        }

        /// <summary>
        /// divide 2 vector together, the first one is the numerator, the second the denominator
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static Vector3 DivideVectors(Vector3 numerator, Vector3 denominator)
        {
            return (new Vector3(numerator.x / denominator.x, numerator.y / denominator.y, numerator.z / denominator.z));
        }

        /// <summary>
        /// get the max lenght of this vector
        /// </summary>
        /// <returns>min lenght of x, y or z</returns>
        public static float Maximum(this Vector3 vector)
        {
            return ExtMathf.Max(vector.x, vector.y, vector.z);
        }

        public static int Sign(this Vector3 vector)
        {
            float sign = vector.x * vector.y * vector.z;
            if (sign < 0)
            {
                return (-1);
            }
            else if (sign > 0)
            {
                return (1);
            }
            else
            {
                return (0);
            }
        }

        public static Vector3 AddDeltaToPositionFromLocalSpace(Vector3 globalPosition, Vector3 localDelta, Vector3 up, Vector3 forward, Vector3 right)
        {
            Vector3 forwardLocal = forward * localDelta.x;
            Vector3 upLocal = up * localDelta.y;
            Vector3 rightLocal = right * localDelta.z;

            Debug.DrawLine(globalPosition, globalPosition + forwardLocal, Color.blue);
            Debug.DrawLine(globalPosition, globalPosition + upLocal, Color.green);
            Debug.DrawLine(globalPosition, globalPosition + rightLocal, Color.red);



            globalPosition = globalPosition + forwardLocal + upLocal + rightLocal;

            //Vector3 orientedDelta = localDelta.z * right + localDelta.y * up + localDelta.x * forward;
            return (globalPosition);
        }

        /// <summary>
        /// get the min lenght of this vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>min lenght of x, y or z</returns>
        public static float Minimum(this Vector3 vector)
        {
            return ExtMathf.Min(vector.x, vector.y, vector.z);
        }

        /// <summary>
        /// is 2 vectors parallel
        /// </summary>
        /// <param name="direction">vector 1</param>
        /// <param name="otherDirection">vector 2</param>
        /// <param name="precision">test precision</param>
        /// <returns>is parallel</returns>
        public static bool IsParallel(Vector3 direction, Vector3 otherDirection, float precision = .000001f)
        {
            return Vector3.Cross(direction, otherDirection).sqrMagnitude < precision;
        }

        public static Vector3 MultiplyVector(Vector3 one, Vector3 two)
        {
            return new Vector3(one.x * two.x, one.y * two.y, one.z * two.z);
        }

        public static float MagnitudeInDirection(Vector3 vector, Vector3 direction, bool normalizeParameters = true)
        {
            if (normalizeParameters) direction.Normalize();
            return ExtVector3.DotProduct(vector, direction);
        }

        /// <summary>
        /// Absolute value of vector
        /// </summary>
        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            float diff_z = a.z - b.z;
            return (float)Math.Sqrt(diff_x * diff_x + diff_y * diff_y + diff_z * diff_z);
        }

        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            float diff_z = a.z - b.z;
            return (diff_x * diff_x + diff_y * diff_y + diff_z * diff_z);
        }


        /// <summary>
        /// from a given plane (define by a normal), return the projection of a vector
        /// </summary>
        /// <param name="relativeDirection"></param>
        /// <param name="normalPlane"></param>
        /// <returns></returns>
        public static Vector3 ProjectVectorIntoPlane(Vector3 relativeDirection, Vector3 normalPlane)
        {
            //Projection of a vector on a plane and matrix of the projection.
            //http://users.telenet.be/jci/math/rmk.htm

            Vector3 Pprime = Vector3.Project(relativeDirection, normalPlane);
            Vector3 relativeProjeted = relativeDirection - Pprime;
            return (relativeProjeted);
        }


        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/Vector3.Reflect.html
        /// VectorA: input
        /// VectorB: normal
        /// Vector3: result
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetReflectAngle(Vector3 inputVector, Vector3 normalVector)
        {
            return (Vector3.Reflect(inputVector.normalized, normalVector.normalized));
        }


        public static void Reflect(ref Vector2 v, Vector2 normal)
        {
            var dp = 2f * Vector2.Dot(v, normal);
            var ix = v.x - normal.x * dp;
            var iy = v.y - normal.y * dp;
            v.x = ix;
            v.y = iy;
        }

        public static Vector2 Reflect(Vector2 v, Vector2 normal)
        {
            var dp = 2 * Vector2.Dot(v, normal);
            return new Vector2(v.x - normal.x * dp, v.y - normal.y * dp);
        }

        public static void Mirror(ref Vector2 v, Vector2 axis)
        {
            v = (2 * (Vector2.Dot(v, axis) / Vector2.Dot(axis, axis)) * axis) - v;
        }

        public static Vector2 Mirror(Vector2 v, Vector2 axis)
        {
            return (2 * (Vector2.Dot(v, axis) / Vector2.Dot(axis, axis)) * axis) - v;
        }

        /// <summary>
        /// Returns a vector orthogonal to up in the general direction of forward.
        /// </summary>
        /// <param name="up"></param>
        /// <param name="targForward"></param>
        /// <returns></returns>
        public static Vector3 GetForwardTangent(Vector3 forward, Vector3 up)
        {
            return Vector3.Cross(Vector3.Cross(up, forward), up);
        }

        /// <summary>
        /// Dot product de 2 vecteur, retourne négatif si l'angle > 90°, 0 si angle = 90, positif si moin de 90
        /// </summary>
        /// <param name="a">vecteur A</param>
        /// <param name="b">vecteur B</param>
        /// <returns>retourne négatif si l'angle > 90°</returns>
        public static float DotProduct(Vector3 a, Vector3 b)
        {
            return (a.x * b.x + a.y * b.y + a.z * b.z);
        }

        public static float LengthSquared(this Vector3 a)
        {
            return (DotProduct(a, a));
        }

        //This function calculates a signed (+ or - sign instead of being ambiguous) dot product. It is basically used
        //to figure out whether a vector is positioned to the left or right of another vector. The way this is done is
        //by calculating a vector perpendicular to one of the vectors and using that as a reference. This is because
        //the result of a dot product only has signed information when an angle is transitioning between more or less
        //than 90 degrees.
        public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
        {

            Vector3 perpVector;
            float dot;

            //Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
            perpVector = CrossProduct(normal, vectorA);

            //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
            dot = DotProduct(perpVector, vectorB);

            return dot;
        }

        //Calculate the dot product as an angle
        public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
        {

            double dot;
            double angle;

            //get the dot product
            dot = Vector3.Dot(vec1, vec2);

            //Clamp to prevent NaN error. Shouldn't need this in the first place, but there could be a rounding error issue.
            if (dot < -1.0f)
            {
                dot = -1.0f;
            }
            if (dot > 1.0f)
            {
                dot = 1.0f;
            }

            //Calculate the angle. The output is in radians
            //This step can be skipped for optimization...
            angle = Math.Acos(dot);

            return (float)angle;
        }

        /// <summary>
        /// retourne le vecteur de droite au vecteur A, selon l'axe Z
        /// </summary>
        /// <param name="a"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 CrossProduct(Vector3 a, Vector3 z)
        {
            return new Vector3(
                a.y * z.z - a.z * z.y,
                a.z * z.x - a.x * z.z,
                a.x * z.y - a.y * z.x);
        }

        /// <summary>
        /// return if we are right or left from a vector. 1: right, -1: left, 0: forward
        /// </summary>
        /// <param name="forwardDir"></param>
        /// <param name="upDir">up reference of the forward dir</param>
        /// <param name="toGoDir">target direction to test</param>
        public static int IsRightOrLeft(Vector3 forwardDir, Vector3 upDir, Vector3 toGoDir, Vector3 debugPos, ref float dotLeft, ref float dotRight)
        {
            Vector3 left = CrossProduct(forwardDir, upDir);
            Vector3 right = -left;
            dotRight = DotProduct(right, toGoDir);
            dotLeft = DotProduct(left, toGoDir);
            if (dotRight > 0)
            {
                return (1);
            }
            else if (dotLeft > 0)
            {
                return (-1);
            }
            return (0);
        }

        /// <summary>
        /// get mirror of a vector, according to a normal
        /// </summary>
        /// <param name="point">Vector 1</param>
        /// <param name="normal">normal</param>
        /// <returns>vector mirror to 1 (reference= normal)</returns>
        public static Vector3 ReflectionOverPlane(Vector3 point, Vector3 normal)
        {
            return point - 2 * normal * Vector3.Dot(point, normal) / Vector3.Dot(normal, normal);
        }

        public static Vector3 ProjectAOnB(Vector3 A, Vector3 B)
        {
            float sqrMag = DotProduct(B, B);
            if (sqrMag < Mathf.Epsilon)
            {
                return (Vector3.zero);
            }
            else
            {
                var dot = DotProduct(A, B);
                return new Vector3(B.x * dot / sqrMag,
                    B.y * dot / sqrMag,
                    B.z * dot / sqrMag);
            }
        }

        /// <summary>
        /// Return the projection of A on B (with the good magnitude), based on ref (ex: Vector3.up)
        /// </summary>
        public static Vector3 GetProjectionOfAOnB(Vector3 A, Vector3 B, Vector3 refVector, float minMagnitude, float maxMagnitude)
        {
            float angleDegre = SignedAngleBetween(A, B, refVector); //get angle A-B
            angleDegre *= Mathf.Deg2Rad;                            //convert to rad
            float magnitudeX = Mathf.Cos(angleDegre) * A.magnitude; //get magnitude
                                                                    //set magnitude of new Vector
            Vector3 realDir = B.normalized * Mathf.Clamp(Mathf.Abs(magnitudeX), minMagnitude, maxMagnitude) * Mathf.Sign(magnitudeX);
            return (realDir);   //vector A with magnitude based on B
        }

        /// <summary>
        /// return the fast inverse squared of a float, based of the magic number
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public unsafe static float FastInvSqrt(float x)
        {
            float xhalf = 0.5f * x;
            int i = *(int*)&x;
            i = 0x5f375a86 - (i >> 1); //this constant is slightly more accurate than the common one
            x = *(float*)&i;
            x = x * (1.5f - xhalf * x * x);
            return (x);
        }

        /// <summary>
        /// Using the magic of 0x5f3759df
        /// </summary>
        /// <param name="vec1"></param>
        /// <returns></returns>
        public static Vector3 FastNormalized(this Vector3 vec1)
        {
            var componentMult = FastInvSqrt(vec1.sqrMagnitude);
            return new Vector3(vec1.x * componentMult, vec1.y * componentMult, vec1.z * componentMult);
        }

        /// <summary>
        /// Gets the normal of the triangle formed by the 3 vectors
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <param name="vec3"></param>
        /// <returns></returns>
        public static Vector3 GetNormalFromTriangle(Vector3 vec1, Vector3 vec2, Vector3 vec3)
        {
            return Vector3.Cross((vec3 - vec1), (vec2 - vec1));
        }
        #endregion

        #region Get middle & direction
        //Gets an XY direction of magnitude from a radian angle relative to the x axis
        //Simple version
        public static Vector3 GetXYDirection(float angle, float magnitude)
        {
            return (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * magnitude);
        }

        public static float GetHighestAxis(Vector3 axis)
        {
            if (axis.x > axis.y && axis.x > axis.z)
            {
                return (axis.x);
            }
            if (axis.y > axis.x && axis.y > axis.z)
            {
                return (axis.y);
            }
            return (axis.z);
        }

        public static Vector3 GetMiddleOf2VectorNormalized(Vector3 a, Vector3 b)
        {
            return ((a + b).normalized);
        }


        public static Vector3 GetMiddleOfXContactNormal(ContactPoint[] arrayVect)
        {
            Vector3[] arrayTmp = new Vector3[arrayVect.Length];

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < arrayVect.Length; i++)
            {
                arrayTmp[i] = arrayVect[i].normal;
            }
            return (GetMiddleOfXVector(arrayTmp));
        }

        public static Vector3 GetMeanOfXContactPoints(ContactPoint[] arrayContact)
        {
            Vector3[] arrayTmp = new Vector3[arrayContact.Length];

            for (int i = 0; i < arrayContact.Length; i++)
            {
                arrayTmp[i] = arrayContact[i].point;
            }
            return (GetMeanOfXPoints(arrayTmp, out Vector3 sizeBOundingBox, true));
        }

        public static Vector3 GetMiddleOfXVector(Vector3[] arrayVect)
        {
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < arrayVect.Length; i++)
            {
                sum += arrayVect[i];
            }
            return ((sum).normalized);
        }

        public static Vector3 GetMeanOfXPoints(Transform[] arrayVect, out Vector3 sizeBoundingBox, bool middleBoundingBox = true)
        {
            return GetMeanOfXPoints(ExtArray.ToGameObjectsArray(arrayVect), out sizeBoundingBox, middleBoundingBox);
        }

        public static Vector3 GetMeanOfXPoints(GameObject[] arrayVect, out Vector3 sizeBoundingBox, bool middleBoundingBox = true)
        {
            Vector3[] arrayPoint = new Vector3[arrayVect.Length];
            for (int i = 0; i < arrayPoint.Length; i++)
            {
                arrayPoint[i] = arrayVect[i].transform.position;
            }
            return (GetMeanOfXPoints(arrayPoint, out sizeBoundingBox, middleBoundingBox));
        }

        /// <summary>
        /// return the middle of X points (POINTS, NOT vector)
        /// </summary>
        public static Vector3 GetMeanOfXPoints(Vector3[] arrayVect, out Vector3 sizeBoundingBox, bool middleBoundingBox = true)
        {
            return (GetMeanOfXPoints(out sizeBoundingBox, middleBoundingBox, arrayVect));
        }
        public static Vector3 GetMeanOfXPoints(Vector3[] arrayVect, bool middleBoundingBox = true)
        {
            return (GetMeanOfXPoints(arrayVect, out Vector3 sizeBOundingBox, middleBoundingBox));
        }

        public static Vector3 GetMeanOfXPoints(params Vector3[] points)
        {
            return (GetMeanOfXPoints(out Vector3 sizeBoundingBox, false, points));
        }

        public static Vector3 GetMeanOfXPoints(out Vector3 sizeBoundingBox, bool middleBoundingBox = true, params Vector3[] points)
        {
            sizeBoundingBox = Vector2.zero;

            if (points.Length == 0)
            {
                return (ExtVector3.GetNullVector());
            }

            if (!middleBoundingBox)
            {
                Vector3 sum = Vector3.zero;
                for (int i = 0; i < points.Length; i++)
                {
                    sum += points[i];
                }
                return (sum / points.Length);
            }
            else
            {
                if (points.Length == 1)
                    return (points[0]);

                float xMin = points[0].x;
                float yMin = points[0].y;
                float zMin = points[0].z;
                float xMax = points[0].x;
                float yMax = points[0].y;
                float zMax = points[0].z;

                for (int i = 1; i < points.Length; i++)
                {
                    if (points[i].x < xMin)
                        xMin = points[i].x;
                    if (points[i].x > xMax)
                        xMax = points[i].x;

                    if (points[i].y < yMin)
                        yMin = points[i].y;
                    if (points[i].y > yMax)
                        yMax = points[i].y;

                    if (points[i].z < zMin)
                        zMin = points[i].z;
                    if (points[i].z > zMax)
                        zMax = points[i].z;
                }
                Vector3 lastMiddle = new Vector3((xMin + xMax) / 2, (yMin + yMax) / 2, (zMin + zMax) / 2);
                sizeBoundingBox.x = Mathf.Abs(xMin - xMax);
                sizeBoundingBox.y = Mathf.Abs(yMin - yMax);
                sizeBoundingBox.z = Mathf.Abs(zMin - zMax);

                return (lastMiddle);
            }
        }

        

        /// <summary>
        /// get la bisection de 2 vecteur
        /// </summary>
        public static Vector3 GetbisectionOf2Vector(Vector3 a, Vector3 b)
        {
            return ((a + b) * 0.5f);
        }

        /// <summary>
        /// is a vector is in the same direction of another, with a given precision
        /// </summary>
        /// <param name="direction">vector 1</param>
        /// <param name="otherDirection">vector 2</param>
        /// <param name="precision">precision</param>
        /// <param name="normalizeParameters">Should normalise the vector first</param>
        /// <returns>is in the same direction</returns>
        public static bool IsInDirection(Vector3 direction, Vector3 otherDirection, float precision, bool normalizeParameters = true)
        {
            if (normalizeParameters)
            {
                direction.Normalize();
                otherDirection.Normalize();
            }
            return Vector3.Dot(direction, otherDirection) > 0f + precision;
        }
        public static bool IsInDirection(Vector3 direction, Vector3 otherDirection)
        {
            return Vector3.Dot(direction, otherDirection) > 0f;
        }


        public static Vector3 ClosestGeneralDirection(Vector3 vector) { return ClosestGeneralDirection(vector, GeneralDirections); }
        public static Vector3 ClosestGeneralDirection(Vector3 vector, IList<Vector3> directions)
        {
            float maxDot = float.MinValue;
            int closestDirectionIndex = 0;

            for (int i = 0; i < directions.Count; i++)
            {
                float dot = Vector3.Dot(vector, directions[i]);
                if (dot > maxDot)
                {
                    closestDirectionIndex = i;
                    maxDot = dot;
                }
            }

            return directions[closestDirectionIndex];
        }

        /// <summary>
        /// return an angle in degree between 2 vector, based on an axis
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float AngleAroundAxis(Vector3 dir1, Vector3 dir2, Vector3 axis)
        {
            dir1 = dir1 - Vector3.Project(dir1, axis);
            dir2 = dir2 - Vector3.Project(dir2, axis);

            float angle = Vector3.Angle(dir1, dir2);
            return angle * (Vector3.Dot(axis, Vector3.Cross(dir1, dir2)) < 0 ? -1 : 1);
        }

        #endregion

        #region angle
        /// <summary>
        /// get an angle in degree using 2 vector
        /// (must be normalized)
        /// </summary>
        /// <param name="from">vector 1</param>
        /// <param name="to">vector 2</param>
        /// <returns></returns>
        public static float Angle(Vector3 from, Vector3 to)
        {
            return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from, to), -1f, 1f)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// get X amount of angle randomly, and evenly shared in the sphere
        /// </summary>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        public static Vector3[] EvenlyRepartitionOfPointsOnSphere(int numberOfPoints)
        {
            Vector3[] pointsPosition = new Vector3[numberOfPoints];
            float pointsAsFloat = numberOfPoints;

            float goldenAngle = Mathf.PI * (3 - Mathf.Sqrt(5));
            float step = 2f / pointsAsFloat;
            for (int i = 0; i < numberOfPoints; i++)
            {
                float y = i * step - 1 + (step / 2);
                float r = Mathf.Sqrt(1 - y * y);
                float phi = i * goldenAngle;

                pointsPosition[i] = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);
            }

            return pointsPosition;
        }

        /// <summary>
        /// get X amount of angle randomly, and evenly shared in the sphere
        /// </summary>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        public static Vector3[] EvenlyRepartitionOfPointsOnSphere2d(int numberOfPoints)
        {
            Vector3[] pointsPosition = new Vector3[numberOfPoints];
            float pointsAsFloat = numberOfPoints;

            float goldenAngle = Mathf.PI * (3 - Mathf.Sqrt(5));
            float step = 2f / pointsAsFloat;
            for (int i = 0; i < numberOfPoints; i++)
            {
                float y = i * step - 1 + (step / 2);
                float r = Mathf.Sqrt(1 - y * y);
                float phi = i * goldenAngle;

                pointsPosition[i] = new Vector3(Mathf.Cos(phi) * r, 0, Mathf.Sin(phi) * r);
            }
            return pointsPosition;
        }

        /// <summary>
        /// create a vector of direction "vector" with length "size"
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Vector3 SetVectorLength(Vector3 vector, float size)
        {
            //normalize the vector
            Vector3 vectorNormalized = Vector3.Normalize(vector);

            //scale the vector
            return vectorNormalized *= size;
        }

        //increase or decrease the length of vector by size
        public static Vector3 AddVectorLength(Vector3 vector, float size)
        {

            //get the vector length
            float magnitude = Vector3.Magnitude(vector);

            //calculate new vector length
            float newMagnitude = magnitude + size;

            //calculate the ratio of the new length to the old length
            float scale = newMagnitude / magnitude;

            //scale the vector
            return vector * scale;
        }

        /// <summary>
        /// prend un vecteur2 et retourne l'angle x, y en degré
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static float GetAngleFromVector2(Vector2 dir)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));       //Cross for testing -1, 0, 1
            //float signed_angle = angle * sign;                                  // angle in [-179,180]
            float angle360 = (angle + 360) % 360;                       // angle in [0,360]
            return (angle360);

            //return (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

        public static float AngleSigned(this Vector3 a, Vector3 b, Vector3 normal)
        {
            return Mathf.Atan2(Vector3.Dot(normal, Vector3.Cross(a, b)), Vector3.Dot(a, b)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Get Vector2 from angle
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2 AngleToVector2(float a, bool useRadians = false, bool yDominant = false)
        {
            if (!useRadians) a *= Mathf.Rad2Deg;
            if (yDominant)
            {
                return new Vector2(Mathf.Sin(a), Mathf.Cos(a));
            }
            else
            {
                return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            }
        }
        /// <summary>
        /// return an angle from a vector
        /// </summary>
        public static float GetAngleFromVector3(Vector3 dir, Vector3 reference)
        {
            float angle = Vector3.Angle(dir, reference);
            return (angle);
        }

        /// <summary>
        /// check la différence d'angle entre les 2 vecteurs
        /// </summary>
        public static float GetDiffAngleBetween2Vectors(Vector2 dir1, Vector2 dir2)
        {
            float angle1 = GetAngleFromVector2(dir1);
            float angle2 = GetAngleFromVector2(dir2);

            float diffAngle;
            IsAngleCloseToOtherByAmount(angle1, angle2, 10f, out diffAngle);
            return (diffAngle);
        }

        /// <summary>
        /// prend un angle A, B, en 360 format, et test si les 2 angles sont inférieur à différence (180, 190, 20 -> true, 180, 210, 20 -> false)
        /// </summary>
        /// <param name="angleReference">angle A</param>
        /// <param name="angleToTest">angle B</param>
        /// <param name="differenceAngle">différence d'angle accepté</param>
        /// <returns></returns>
        public static bool IsAngleCloseToOtherByAmount(float angleReference, float angleToTest, float differenceAngle, out float diff)
        {
            if (angleReference < 0 || angleReference > 360 ||
                angleToTest < 0 || angleToTest > 360)
            {
                Debug.LogError("angle non valide: " + angleReference + ", " + angleToTest);
            }

            diff = 180 - Mathf.Abs(Mathf.Abs(angleReference - angleToTest) - 180);

            //diff = Mathf.Abs(angleReference - angleToTest);        

            if (diff <= differenceAngle)
                return (true);
            return (false);
        }
        public static bool IsAngleCloseToOtherByAmount(float angleReference, float angleToTest, float differenceAngle)
        {
            if (angleReference < 0 || angleReference > 360 ||
                angleToTest < 0 || angleToTest > 360)
            {
                Debug.LogError("angle non valide: " + angleReference + ", " + angleToTest);
            }

            float diff = 180 - Mathf.Abs(Mathf.Abs(angleReference - angleToTest) - 180);

            //diff = Mathf.Abs(angleReference - angleToTest);        

            if (diff <= differenceAngle)
                return (true);
            return (false);
        }

        /// <summary>
        /// retourne un vecteur2 par rapport à un angle
        /// </summary>
        /// <param name="angle"></param>
        public static Vector3 GetVectorFromAngle(float angle)
        {
            return (new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle), 0));
        }

        /// <summary>
        /// renvoi l'angle entre deux vecteur, avec le 3eme vecteur de référence
        /// </summary>
        /// <param name="a">vecteur A</param>
        /// <param name="b">vecteur B</param>
        /// <param name="n">reference</param>
        /// <returns>Retourne un angle en degré</returns>
        public static float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
        {
            float angle = Vector3.Angle(a, b);                                  // angle in [0,180]
            float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));       //Cross for testing -1, 0, 1
            float signed_angle = angle * sign;                                  // angle in [-179,180]
            float angle360 = (signed_angle + 360) % 360;                       // angle in [0,360]
            return (angle360);
        }

        /// <summary>
        /// renvoi l'angle entre deux vecteur, avec le 3eme vecteur de référence
        /// </summary>
        /// <param name="a">vecteur A</param>
        /// <param name="b">vecteur B</param>
        /// <param name="n">reference</param>
        /// <returns>Retourne un angle en degré</returns>
        public static float Angle360(Vector3 a, Vector3 b)
        {
            float angle = Vector3.Angle(a, b);                                  // angle in [0,180]
            float angle360 = (angle + 360) % 360;                       // angle in [0,360]
            return (angle360);
        }
        #endregion

    }
}