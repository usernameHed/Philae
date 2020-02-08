using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hedCommon.extension.runtime.interpolation
{
    public class RotationInterpolator
    {
        private SplineInterpolator xInterpolate;
        private SplineInterpolator yInterpolate;
        private SplineInterpolator zInterpolate;
        private SplineInterpolator wInterpolate;

        public enum TypeData
        {
            X,
            Y,
            Z,
            W
        }

        public RotationInterpolator(float[] keys, Quaternion[] values)
        {
            Init(ExtConverstion.ConvertFloatIntoDouble(keys), values);
        }

        public void Init(double[] keys, Quaternion[] values)
        {
            if (keys == null || values == null)
            {
                throw new ArgumentNullException("nodes");
            }
            xInterpolate = new SplineInterpolator(keys, GetValues(values, TypeData.X));
            yInterpolate = new SplineInterpolator(keys, GetValues(values, TypeData.Y));
            zInterpolate = new SplineInterpolator(keys, GetValues(values, TypeData.Z));
            wInterpolate = new SplineInterpolator(keys, GetValues(values, TypeData.W));
        }

        public Quaternion GetValue(double key)
        {
            Quaternion interpolatedPosition = new Quaternion(
                (float)xInterpolate.GetValue(key),
                (float)yInterpolate.GetValue(key),
                (float)zInterpolate.GetValue(key),
                (float)wInterpolate.GetValue(key));
            return (interpolatedPosition);
        }

        private double[] GetValues(Quaternion[] rotations, TypeData typeData)
        {
            Dictionary<double, double> finalPairs = new Dictionary<double, double>();

            double[] finalValues = new double[rotations.Length];

            for (int i = 0; i < rotations.Length; i++)
            {
                switch (typeData)
                {
                    case TypeData.X:
                        finalValues[i] = rotations[i].x;
                        break;

                    case TypeData.Y:
                        finalValues[i] = rotations[i].y;
                        break;

                    case TypeData.Z:
                        finalValues[i] = rotations[i].z;
                        break;

                    case TypeData.W:
                        finalValues[i] = rotations[i].w;
                        break;
                }
            }
            return (finalValues);
        }
    }
}