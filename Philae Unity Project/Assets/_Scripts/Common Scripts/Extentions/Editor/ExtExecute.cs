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
    }
}