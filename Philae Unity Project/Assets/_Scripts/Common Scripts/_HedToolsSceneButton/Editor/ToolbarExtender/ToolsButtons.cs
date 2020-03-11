using hedCommon.mixed;
using hedCommon.saveLastSelection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;


namespace UnityToolbarExtender
{
    [InitializeOnLoad]
    public class ToolsButton
    {
        private static CustomSceneButtons _customSceneButtons = new CustomSceneButtons();
        private static SaveLastSelections _saveLastSelections = new SaveLastSelections();

        static ToolsButton()
        {
            _customSceneButtons.InitTextures();

            ToolbarExtender.LeftToolbarGUI.Add(_saveLastSelections.DisplayButton);

            ToolbarExtender.LeftToolbarGUI.Add(_customSceneButtons.OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(_customSceneButtons.OnRightToolbarGUI);
        }
    }
}