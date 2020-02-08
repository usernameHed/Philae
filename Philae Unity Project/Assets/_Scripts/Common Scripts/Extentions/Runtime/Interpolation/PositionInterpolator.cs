using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hedCommon.extension.runtime.interpolation
{
    public class PositionInterpolator
    {
        private SplineInterpolator xInterpolate;
        private SplineInterpolator yInterpolate;
        private SplineInterpolator zInterpolate;

        public enum TypeData
        {
            X,
            Y,
            Z,
        }

        public PositionInterpolator(double[] keys, Vector3[] values)
        {
            Init(keys, values);
        }

        public PositionInterpolator(float[] keys, Vector3[] values)
        {
            Init(ExtConverstion.ConvertFloatIntoDouble(keys), values);
        }

        private void Init(double[] keys, Vector3[] values)
        {
            if (keys == null || values == null)
            {
                throw new ArgumentNullException("nodes");
            }
            xInterpolate = new SplineInterpolator(keys, GetValues(values, TypeData.X));
            yInterpolate = new SplineInterpolator(keys, GetValues(values, TypeData.Y));
            zInterpolate = new SplineInterpolator(keys, GetValues(values, TypeData.Z));
        }

        public Vector3 GetValue(double key)
        {
            Vector3 interpolatedPosition = new Vector3((float)xInterpolate.GetValue(key), (float)yInterpolate.GetValue(key), (float)zInterpolate.GetValue(key));
            return (interpolatedPosition);
        }

        private double[] GetValues(Vector3[] positions, TypeData typeData)
        {
            double[] finalValues = new double[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                switch (typeData)
                {
                    case TypeData.X:
                        finalValues[i] = positions[i].x;
                        break;

                    case TypeData.Y:
                        finalValues[i] = positions[i].y;
                        break;

                    case TypeData.Z:
                        finalValues[i] = positions[i].z;
                        break;
                }
            }
            return (finalValues);
        }
    }
}