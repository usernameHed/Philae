using System;
using UnityEngine;

namespace hedCommon.extension.runtime.range
{
    [Serializable]
    public struct FloatRandomRange
    {
        public float Minimum;
        public float Maximum;
        public AnimationCurve Curve;

        public FloatRandomRange(float min, float max, AnimationCurve curve)
        {
            Minimum = min;
            Maximum = max;
            Curve = curve;
        }

        public float GetRandomValue()
        {
            if (Curve == null || (Minimum == Maximum))
            {
                return (Minimum);
            }
            return (ExtRandom.GetRandomNumber(this));
        }
    }
}