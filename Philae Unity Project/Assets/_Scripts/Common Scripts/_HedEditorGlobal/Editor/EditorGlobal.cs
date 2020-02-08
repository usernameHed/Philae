using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.mixed;
using UnityEngine;

namespace hedCommon.editorGlobal
{
    /// <summary>
    /// This script is a wrapper for the CurrentEditorGlobal class
    /// this wrapper has additionnal testing and variable for initializing
    /// well in editor & play, for nicly handle input and setup the TinyEditor
    /// </summary>
    public class EditorGlobal
    {
        private AbstractLinker _gameLinker;
        public bool IsGUIShowed = false;

        private bool _needToSetupUI = true; //if it's the first time, init all the Navigator (but only in OwnGUI !)

        private CurrentEditorGlobal _currentEditorGlobal = new CurrentEditorGlobal();

        public void Init()
        {
            _gameLinker = ExtFind.GetScript<AbstractLinker>();
            if (_gameLinker == null)
            {
                return;
            }
            IsGUIShowed = true;
            _needToSetupUI = true;
            _currentEditorGlobal.Init(_gameLinker);
        }

        public bool IsInit()
        {
            return (_gameLinker != null);
        }

        public void RightAltControlClickOnObjectInScene(ref ExtUtilityEditor.HitSceneView overObjectData)
        {
            _currentEditorGlobal.RightAltControlClickOnObjectInScene(ref overObjectData);
        }

        public void RightClickOnObjectInScene(ref ExtUtilityEditor.HitSceneView overObjectData)
        {
            _currentEditorGlobal.RightClickOnObjectInScene(ref overObjectData);
        }

        public void RightControlClickOnObjectInScene(ref ExtUtilityEditor.HitSceneView overObjectData)
        {
            _currentEditorGlobal.RightControlClickOnObjectInScene(ref overObjectData);
        }

        /// <summary>
        /// called in OnGUI, setup everything, and 
        /// </summary>
        public void OwnGUISetups(ref ExtUtilityEditor.HitSceneView overObjectData, ref MouseEditorInfo mouseEditorInfo)
        {
            if (_needToSetupUI)
            {
                SetupAllNavigator();
            }
            OwnGUIDisplay(ref overObjectData, ref mouseEditorInfo);
        }

        public void Disable()
        {
            IsGUIShowed = false;
            Debug.Log("here disable...");
        }

        private void SetupAllNavigator()
        {
            _needToSetupUI = false;
            _currentEditorGlobal.SetupAllNavigator();
        }



        /// <summary>
        /// here is all the GUI we want to display
        /// </summary>
        private void OwnGUIDisplay(ref ExtUtilityEditor.HitSceneView overObjectData, ref MouseEditorInfo mouseEditorInfo)
        {
            _currentEditorGlobal.OwnGUI(ref overObjectData, ref mouseEditorInfo);
        }

        /// <summary>
        /// here is all the GUI we want to display
        /// </summary>
        public void LateOwnGUI()
        {
            _currentEditorGlobal.LateOwnGUI();
        }

        public void OnHierarchyChanged()
        {
            if (_gameLinker == null || !IsGUIShowed)
            {
                return;
            }
            _currentEditorGlobal.OnHierarchyChanged();
        }
    }
}