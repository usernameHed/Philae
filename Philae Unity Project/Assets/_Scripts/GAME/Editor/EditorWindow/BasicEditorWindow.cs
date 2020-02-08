using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using philae.editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace philae.editor.editorWindow
{
    public class BasicEditorWindow : OdinEditorWindow
    {
        private static BasicEditorWindow _thisWindow;
        private PhilaeLinker _gameLinker;
        public GUIStyle BackGroundWhite = new GUIStyle();
        private Texture _addTexture;

        /*
        #region Odin Text

        [MenuItem("My Game/My Window")]
        private static void OpenWindow()
        {
            GetWindow<BasicEditorWindow>().Show();
        }


        [PropertyOrder(-10)]
        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void SomeButton1() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void SomeButton2() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void SomeButton3() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void SomeButton4() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large), GUIColor(1, 0.5f, 0)]
        public void SomeButton5() { }

        [TableList]
        public List<SomeType> SomeTableData;


        [OnInspectorGUI("DrawPreview", append: true)]
        public Texture2D Texture = EditorIcons.OdinInspectorLogo;

        #endregion
        */

        #region Setup Camera Navigator

        [MenuItem("PERSO/CameraNavigator")]
        public static void ShowBasicNavigator()
        {
            _thisWindow = BasicEditorWindow.GetWindow<BasicEditorWindow>("BasicEditorWindow");
            _thisWindow.Init();
        }

        protected override void OnEnable()
        {
            //Debug.Log("true camera nav");
            EditorPrefs.SetBool(EditorContants.EditorOpenPreference.BASIC_EDITOR_WINDOW_IS_OPEN, true);
        }

        protected override void OnDestroy()
        {
            //Debug.Log("false camera nav");
            EditorPrefs.SetBool(EditorContants.EditorOpenPreference.BASIC_EDITOR_WINDOW_IS_OPEN, false);
        }

        public void Init()
        {
            Vector2 minSize = this.minSize;
            minSize.y = 26;
            this.minSize = minSize;


            ActualiseEditorWindow();
        }

        /// <summary>
        /// SHow everything in the RaceTrackNavigator
        /// </summary>
        private void ActualiseEditorWindow()
        {
            _gameLinker = ExtFind.GetScript<PhilaeLinker>();
            SetupTextures();
        }

        public void SetupTextures()
        {
            _addTexture = (Texture)EditorGUIUtility.Load("SceneView/add.png");
        }

        public void SetupInGUI()
        {
            BackGroundWhite.normal.background = ExtTexture.MakeTex(600, 1, Color.white);
        }

        protected override void OnGUI()
        {


            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                GUILayout.Label("ici 1");
                GUILayout.Label("ici 2");
            }
            base.OnGUI();
        }

        public void OnInspectorUpdate()
        {


            // This will only get called 10 times per second.
            Repaint();
            SetupInGUI();
        }

        #endregion

        #region OdinFunctions

        /*
        private void DrawPreview()
        {
            if (this.Texture == null) return;

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(this.Texture, GUILayout.MaxWidth(100), GUILayout.MaxHeight(100));
            GUILayout.EndVertical();
        }
        */

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox("OnInspectorGUI can also be used on both methods and properties", UnityEditor.MessageType.Info);
        }

        #endregion
    }

    public class SomeType
    {
        [TableColumnWidth(50)]
        public bool Toggle;

        [AssetsOnly]
        public GameObject SomePrefab;

        public string Message;

        [TableColumnWidth(160)]
        [HorizontalGroup("Actions")]
        public void Test1() { }

        [HorizontalGroup("Actions")]
        public void Test2() { }
    }
}