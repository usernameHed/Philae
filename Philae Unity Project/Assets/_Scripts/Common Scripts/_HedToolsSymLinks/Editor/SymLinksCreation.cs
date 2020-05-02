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
    public static class SymLinksCreation
    {
        private const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | FileAttributes.ReparsePoint;
        
        /// <summary>
        /// add a folder link at the current selection in the project view
        /// </summary>
        [MenuItem("Assets/SymLink/Add Folder Link", false, 20)]
		static void DoTheSymlink()
		{
            string targetPath = GetSelectedPathOrFallback();

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
                targetPath = Path.GetDirectoryName(targetPath);

            if (IsSymLinkFolder(targetPath))
            {
                throw new System.Exception("directory is already a symLink");
            }
            if (IsFolderHasParentSymLink(targetPath, out string pathLinkFound))
            {
                throw new System.Exception("parent " + pathLinkFound + " is a symLink, doen't allow recursive symlinks");
            }

            ExtWindowComands.OpenFolderPanel(out string sourceFolderPath, out string sourceFolderName);

            targetPath = targetPath + "/" + sourceFolderName;

			if (Directory.Exists(targetPath))	
			{
				UnityEngine.Debug.LogWarning(string.Format("A folder already exists at this location, aborting link.\n{0} -> {1}", sourceFolderPath, targetPath));
				return;
			}


            string commandeLine = "/C mklink /J \"" + targetPath + "\" \"" + sourceFolderPath + "\"";
            ExtWindowComands.Execute(commandeLine);
			
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            ExtSymLinks.ResetSymLinksDatas();
		}

        

        /// <summary>
        /// Restore a lost link
        /// WARNING: it will override identical files
        /// </summary>
        [MenuItem("Assets/SymLink/Restore Folder Link", false, 20)]
        private static void RestoreSymLink()
        {
            string targetPath = GetSelectedPathOrFallback();
            string directoryName = Path.GetFileName(targetPath);

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
            {
                targetPath = Path.GetDirectoryName(targetPath);
            }

            if (IsSymLinkFolder(targetPath))
            {
                throw new System.Exception("directory " + directoryName + " is already a symLinkFolder");
            }

            ExtWindowComands.OpenFolderPanel(out string sourceFolderPath, out string sourceFolderName);

            if (string.IsNullOrEmpty(sourceFolderName))
            {
                return;
            }

            if (directoryName != sourceFolderName)
            {
                throw new System.Exception("source and target have different names");
            }

            int choice = EditorUtility.DisplayDialogComplex("Restore SymLink", "If 2 files have the same names," +
                "which one do you want to keep ?", "Keep Local ones /!\\", "cancel procedure", "Override with new ones /!\\");

            if (choice == 1)
            {
                return;
            }

            try
            {
                //Place the Asset Database in a state where
                //importing is suspended for most APIs
                AssetDatabase.StartAssetEditing();

                ExtFileEditor.DuplicateDirectory(targetPath, out string newPathCreated);
                ExtFileEditor.DeleteDirectory(targetPath);
                string commandeLine = "/C mklink /J \"" + targetPath + "\" \"" + sourceFolderPath + "\"";
                ExtWindowComands.Execute(commandeLine);
                ExtFileEditor.CopyAll(new DirectoryInfo(newPathCreated), new DirectoryInfo(targetPath), choice == 2);
                ExtFileEditor.DeleteDirectory(newPathCreated);
            }
            finally
            {
                //By adding a call to StopAssetEditing inside
                //a "finally" block, we ensure the AssetDatabase
                //state will be reset when leaving this function
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            ExtSymLinks.ResetSymLinksDatas();
        }

        [MenuItem("Assets/SymLink/Remove Folder Link", false, 20)]
        private static void RemoveSymLink()
        {
            string targetPath = GetSelectedPathOrFallback();
            string directoryName = Path.GetFileName(targetPath);

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
            {
                targetPath = Path.GetDirectoryName(targetPath);
            }

            if (!IsSymLinkFolder(targetPath))
            {
                throw new System.Exception("directory " + directoryName + " is not a symLinkFolder");
            }

            int choice = EditorUtility.DisplayDialogComplex("Remove SymLink", "Do you want to Remove the link only ?", "Remove Link Only", "Cancel", "Remove Link and Directory /!\\");
            if (choice == 1)
            {
                return;
            }
            string commandeLine = "/C rmdir \"" + ReformatPathForWindow(targetPath) + "\"";

            if (choice == 2)
            {
                ExtWindowComands.Execute(commandeLine);
            }
            else
            {
                try
                {
                    //Place the Asset Database in a state where
                    //importing is suspended for most APIs
                    AssetDatabase.StartAssetEditing();

                    ExtFileEditor.DuplicateDirectory(targetPath, out string newPathCreated);
                    ExtWindowComands.Execute(commandeLine);
                    ExtFileEditor.RenameDirectory(newPathCreated, directoryName, false);
                }
                finally
                {
                    //By adding a call to StopAssetEditing inside
                    //a "finally" block, we ensure the AssetDatabase
                    //state will be reset when leaving this function
                    AssetDatabase.StopAssetEditing();
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            ExtSymLinks.ResetSymLinksDatas();
        }

        #region UseFull Function

        public static bool IsFolderHasParentSymLink(string pathFolder, out string pathSymLinkFound)
        {
            pathSymLinkFound = "";
            
            while (!string.IsNullOrEmpty(pathFolder))
            {
                string directoryName = Path.GetDirectoryName(pathFolder);

                if (IsSymLinkFolder(directoryName))
                {
                    pathSymLinkFound = directoryName;
                    return (true);
                }
                pathFolder = directoryName;
            }

            return (false);
        }

        public static bool IsSymLinkFolder(string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                return (false);
            }

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FOLDER_SYMLINK_ATTRIBS) != FOLDER_SYMLINK_ATTRIBS)
            {
                return (false);
            }
            else
            {
                return (true);
            }
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
            return ReformatPathForUnity(path);
        }

        /// <summary>
        /// change a path from
        /// Assets\path\of\file
        /// to
        /// Assets/path/of/file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReformatPathForUnity(string path, char characterReplacer = '-')
        {
            string formattedPath = path.Replace('\\', '/');
            formattedPath = formattedPath.Replace('|', characterReplacer);
            return (formattedPath);
        }

        /// <summary>
        /// change a path from
        /// Assets/path/of/file
        /// to
        /// Assets\path\of\file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReformatPathForWindow(string path)
        {
            string formattedPath = path.Replace('/', '\\');
            formattedPath = formattedPath.Replace('|', '-');
            return (formattedPath);
        }
        #endregion
    }
}
