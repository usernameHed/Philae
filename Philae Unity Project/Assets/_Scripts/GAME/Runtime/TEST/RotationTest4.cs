using hedCommon.extension.propertyAttribute.animationCurve;
using hedCommon.extension.propertyAttribute.onvalueChanged;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest4 : MonoBehaviour
{
    [OnValueChanged(nameof(Ici))]
    public float toChange = 5;
    [Curve(0, 1, 0, 1)]
    public AnimationCurve Curve;

    public float Other = 12f;

    public void Ici()
    {
        Other = toChange;
    }
}
