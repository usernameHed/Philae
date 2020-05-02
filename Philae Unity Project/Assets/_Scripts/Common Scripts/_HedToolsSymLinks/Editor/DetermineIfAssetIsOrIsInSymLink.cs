using extUnityComponents;
using extUnityComponents.transform;
using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace hedCommon.symlinks
{
    public static class DetermineIfAssetIsOrIsInSymLink
    {
        private const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | FileAttributes.ReparsePoint;
        private const FileAttributes FILE_SYMLINK_ATTRIBS = FileAttributes.Directory & FileAttributes.Archive;
        
        /// <summary>
        /// is this 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="attribs"></param>
        /// <returns></returns>
        public static bool IsAttributeAFileInsideASymLink(string path, FileAttributes attribs, ref List<string> allSymLinksAssetPathSaved)
        {
            return (attribs & FILE_SYMLINK_ATTRIBS) == FILE_SYMLINK_ATTRIBS && path.ContainIsPaths(allSymLinksAssetPathSaved);
        }

        public static bool IsAttributeASymLink(FileAttributes attribs)
        {
            return (attribs & FOLDER_SYMLINK_ATTRIBS) == FOLDER_SYMLINK_ATTRIBS;
        }

        public static void UpdateSymLinksParent(string path, ref List<string> allSymLinksAssetPathSaved)
        {
            while (!string.IsNullOrEmpty(path))
            {
                FileAttributes attribs = File.GetAttributes(path);
                if (IsAttributeASymLink(attribs))
                {
                    allSymLinksAssetPathSaved.AddIfNotContain(path);
                    return;
                }
                path = ExtPaths.GetDirectoryFromCompletPath(path);
            }
        }
        //end of class
    }
}
