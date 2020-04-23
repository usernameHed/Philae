using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtFileEditor
    {
        /// <summary>
        /// Creates a directory at <paramref name="folder"/> if it doesn't exist
        /// </summary>
        /// <param name="folder">true if we created a new directory</param>
        public static bool CreateDirectoryIfNotExist(this string folder, bool refreshProject = false)
        {
            bool directoryCreated = ExtFile.CreateDirectoryIfNotExist(folder);
            if (directoryCreated && refreshProject)
            {
                AssetDatabase.Refresh();
            }
            return (directoryCreated);
        }

        public static void DeleteDirectory(string localPath, bool recursive = true, bool refreshProject = true)
        {
            bool directoryDeleted = ExtFile.DeleteADirectory(localPath, recursive);
            if (directoryDeleted && refreshProject)
            {
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// duplicate a directory at the same location, with an incremental name
        /// </summary>
        /// <param name="pathDirectoryToDuplicate"></param>
        /// <returns></returns>
        public static bool DuplicateDirectory(string pathDirectoryToDuplicate, out string newPathDirectory)
        {
            newPathDirectory = ExtPaths.RenameIncrementalFile(pathDirectoryToDuplicate, out int index, false);
            return (ExtFileEditor.DuplicateDirectory(pathDirectoryToDuplicate, newPathDirectory));
        }

        /// <summary>
        /// duplicate a directory
        /// from Assets/to/duplicate
        /// to Assets/new/location
        /// </summary>
        /// <param name="pathDirectoryToDuplicate">directory path to duplicate</param>
        /// <param name="newPathDirectory">new direcotry path</param>
        /// <param name="incrementDirectoryNameIfNeeded">do we increment the new duplicated name of the folder if folder already exist ?</param>
        /// <returns>true if succed, false if not duplicated</returns>
        public static bool DuplicateDirectory(string pathDirectoryToDuplicate, string newPathDirectory, bool incrementDirectoryNameIfNeeded = true, bool refreshDataBase = true)
        {
            if (ExtFile.IsDirectiryExist(newPathDirectory))
            {
                if (incrementDirectoryNameIfNeeded)
                {
                    newPathDirectory = ExtPaths.RenameIncrementalFile(newPathDirectory, out int index, false);
                }
                else
                {
                    Debug.Log("directory exist");
                    return (false);
                }
            }
            Debug.Log("copie " + pathDirectoryToDuplicate);
            Debug.Log("into " + newPathDirectory);

            FileUtil.CopyFileOrDirectory(pathDirectoryToDuplicate, newPathDirectory);

            if (refreshDataBase)
            {
                AssetDatabase.Refresh();
            }
            return (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathOldFile"></param>
        /// <param name="pathNewFile"></param>
        /// <returns></returns>
        public static bool DuplicateFile(string pathOldFile, string pathNewFile, bool refreshAsset)
        {
            FileUtil.CopyFileOrDirectory(pathOldFile, pathNewFile);
            if (refreshAsset)
            {
                AssetDatabase.Refresh();
            }

            return (true);
        }

        public static bool DeleteDirectory(string pathOfDirectory)
        {
            return (FileUtil.DeleteFileOrDirectory(pathOfDirectory));
        }

        /// <summary>
        /// rename an asset, and return the name
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static string RenameAsset(string oldPath, string newName, bool refreshAsset)
        {
            if (ExtFile.IsDirectoryExist(oldPath))
            {
                throw new System.Exception("a file must be given, not a directory");
            }

            string pathWhitoutName = Path.GetDirectoryName(oldPath);
            pathWhitoutName = ExtPaths.ReformatPathForUnity(pathWhitoutName);
            string extension = Path.GetExtension(oldPath);
            string newWantedName = ExtPaths.ReformatPathForUnity(newName);

            AssetDatabase.RenameAsset(oldPath, newWantedName);
            if (refreshAsset)
            {
                AssetDatabase.Refresh();
            }
            string newPath = pathWhitoutName + "/" + newWantedName + extension;
            Debug.Log(oldPath + " renamed to: " + newPath);
            return (newPath);
        }

        /// <summary>
        /// rename an asset, and return the name
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static string RenameDirectory(string oldPath, string newName, bool refreshAsset)
        {
            if (ExtFile.IsFileExist(oldPath))
            {
                throw new System.Exception("a directory must be given, not a file");
            }

            string pathWhitoutName = Path.GetDirectoryName(oldPath);
            pathWhitoutName = ExtPaths.ReformatPathForUnity(pathWhitoutName);
            string newWantedName = ExtPaths.ReformatPathForUnity(newName);

            FileUtil.MoveFileOrDirectory(oldPath, pathWhitoutName + "/" + newWantedName);
            if (refreshAsset)
            {
                AssetDatabase.Refresh();
            }
            string newPath = pathWhitoutName + "/" + newWantedName;
            Debug.Log(oldPath + " renamed to: " + newPath);
            return (newPath);
        }

        /// <summary>
        /// return the path of the current selected folder (or the current folder of the asset selected)
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return ExtPaths.ReformatPathForUnity(path);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, bool overrideFile)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Debug.Log("Copying " + target.FullName + " / " + fi.Name);
                string destinationPath = Path.Combine(target.ToString(), fi.Name);
                destinationPath = ExtPaths.ReformatPathForWindow(destinationPath);
                Debug.Log("destination: " + destinationPath);
                if (ExtFile.IsFileExist(destinationPath) && overrideFile)
                {
                    Debug.Log("file exist ! don't override this one");
                    continue;
                }
                fi.CopyTo(destinationPath, true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, overrideFile);
            }
        }

        public static string GetPath(this Object asset)
        {
            return (AssetDatabase.GetAssetPath(asset));
        }
    }
}