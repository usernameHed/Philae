using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace hedCommon.extension.editor
{
    public static class ExtEnumEditor
    {
        public static void CreateEnum(string enumName, List<string> enumEntries)
        {
            string pathDirectory = "Assets/Scripts/ProcedurallyGeneratedScripts/Enums/";
            ExtFileEditor.CreateDirectoryIfNotExist(pathDirectory);

            string filePathAndName = pathDirectory + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum " + enumName);
                streamWriter.WriteLine("{");
                for (int i = 0; i < enumEntries.Count; i++)
                {
                    streamWriter.WriteLine("\t" + enumEntries[i] + " = " + i + ",");
                }
                streamWriter.WriteLine("}");
            }
            AssetDatabase.Refresh();
        }
    }
}