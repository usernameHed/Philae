using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace hedCommon.extension.editor
{
    public static class ExtUndo
    {
        public static void Record(Object toTarget, string messageUndo)
        {
            UnityEditor.Undo.RecordObject(toTarget, messageUndo);
        }
    }
}