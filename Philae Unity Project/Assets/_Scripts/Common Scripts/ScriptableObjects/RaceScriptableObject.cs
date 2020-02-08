using hedCommon.extension.runtime;
using Sirenix.OdinInspector;
using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

namespace hedCommon.scriptableObject
{
    public abstract class RaceScriptableObject : ScriptableObject
    {
        [SerializeField]
        protected bool _isActive = true;
        public void SetActiveSelf(bool active)
        {
            _isActive = active;
        }

        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        public string FolderParentName = "";
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        public string FolderParentPath = "";

        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        public string CalculatedPath = "";

        public virtual bool IsActiveSelf()
        {
            return (_isActive);
        }
        public virtual bool IsActiveInBuild()
        {
            return (_isActive);
        }

        public abstract string GetModifiableKeyAsset();
        public abstract string GetExplicitModifiableKeyAsset();

#if UNITY_EDITOR
        [Button]
        public void CalculatePaths()
        {
            CalculatedPath = AssetDatabase.GetAssetPath(this);      //get Assets/__RACES/Tracks/Monaco/Monaco.asset
            FolderParentPath = Path.GetDirectoryName(CalculatedPath);   //get Assets\__RACES\Tracks\Monaco
            FolderParentPath = ExtPaths.ReformatPathForUnity(FolderParentPath);  //get Assets/__RACES/Tracks/Monaco
            FolderParentName = Path.GetFileName(FolderParentPath);            //get Monaco
        }
#endif
    }
}