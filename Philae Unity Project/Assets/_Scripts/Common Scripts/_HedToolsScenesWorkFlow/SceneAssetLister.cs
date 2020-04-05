using hedCommon.extension.runtime;
using hedCommon.scriptableObject;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.sceneWorkflow
{
    [CreateAssetMenu(fileName = "TOOLS/Scene Workflow/Scenes Asset Lister", menuName = "Scene Workflow/Scenes Asset Lister")]
    public class SceneAssetLister : RaceScriptableObject
    {
        public List<ExtSceneReference> SceneToLoad = new List<ExtSceneReference>();

#if UNITY_EDITOR
        /// <summary>
        /// save this asset and all the assets inside to disk
        /// </summary>
        [Button]
        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif

        
        public override string GetModifiableKeyAsset()
        {
            return (FolderParentName);
        }

        public override string GetExplicitModifiableKeyAsset()
        {
            return (this.GetType().ToString() + " - " + FolderParentName);
        }
    }
}