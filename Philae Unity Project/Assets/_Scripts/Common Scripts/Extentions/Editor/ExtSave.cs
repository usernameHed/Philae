using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ExtSave
{
    public static void SetDirty(Object targetToSave)
    {
        EditorUtility.SetDirty(targetToSave);
    }

    public static void SaveAsset(Object targetToSave)
    {
        SetDirty(targetToSave);
        AssetDatabase.SaveAssets();
    }

    public static void SaveAssetAndRefresh(Object targetToSave)
    {
        SaveAsset(targetToSave);
        AssetDatabase.Refresh();
    }
}
