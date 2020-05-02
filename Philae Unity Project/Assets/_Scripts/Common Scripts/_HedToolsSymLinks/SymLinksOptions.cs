using hedCommon.extension.runtime;
using hedCommon.scriptableObject;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.symlinks
{
    [CreateAssetMenu(fileName = "TOOLS/SymLink/Options", menuName = "SymLink/SymLinks Options")]
    public class SymLinksOptions : ScriptableObject
    {
        [SerializeField]
        private Color[] _colorSymlinks = new Color[2] { Color.blue, Color.red };
        [SerializeField]
        private Color _defaultColor = Color.blue;

        public Color GetColorOfAssetInSymLink(int index)
        {
            if (index < 0 || index >= _colorSymlinks.Length)
            {
                return (_defaultColor);
            }
            return (_colorSymlinks[index]);
        }
    }
}