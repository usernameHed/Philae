using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtExecute
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

        public static void ShowExplorer(string itemPath)
        {
            itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
#if UNITY_EDITOR_WIN
            System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
			// @todo
#endif
        }
    }
}