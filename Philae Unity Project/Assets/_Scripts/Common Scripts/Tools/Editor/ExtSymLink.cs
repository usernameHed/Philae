using UnityEditor;
using UnityEngine;
using System.IO;
using hedCommon.extension.editor;

namespace Parabox
{
    /**
	 *	An editor utility for easily creating symlinks in your project.
	 *
	 *	Adds a Menu item under `Assets/Create/Folder (Symlink)`, and 
	 *	draws a small indicator in the Project view for folders that are
	 *	symlinks.
	 */
    [InitializeOnLoad]
	public static class ExtSymLink
	{
		// FileAttributes that match a junction folder.
		const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | FileAttributes.ReparsePoint;

		// Style used to draw the symlink indicator in the project view.
		private static GUIStyle _symlinkMarkerStyle = null;
		private static GUIStyle symlinkMarkerStyle
		{
			get
			{
				if(_symlinkMarkerStyle == null)
				{
					_symlinkMarkerStyle = new GUIStyle(EditorStyles.label);
                    _symlinkMarkerStyle.normal.textColor = Color.red;
					_symlinkMarkerStyle.alignment = TextAnchor.MiddleRight;
				}
				return _symlinkMarkerStyle;
			}
		}

		/**
		 *	Static constructor subscribes to projectWindowItemOnGUI delegate.
		 */
		static ExtSymLink()
		{
			EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
		}

		/**
		 *	Draw a little indicator if folder is a symlink
		 */
		private static void OnProjectWindowItemGUI(string guid, Rect r)
		{
			try
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);

				if(!string.IsNullOrEmpty(path))
				{
					FileAttributes attribs = File.GetAttributes(path);

					if((attribs & FOLDER_SYMLINK_ATTRIBS) == FOLDER_SYMLINK_ATTRIBS )
                    {
						GUI.Label(r, "<=>", symlinkMarkerStyle);
                    }
				}
			}
			catch {}
		}

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

            OpenFolderPanel(out string sourceFolderPath, out string sourceFolderName);

            targetPath = targetPath + "/" + sourceFolderName;

			if (Directory.Exists(targetPath))	
			{
				UnityEngine.Debug.LogWarning(string.Format("A folder already exists at this location, aborting link.\n{0} -> {1}", sourceFolderPath, targetPath));
				return;
			}


            string commandeLine = "/C mklink /J \"" + targetPath + "\" \"" + sourceFolderPath + "\"";
            ExtExecute.Execute(commandeLine);
			
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		}

        /// <summary>
        /// open the folder panel, wait on output the folder path & folder name
        /// </summary>
        /// <param name="sourceFolderPath">path of the folder selected</param>
        /// <param name="sourceFolderName">name of the folder selected</param>
        /// <returns>false if abort</returns>
        private static bool OpenFolderPanel(out string sourceFolderPath, out string sourceFolderName)
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

            OpenFolderPanel(out string sourceFolderPath, out string sourceFolderName);

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
                ExtExecute.Execute(commandeLine);
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
                ExtExecute.Execute(commandeLine);
            }
            else
            {
                try
                {
                    //Place the Asset Database in a state where
                    //importing is suspended for most APIs
                    AssetDatabase.StartAssetEditing();

                    ExtFileEditor.DuplicateDirectory(targetPath, out string newPathCreated);
                    ExtExecute.Execute(commandeLine);
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
        }

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
    }
}
