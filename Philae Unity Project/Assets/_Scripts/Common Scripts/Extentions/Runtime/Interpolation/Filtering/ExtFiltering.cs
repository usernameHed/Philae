using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime.interpolation.filtering
{
    public static class ExtFiltering
    {
        public static float[] Smooth(float[] xval, float[] yval, float bandWidth = (float)LoessInterpolator.DEFAULT_BANDWIDTH, int robustnessIterations = LoessInterpolator.DEFAULT_ROBUSTNESS_ITERS)
        {
            LoessInterpolator loess = new LoessInterpolator((double)bandWidth, robustnessIterations);

            double[] xValDouble = ExtConverstion.ConvertFloatIntoDouble(xval);
            double[] yValDouble = ExtConverstion.ConvertFloatIntoDouble(yval);

            double[] datas = loess.Smooth(xValDouble, yValDouble);
            return (ExtConverstion.ConvertDoubleIntoFloat(datas));
        }
    }
}