using FluffyUnderware.Curvy;
using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.mixed
{
    public class SplineOptions : MonoBehaviour
    {
        [Tooltip("")]
        public float BaseOffsetUp = 0.75f;
        public CurvySpline DefaultSpline;

        public CurvySpline GetDefaultSplineForCustomSplineController()
        {
            return (DefaultSpline);
        }
    }
}
