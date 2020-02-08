using hedCommon.mixed;
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

        static ToolsButton()
        {
            _customSceneButtons.InitTextures();

            ToolbarExtender.LeftToolbarGUI.Add(_customSceneButtons.OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(_customSceneButtons.OnRightToolbarGUI);
        }
    }
}