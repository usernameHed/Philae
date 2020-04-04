using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using philae.editor.editorWindow;
using hedCommon.time;
using hedCommon.extension.editor;
using hedCommon.mixed;
using hedCommon.extension.editor.sceneView;

namespace philae.editor.editorGlobal
{
    public class TinyActionPanel
    {
        private static PhilaeLinker _gameLinker;

        private TinyEditorWindowSceneView _actionPanel;
        private CurrentEditorGlobal _currentEditorGlobal;

        public void Init(PhilaeLinker gameLinker, CurrentEditorGlobal currentEditorGlobal)
        {
            _currentEditorGlobal = currentEditorGlobal;
            _gameLinker = gameLinker;

            LoadAllSceneViewPic();

            SetupEdtitorWindowAction();
        }

        public void LoadAllSceneViewPic()
        {
            //_playTexture = (Texture)EditorGUIUtility.Load("SceneView/play.png");
        }

        public void SetupEdtitorWindowAction()
        {
            _actionPanel = new TinyEditorWindowSceneView();
            _actionPanel.TinyInit(EditorContants.EditorTinyWindowInSceneViewPreference.KEY_ACTION_NAVIGATOR, "Action", TinyEditorWindowSceneView.DEFAULT_POSITION.UP_RIGHT);
            _actionPanel.IsClosable = false;
            _actionPanel.IsMinimisable = true;
        }


        public void TinyUpdate()
        {
            _actionPanel.ShowEditorWindow(DisplayActionButtons, SceneView.currentDrawingSceneView, Event.current);
        }

        private void DisplayActionButtons()
        {
            GUILayout.Label("action ?");
            //TimeEditor.timeScale = ExtGUIFloatFields.FloatField(TimeEditor.timeScale, _timeEditor, out bool valueHasChanged, "TimeScale: ", "", 0, 1, true);
        }

        public void LateOwnGUI()
        {

        }
    }
}