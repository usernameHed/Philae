using hedCommon.editor.editorWindow;
using hedCommon.extension.editor;
using hedCommon.mixed;
using philae.editor.editorWindow;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace philae.editor.editorGlobal
{
    public class TinyMainToolsPanel
    {
        private static PhilaeLinker _philaeLinker;

        private TinyEditorWindowSceneView _mainToolsPanel;
        private CurrentEditorGlobal _currentEditorGlobal;


        public void Init(PhilaeLinker gameLinker, CurrentEditorGlobal currentEditorGlobal)
        {
            _currentEditorGlobal = currentEditorGlobal;
            _philaeLinker = gameLinker;

            LoadAllSceneViewPic();

            SetupEdtitorWindowAction();
        }

        public void LoadAllSceneViewPic()
        {
            //_playTexture = (Texture)EditorGUIUtility.Load("SceneView/play.png");
        }



        public void SetupEdtitorWindowAction()
        {
            _mainToolsPanel = new TinyEditorWindowSceneView();
            float xFromRight = 10; //from the right, substract this value to go left a bit
            float width = 120;
            float x = EditorGUIUtility.currentViewWidth - width - xFromRight;
            float y = 130;
            float height = 10;
            Rect rectNav = new Rect(x, y, width, height);
            _mainToolsPanel.Init(EditorContants.EditorTinyWindowInSceneViewPreference.KEY_MAIN_TOOLS_NAVIGATOR, "Tools Keys", 1, rectNav, SceneView.currentDrawingSceneView, Event.current, true, true, true, true, Vector2.zero);
        }

        private void DisplayActionButtons()
        {
            if (GUILayout.Button("Reload"))
            {
                _philaeLinker.InitFromEditor(false);
            }

            GUILayout.Label("Layout:");
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                ExtLayout.DisplaySaveAndLoadButton();
            }

            if (GUILayout.Button("Reset Prefs Edtr"))
            {
                if (EditorUtility.DisplayDialog("Delete all editor preferences.",
                "Are you sure you want to delete all the editor preferences of the sceneView ? " +
                "This action cannot be undone.", "Yes", "No"))
                {
                    EditorContants.DeleteAllKey();
                    EditorPrefs.DeleteKey(EditorContants.IS_EDITOR_SCENE_VIEW_ACTIVE);
                    _currentEditorGlobal.SetupAllNavigator();
                }
            }

            EditorOptions.Instance.SimulatePhysics = ExtGUIToggles.Toggle(EditorOptions.Instance.SimulatePhysics, null, "SIMULATE", out bool valueHasChanged, EditorStyles.miniButton);
        }

        public void TinyUpdate()
        {
            _mainToolsPanel.ShowEditorWindow(DisplayActionButtons, SceneView.currentDrawingSceneView, Event.current);
        }

        public void LateOwnGUI()
        {

        }
    }
}