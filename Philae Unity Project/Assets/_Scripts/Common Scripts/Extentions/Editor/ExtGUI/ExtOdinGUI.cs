using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// create editorWindow:
/// public class MyClass : OdinEditorWindow
/// </summary>
public static class ExtOdinGUI
{
    private static void ActiveCustomWinwow(Object objectToInspect)
    {
        var window = OdinEditorWindow.InspectObject(objectToInspect);

        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(270, 200);
        window.titleContent = new GUIContent("Custom title", EditorIcons.RulerRect.Active);
        window.OnClose += () => Debug.Log("Window Closed");
        window.OnBeginGUI += () => GUILayout.Label("-----------");
        window.OnEndGUI += () => GUILayout.Label("-----------");
    }

    public static void DrawList<T>(List<T> listToDraw, Object target)
    {
        PropertyTree tree = PropertyTree.Create(target);

        InspectorUtilities.BeginDrawPropertyTree(tree, true);
        tree.GetPropertyAtPath("listToDraw").Draw();
        InspectorUtilities.EndDrawPropertyTree(tree);

    }
}
