using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtWindowComands
    {
        public static void Execute(string commandeLine)
        {
#if UNITY_EDITOR_WIN
            Process cmd = Process.Start("CMD.exe", commandeLine);
            cmd.WaitForExit();
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
			// @todo
#endif
        }

        public static void ShowInExplorer(Object asset)
        {
            ExtWindowComands.ShowInExplorer(AssetDatabase.GetAssetPath(asset));
        }

        public static void ShowInExplorer(string itemPath)
        {
            itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
			// @todo
#endif
        }

        /// <summary>
        /// open the folder panel, wait on output the folder path & folder name
        /// </summary>
        /// <param name="sourceFolderPath">path of the folder selected</param>
        /// <param name="sourceFolderName">name of the folder selected</param>
        /// <returns>false if abort</returns>
        public static bool OpenFolderPanel(out string sourceFolderPath, out string sourceFolderName)
        {
            sourceFolderPath = EditorUtility.OpenFolderPanel("Select Folder Source", "", "");
            sourceFolderName = "";

            if (sourceFolderPath.Contains(Application.dataPath))
            {
                throw new System.Exception("Cannot create a symlink to folder in your project!");
            }

            // Cancelled dialog
            if (string.IsNullOrEmpty(sourceFolderPath))
            {
                return (false);
            }

            sourceFolderName = Path.GetFileName(sourceFolderPath);

            if (string.IsNullOrEmpty(sourceFolderName))
            {
                UnityEngine.Debug.LogWarning("Couldn't deduce the folder name?");
                return (false);
            }
            return (true);
        }

        //end of class
    }
}