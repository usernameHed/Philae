using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.time;
using System;
using UnityEditor;
using UnityEngine;

namespace hedCommon.editorGlobal
{
    /// <summary>
    /// Manage the global Editor of the project
    /// This script manage to Initialize the EditorGlobal of the project, in play and in eidtor
    /// and call the good functions: call event of the UI, the mouse clic, etc.
    /// 
    /// It need to initialize EditorGlobal.
    /// This editor can be initialize IF it found the Script if want to find in the project hierarchy
    /// </summary>
    [InitializeOnLoad]
    public class EditorSceneView : EditorWindow
    {
        private static bool _isEnabled = false;
        private static SceneView _currentSceneView; //need to be updated each frame
        private static Event _currentEvent; //need to be updated each frame
        private static ExtUtilityEditor.HitSceneView _hitScene = new ExtUtilityEditor.HitSceneView();

        public const string IS_EDITOR_SHOWED = "IS_EDITOR_SHOWED";  //used to save if we want to let the editor showed or not
        private static EditorChronoWithNoTimeEditor _waitBeforeResetup = new EditorChronoWithNoTimeEditor();
        private static EditorGlobal _editorGlobal = new EditorGlobal();
        private static Color _oldColor;
        private static MouseEditorInfo _mouseEditorInfo = new MouseEditorInfo();

        /// <summary>
        /// called by unity at lauch of the editor unity, after unity compile, on play, and when we get out of the play
        /// here we enable the editorSceneView, if not already active
        /// </summary>
        static EditorSceneView()
        {
            if (_isEnabled)
            {
                return;
            }
            _waitBeforeResetup.Reset();
            CustomEnable();
        }

        /// <summary>
        /// here we just show or hide the content of the EditorGlobal
        /// </summary>
        [MenuItem("TOOLS/Enable Global Editor")]
        public static void ShowOrHideGlobalEditorGUI()
        {
            if (!EditorPrefs.HasKey(IS_EDITOR_SHOWED))
            {
                _editorGlobal.IsGUIShowed = true;
                EditorPrefs.SetBool(IS_EDITOR_SHOWED, _editorGlobal.IsGUIShowed);
                return;
            }

            bool currentState = EditorPrefs.GetBool(IS_EDITOR_SHOWED);
            Debug.Log("here try to disable/enable ?");
            _editorGlobal.IsGUIShowed = !currentState;
            EditorPrefs.SetBool(IS_EDITOR_SHOWED, _editorGlobal.IsGUIShowed);
        }

        /// <summary>
        /// called at lauch or the editor unity, after unity compile, on play, and when we get out of the play
        /// Here we subscribe to the main unity OnGUI update
        /// </summary>
        private static void CustomEnable()
        {
            EditorApplication.update += OnUpdate;
            SceneView.duringSceneGui += OnScene;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            _isEnabled = true;
        }
        /// <summary>
        /// here called when we manualy want to disable the editor,
        /// but with our architecture, we never want to.
        /// For us: this script should be active all the time, in editor or in play
        /// if we want to hide the tools, use ShowOrHideGlobalEditorGUI()
        /// </summary>
        private static void Disable()
        {
            SceneView.duringSceneGui -= OnScene;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            Debug.Log("Scene GUI : Disabled");
            _isEnabled = false;
            _editorGlobal.Disable();
        }

        /// <summary>
        /// setup EditorGLobal
        /// </summary>
        private static void OnUpdate()
        {
            //here don't need to try in play ?
            SetupGlobalScriptIfNull();

            if (_editorGlobal.IsInit())
            {
                //if the key doen't exist, create it
                if (!EditorPrefs.HasKey(IS_EDITOR_SHOWED))
                {
                    EditorPrefs.SetBool(IS_EDITOR_SHOWED, true);
                }
            }
        }

        private static void OnHierarchyChanged()
        {
            _editorGlobal.OnHierarchyChanged();
        }

        /// <summary>
        /// Important ! Setup all script in scene if there are not
        /// </summary>
        private static void SetupGlobalScriptIfNull()
        {
            if (!_editorGlobal.IsInit())
            {
                if (_waitBeforeResetup.IsFinished(false))
                {
                    //Debug.Log("here try to init EditorSceneView When timer is finished");
                    _editorGlobal.Init();
                    if (!_editorGlobal.IsInit())
                    {
                        _waitBeforeResetup.StartChrono(2);
                        //Debug.Log("not found, try to init again..");
                    }
                }
                else if (!_waitBeforeResetup.IsRunning())
                {
                    _editorGlobal.Init();
                    if (!_editorGlobal.IsInit())
                    {
                        _waitBeforeResetup.StartChrono(2);
                    }
                }
            }
        }

        /// <summary>
        /// called in OnGUI of the editor
        /// </summary>
        /// <param name="sceneview"></param>
        private static void OnScene(SceneView sceneview)
        {
            if (_editorGlobal.IsInit() && EditorPrefs.GetBool(IS_EDITOR_SHOWED))
            {
                OwnGUI();
            }
        }

        /// <summary>
        /// call the good functions in OnGUI.
        /// - Manage the over of an object: save the last object we over in the scene view
        /// - call the main EditorGLobal UI
        /// We need to save the last color, and then set it back, like every tools editor should do
        /// </summary>
        private static void OwnGUI()
        {
            if (ExtPrefabs.IsInPrefabStage())
            {
                return;
            }

            _currentSceneView = SceneView.currentDrawingSceneView;
            _currentEvent = Event.current;

            _oldColor = GUI.backgroundColor;

            SetCurrentOverObject();

            _editorGlobal.OwnGUISetups(ref _hitScene, ref _mouseEditorInfo);

            EventInput();

            LateOwnGUI();
        }

        /// <summary>
        /// Raycast from mouse Position in scene view for future mouseClick object
        /// </summary>
        private static void SetCurrentOverObject()
        {
            _hitScene = ExtUtilityEditor.SetCurrentOverObject(_hitScene);
        }

        /// <summary>
        /// all event of the RaceScene UI 
        /// </summary>
        private static void EventInput()
        {
            //here click on object with CTRL + ALT + RIGHT CLIC
            if (_currentEvent.button == 1 && _currentEvent.control && _currentEvent.alt && _currentEvent.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseUp && _hitScene.objHit)
            {
                _editorGlobal.RightAltControlClickOnObjectInScene(ref _hitScene);
            }
            //clic on object with right click only
            else if (_currentEvent.button == 1 && !_currentEvent.control && _currentEvent.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseUp && _hitScene.objHit)
            {
                _editorGlobal.RightClickOnObjectInScene(ref _hitScene);
            }

            //clic on object with right click & control
            else if (_currentEvent.button == 1 && _currentEvent.control && _currentEvent.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseUp && _hitScene.objHit)
            {
                _editorGlobal.RightControlClickOnObjectInScene(ref _hitScene);
            }
        }

        /// <summary>
        /// do the ending process of the OwnGUI
        /// </summary>
        private static void LateOwnGUI()
        {
            _editorGlobal.LateOwnGUI();
            _mouseEditorInfo.CustomUpdate();
            GUI.backgroundColor = _oldColor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ExtUtilityEditor.HitSceneView GetEditorSceneViewHit()
        {
            return (_hitScene);
        }

        //end class
    }
}