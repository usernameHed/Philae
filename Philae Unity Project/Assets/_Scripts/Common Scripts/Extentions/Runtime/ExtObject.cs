using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace hedCommon.extension.runtime
{
    public static class ExtObject
    {
        public static string GetPath(this UnityEngine.Object asset)
        {
            return (AssetDatabase.GetAssetPath(asset));
        }

        public static bool IsTruelyNull(this object aRef)
        {
            return aRef != null && aRef.Equals(null);
        }
    }
}