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
    public static class ExtSymLinks
    {
        private static List<string> _allSymLinksAssetPathSaved = new List<string>(300);
        public static void AddPathOfSymLinkAsset(string path)
        {
            _allSymLinksAssetPathSaved.AddIfNotContain(path);
        }
        private const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | FileAttributes.ReparsePoint;
        private const FileAttributes FILE_SYMLINK_ATTRIBS = FileAttributes.Directory & FileAttributes.Archive;

        private static GUIStyle _symlinkMarkerStyle = null;
        private static GUIStyle SymlinkMarkerStyle
        {
            get
            {
                if (_symlinkMarkerStyle == null)
                {
                    _symlinkMarkerStyle = new GUIStyle(EditorStyles.label);
                    _symlinkMarkerStyle.normal.textColor = Color.blue;
                    _symlinkMarkerStyle.alignment = TextAnchor.MiddleRight;
                }
                return _symlinkMarkerStyle;
            }
        }

        public static void DisplayBigMarker(Rect r)
        {
            GUI.Label(r, "<=>", ExtSymLinks.SymlinkMarkerStyle);
        }
        public static void DisplayTinyMarker(Rect r)
        {
            GUI.Label(r, "*  ", ExtSymLinks.SymlinkMarkerStyle);
        }

        public static void ResetSymLinksDatas()
        {
            _allSymLinksAssetPathSaved.Clear();
        }

        /// <summary>
        /// is this 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="attribs"></param>
        /// <returns></returns>
        public static bool IsAttributeAFileInsideASymLink(string path, FileAttributes attribs)
        {
            return (attribs & FILE_SYMLINK_ATTRIBS) == FILE_SYMLINK_ATTRIBS && path.ContainIsPaths(_allSymLinksAssetPathSaved);
        }

        public static bool IsAttributeASymLink(FileAttributes attribs)
        {
            return (attribs & FOLDER_SYMLINK_ATTRIBS) == FOLDER_SYMLINK_ATTRIBS;
        }

        public static void UpdateSymLinksParent(string path)
        {
            while (!string.IsNullOrEmpty(path))
            {
                FileAttributes attribs = File.GetAttributes(path);
                if (IsAttributeASymLink(attribs))
                {
                    ExtSymLinks.AddPathOfSymLinkAsset(path);
                    return;
                }
                path = ExtPaths.GetDirectoryFromCompletPath(path);
            }
        }
    }
}
