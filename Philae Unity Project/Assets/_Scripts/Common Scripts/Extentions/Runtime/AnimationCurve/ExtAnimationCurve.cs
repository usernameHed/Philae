using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtAnimationCurve
    {
        public enum EaseType
        {
            LINEAR,
            EASE_IN_OUT,
        }

        public static AnimationCurve GetCopyOf(AnimationCurve curve)
        {
            AnimationCurve newCurve = new AnimationCurve();
            for (int i = 0; i < curve.length; i++)
            {
                newCurve.AddKey(curve[i]);
            }
            return (newCurve);
        }

        /// <summary>
        /// return a color from a string
        /// </summary>
        public static float GetMaxValue(this AnimationCurve animationCurve, ref int index)
        {
            if (animationCurve.length == 0)
            {
                Debug.LogWarning("no keys");
                index = -1;
                return (0);
            }

            index = 0;
            float maxValue = animationCurve[0].value;
            for (int i = 1; i < animationCurve.length; i++)
            {
                if (animationCurve[i].value > maxValue)
                {
                    maxValue = animationCurve[i].value;
                    index = i;
                }
            }
            return (maxValue);
        }

        /// <summary>
        /// return a color from a string
        /// </summary>
        public static float GetMinValue(this AnimationCurve animationCurve, ref int index)
        {
            if (animationCurve.length == 0)
            {
                Debug.LogWarning("no keys");
                index = -1;
                return (0);
            }

            index = 0;
            float minValue = animationCurve[0].value;
            for (int i = 1; i < animationCurve.length; i++)
            {
                if (animationCurve[i].value < minValue)
                {
                    minValue = animationCurve[i].value;
                    index = i;
                }
            }
            return (minValue);
        }

        public static AnimationCurve GetDefaultCurve(EaseType easeType = EaseType.LINEAR)
        {
            switch (easeType)
            {
                case EaseType.LINEAR:
                    AnimationCurve curveLinear = AnimationCurve.Linear(0, 0, 1, 1);// new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
                                                                                   //curveLinear.postWrapMode = WrapMode.Clamp;
                    return (curveLinear);
                case EaseType.EASE_IN_OUT:
                    AnimationCurve curveEased = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    return (curveEased);
            }
            return (new AnimationCurve());
        }


        /// <summary>
        /// from a set of points corresponding to values, create and return an animationCuve
        /// </summary>
        /// <param name="time"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static AnimationCurve GetCurveFromDatas(float[] time, float[] datas)
        {
            if (time.Length == 0 || datas.Length == 0 || time.Length != datas.Length)
            {
                Debug.LogError("array not suitable");
                return (null);
            }
            //AnimationCurve curve = ExtAnimationCurve.GetDefaultCurve(ExtAnimationCurve.EaseType.EASE_IN_OUT);
            AnimationCurve curve = new AnimationCurve();

            for (int i = 0; i < time.Length; i++)
            {

                curve.AddKey(new Keyframe(time[i], datas[i], 0, 0));
            }
            return (curve);
        }
    }
}