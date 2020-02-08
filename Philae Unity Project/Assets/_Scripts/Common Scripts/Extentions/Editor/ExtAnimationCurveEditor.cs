using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtAnimationCurveEditor
    {
        /// <summary>
        /// set an animation curbe to a linear curve, or other type of tangentMode
        /// use: ExtReflexion.SetAnimationCurveTangentMode(_animationCurve, AnimationUtility.TangentMode.Linear);
        /// </summary>
        /// <param name="curve"></param>
        /// <param name=""></param>
        public static void SetAnimationCurveTangentMode(AnimationCurve curve, AnimationUtility.TangentMode tangentMode)
        {
            for (int i = 0; i < curve.length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, tangentMode);
                AnimationUtility.SetKeyRightTangentMode(curve, i, tangentMode);
            }
        }

        /// <summary>
        /// get an linear AnimationCurve
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve GetLinearDefaultCurve()
        {
            AnimationCurve newCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            SetAnimationCurveTangentMode(newCurve, AnimationUtility.TangentMode.Linear);
            return (newCurve);
        }
    }
}