using hedCommon.extension.editor;
using hedCommon.extension.editor.sceneView;
using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttractorOverrideGenericEditor
{
    public const string PROPERTY_GRAVITY_OVERRIDE = "GravityOverride";


    public void ShowTinyEditorContent()
    {
        EditorOptions.Instance.ShowGravityOverride = GUILayout.Toggle(EditorOptions.Instance.ShowGravityOverride, "Setup Gravity", EditorStyles.miniButton);
    }

    public bool CanSetupGravity()
    {
        return (EditorOptions.Instance.ShowGravityOverride);
    }

    /// <summary>
    /// need to be called at the end of the editor, lock the editor from deselecting the gameObject from the sceneView
    /// </summary>
    public void LockEditor()
    {
        //if nothing else, lock editor !
        if (EditorOptions.Instance.ShowGravityOverride)
        {
            ExtSceneView.LockFromUnselect();
        }
        if (ExtEventEditor.IsKeyDown(KeyCode.Escape))
        {
            EditorOptions.Instance.ShowGravityOverride = false;
        }

        if (EditorOptions.Instance.ShowGravityOverride && ExtEventEditor.IsKeyDown(KeyCode.Delete))
        {
            ExtEventEditor.Use();
        }
    }
}
