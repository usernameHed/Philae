using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.symlinks
{
    [InitializeOnLoad]
    public class SymLinksColorChoice
    {
        private static SymLinksOptions _symLinksOptions;

        static SymLinksColorChoice()
        {
            _symLinksOptions = ExtFind.GetAssetByGenericType<SymLinksOptions>();
        }

        public static Color ChooseColorFromPath(string path)
        {
            if (_symLinksOptions == null)
            {
                return (Color.blue);
            }

            if (string.IsNullOrEmpty(path))
            {
                return (Color.blue);
            }
            for (int i = 0; i < SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved.Count; i++)
            {
                if (path.Contains(SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved[i]))
                {
                    return (_symLinksOptions.GetColorOfAssetInSymLink(i));
                }
            }
            return (_symLinksOptions.GetColorOfAssetInSymLink(-1));
        }
    }
}