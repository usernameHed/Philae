using hedCommon.extension.editor;
using hedCommon.extension.editor.editorWindow;
using hedCommon.extension.runtime;
using hedCommon.sceneWorkflow;
using philae.architecture;
using philae.editor;
using philae.editor.editorGlobal;
using philae.editor.editorWindow;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace hedCommon.mixed
{
    public class CurrentEditorGlobal
    {
        private PhilaeLinker _philaeLinker;
        private ExtUtilityEditor.HitSceneView _overObjectData;
        private MouseEditorInfo _mouseEditorInfo;

        private TinyActionPanel _tinyActionPanel = new TinyActionPanel();
        private TinyMainToolsPanel _tinyMainToolsPanel = new TinyMainToolsPanel();
        private OrderRightClick _rightClick = new OrderRightClick();

        public void Init(AbstractLinker gameLinker)
        {
            _philaeLinker = (PhilaeLinker)gameLinker;
            LoadAllExternEditorWindow();

            _philaeLinker.InitFromEditor();
        }

        /// <summary>
        /// open the OpenRaceTrackNavigator of scenes
        /// </summary>
        public void OpenSomeEditorWindow()
        {
            //open or focus raceTrackNavigator
            BasicEditorWindow basicEditorWindow = ExtEditorWindow.OpenEditorWindow<BasicEditorWindow>();
            basicEditorWindow.Init();
        }


        public void LoadAllExternEditorWindow()
        {
            if (EditorPrefs.GetBool(EditorContants.EditorOpenPreference.BASIC_EDITOR_WINDOW_IS_OPEN))
            {
                OpenSomeEditorWindow();
            }
        }

        public void RightAltControlClickOnObjectInScene(ref ExtUtilityEditor.HitSceneView overObjectData)
        {
            _rightClick.RightAltControl(ref overObjectData);
        }

        public void RightClickOnObjectInScene(ref ExtUtilityEditor.HitSceneView overObjectData)
        {
            _rightClick.RightClick(ref overObjectData);
        }

        public void RightControlClickOnObjectInScene(ref ExtUtilityEditor.HitSceneView overObjectData)
        {
            _rightClick.RightControlClick(ref overObjectData);
        }

        public void SetupAllNavigator()
        {
            _tinyMainToolsPanel.Init(_philaeLinker, this);
            _tinyActionPanel.Init(_philaeLinker, this);
        }

        public void OwnGUI(ref ExtUtilityEditor.HitSceneView overObjectData, ref MouseEditorInfo mouseEditorInfo)
        {
            _overObjectData = overObjectData;
            _mouseEditorInfo = mouseEditorInfo;
            _tinyMainToolsPanel.TinyUpdate();
            _tinyActionPanel.TinyUpdate();

            EventInput();
        }

        public void OnHierarchyChanged()
        {
            if (_philaeLinker == null)
            {
                return;
            }
        }

        public void LateOwnGUI()
        {

        }

        private void EventInput()
        {
            if (Event.current.control && Event.current.shift/* && Selection.activeGameObject == null*/)
            {

            }
        }
    }

}
