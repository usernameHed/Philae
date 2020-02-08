using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtNumbers
{
    public static bool IsClose(this float currentValue, float valueToCompare)
    {
        return IsClose(currentValue, valueToCompare, Mathf.Epsilon);
    }


    public static bool IsClose(this float currentValue, float valueToCompare, float precision)
    {
        return Mathf.Abs(currentValue - valueToCompare) < precision;
    }
    public static bool IsBetween(this float currentValue, float value1, float value2)
    {
        return (value1 <= currentValue && currentValue <= value2);
    }
}
