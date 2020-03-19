using hedCommon.mixed;
using hedCommon.saveLastSelection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace hedCommon.toolbarExtent
{
    [InitializeOnLoad]
    public class ToolsButton
    {
        private static CustomSceneButtons _customSceneButtons = new CustomSceneButtons();
        private static SaveLastSelections _saveLastSelections = new SaveLastSelections();
        private static TimeScaleSlider _timeScaleSlider = new TimeScaleSlider();

        static ToolsButton()
        {
            _customSceneButtons.InitTextures();

            ToolbarExtender.LeftToolbarGUI.Add(_saveLastSelections.DisplayButton);
            ToolbarExtender.LeftToolbarGUI.Add(_timeScaleSlider.DisplaySlider);
            ToolbarExtender.LeftToolbarGUI.Add(_customSceneButtons.OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(_customSceneButtons.OnRightToolbarGUI);
        }
    }
}