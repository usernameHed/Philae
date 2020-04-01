using hedCommon.extension.runtime.range;
using UnityEngine;

namespace hedCommon.extension.propertyAttribute.animationCurve
{
    /// <summary>
    /// usage:
    /// [SerializeField, Curve(0, 1, 0, 100)]
    /// private AnimationCurve _curvePercentage = new AnimationCurve();
    /// </summary>
    public class CurveAttribute : PropertyAttribute
    {
        public FloatRange X;
        public FloatRange Y;

        public CurveAttribute(float xMin, float xMax, float yMin, float yMax)
        {
            this.X = new FloatRange(xMin, xMax);
            this.Y = new FloatRange(yMin, yMax);
        }
    }
}