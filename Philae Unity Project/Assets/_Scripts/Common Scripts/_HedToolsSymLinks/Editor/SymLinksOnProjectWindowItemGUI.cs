using UnityEditor;
using UnityEngine;
using System.IO;
using hedCommon.extension.editor;
using System.Collections.Generic;
using hedCommon.extension.runtime;
using extUnityComponents.transform;
using extUnityComponents;
using System;
using System.Reflection;
using hedCommon.time;

namespace hedCommon.symlinks
{
    /// <summary>
    /// An editor utility for easily creating symlinks in your project.
    /// 
    /// Adds a Menu item under `Assets/Create/Folder(Symlink)`, and
    /// draws a small indicator in the Project view for folders that are
    /// symlinks.
    /// </summary>
    [InitializeOnLoad]
    public static class SymLinksOnProjectWindowItemGUI
    {
        public static List<string> AllSymLinksAssetPathSaved = new List<string>(300);
        
        public static void AddPathOfSymLinkAsset(string path)
        {
            if (AllSymLinksAssetPathSaved.AddIfNotContain(path))
            {
                AllSymLinksAssetPathSaved.Sort();
            }
        }

        /// <summary>
        /// Static constructor subscribes to projectWindowItemOnGUI delegate.
        /// </summary>
        static SymLinksOnProjectWindowItemGUI()
		{
            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemGUI;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        public static void ResetSymLinksDatas()
        {
            AllSymLinksAssetPathSaved.Clear();
        }

        /// <summary>
        /// Draw a little indicator if folder is a symlink
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="r"></param>
        private static void OnProjectWindowItemGUI(string guid, Rect r)
		{
			try
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);

				if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                FileAttributes attribs = File.GetAttributes(path);
                DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(path, ref AllSymLinksAssetPathSaved);
                if (DetermineIfAssetIsOrIsInSymLink.IsAttributeASymLink(attribs))
                {
                    ExtSymLinks.DisplayBigMarker(r, "this folder is a symlink", SymLinksColorChoice.ChooseColorFromPath(path));
                }
                else if (DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(path, attribs, ref AllSymLinksAssetPathSaved))
                {
                    ExtSymLinks.DisplayTinyMarker(r, "this object is inside a symlink folder", SymLinksColorChoice.ChooseColorFromPath(path));
                }
            }
			catch {}
		}
    }
}
