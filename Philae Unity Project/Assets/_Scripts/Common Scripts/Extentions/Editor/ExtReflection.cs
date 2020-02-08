using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

namespace hedCommon.extension.editor
{
    /// <summary>
    /// useful reflection methods
    /// </summary>
    public class ExtReflection
    {
        public static void ClearConsole()
        {
            // This simply does "LogEntries.Clear()" the long way:
            var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        public static void Place(GameObject go, GameObject parent)
        {
            if (parent != null)
            {
                var transform = go.transform;
                Undo.SetTransformParent(transform, parent.transform, "Reparenting");
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                go.layer = parent.layer;

                if (parent.GetComponent<RectTransform>())
                    ObjectFactory.AddComponent<RectTransform>(go);
            }
            else
            {
                ExtSceneView.PlaceGameObjectInFrontOfSceneView(go);
                StageUtility.PlaceGameObjectInCurrentStage(go); // may change parent
            }

            // Only at this point do we know the actual parent of the object and can mopdify its name accordingly.
            GameObjectUtility.EnsureUniqueNameForSibling(go);
            Undo.SetCurrentGroupName("Create " + go.name);

            //EditorWindow.FocusWindowIfItsOpen<SceneHierarchyWindow>();
            Selection.activeGameObject = go;
        }

        /// <summary>
        /// search for an object in all editro search bar
        /// use: ExtReflexion.SetSearch("to find", ExtReflexion.AllNameAssemblyKnown.SceneHierarchyWindow);
        /// use: ExtReflexion.SetSearch("to find", ExtReflexion.AllNameAssemblyKnown.SceneView);
        /// </summary>
        /// <param name="search"></param>
        public static void SetSearch(string search, ExtReflection.AllNameAssemblyKnown nameEditorWindow = AllNameAssemblyKnown.SceneView)
        {
            //open animation window
            System.Type animationWindowType = null;
            EditorWindow animationWindowEditor = ExtReflection.OpenEditorWindow(nameEditorWindow, ref animationWindowType);

            //System.Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            //EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");

            MethodInfo methodInfo = animationWindowType.GetMethod("SetSearchFilter", GetFullBinding());

            if (methodInfo == null)
            {
                Debug.Log("null");
                return;
            }

            var window = EditorWindow.focusedWindow;
            methodInfo.Invoke(window, new object[] { search, SearchableEditorWindow.SearchMode.All, true, false });
        }

        /// <summary>
        /// play button on animator
        /// </summary>
        public static void SetPlayButton(out bool isPlayingAnimation)
        {
            //open animator
            System.Type animatorWindowType = null;
            EditorWindow animatorWindow = ExtReflection.OpenEditorWindow(ExtReflection.AllNameAssemblyKnown.AnimatorControllerTool, ref animatorWindowType);

            //open animation
            System.Type animationWindowType = null;
            EditorWindow animationWindowEditor = ExtReflection.OpenEditorWindow(ExtReflection.AllNameAssemblyKnown.AnimationWindow, ref animationWindowType);

            //Get field m_AnimEditor
            FieldInfo animEditorFI = animationWindowType.GetField("m_AnimEditor", ExtReflection.GetFullBinding());

            //Get the propertue of animEditorFI
            PropertyInfo controlInterfacePI = animEditorFI.FieldType.GetProperty("controlInterface", ExtReflection.GetFullBinding());

            //Get property i splaying or not
            PropertyInfo isPlaying = controlInterfacePI.PropertyType.GetProperty("playing", ExtReflection.GetFullBinding());

            //get object controlInterface
            object controlInterface = controlInterfacePI.GetValue(animEditorFI.GetValue(animationWindowEditor));
            //get the state of the "play" button
            isPlayingAnimation = (bool)isPlaying.GetValue(controlInterface);

            if (!isPlayingAnimation)
            {
                MethodInfo playMI = controlInterfacePI.PropertyType.GetMethod("StartPlayback", ExtReflection.GetFullBinding());
                playMI.Invoke(controlInterface, new object[0]);
            }
            else
            {
                MethodInfo playMI = controlInterfacePI.PropertyType.GetMethod("StopPlayback", ExtReflection.GetFullBinding());
                playMI.Invoke(controlInterface, new object[0]);
            }
        }

        /// <summary>
        /// hide/show the cursor
        /// </summary>
        public static bool Hidden
        {
            get
            {
                Type type = typeof(Tools);
                FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
                return ((bool)field.GetValue(null));
            }
            set
            {
                Type type = typeof(Tools);
                FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
                field.SetValue(null, value);
            }
        }



        public static void PreventUnselect(GameObject currentTarget)
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                Selection.activeGameObject = null;
            }
            else
            {
                if (EditorWindow.focusedWindow == SceneView.currentDrawingSceneView)
                {
                    Selection.activeGameObject = currentTarget;
                }
            }
        }

        /// <summary>
        /// Open of focus an editor Window
        /// if canDuplicate = true, duplicate the window if it existe
        /// </summary>
        public static T OpenEditorWindow<T>(bool canDuplicate = false) where T : EditorWindow
        {
            T raceTrackNavigator;

            if (canDuplicate)
            {
                raceTrackNavigator = ScriptableObject.CreateInstance<T>();
                raceTrackNavigator.Show();
                return (raceTrackNavigator);
            }

            // create a new instance
            raceTrackNavigator = EditorWindow.GetWindow<T>();

            // show the window
            raceTrackNavigator.Show();
            return (raceTrackNavigator);
        }

        public static void CloseEditorWindow<T>() where T : EditorWindow
        {
            T raceTrackNavigator = EditorWindow.GetWindow<T>();
            raceTrackNavigator.Close();
        }


        /// <summary>
        /// from a given name, Open an EditorWindow.
        /// you can also do:
        /// EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        /// to open an known EditorWindow
        /// 
        /// To get the type of a known script, like UnityEditor.SceneHierarchyWindow, you can do also:
        /// System.Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        /// </summary>
        /// <param name="editorWindowName">name of the editorWindow to open</param>
        /// <param name="animationWindowType">type of the editorWindow (useful for others functions)</param>
        /// <returns></returns>
        public static EditorWindow OpenEditorWindow(AllNameAssemblyKnown editorWindowName, ref System.Type animationWindowType)
        {
            animationWindowType = GetEditorWindowTypeByName(editorWindowName.ToString());
            EditorWindow animatorWindow = EditorWindow.GetWindow(animationWindowType);
            return (animatorWindow);
        }

        /// <summary>
        /// collapse an object
        /// </summary>
        public static void Collapse(GameObject go, bool collapse)
        {
            // bail out immediately if the go doesn't have children
            if (go.transform.childCount == 0)
                return;

            if (collapse)
            {
                EditorGUIUtility.PingObject(go.transform.GetChild(0).gameObject);
                Selection.activeObject = go;
            }
            else
            {
                SetExpandedRecursive(go, false);
            }
        }

        /// <summary>
        /// expand recursivly a hierarchy foldout
        /// </summary>
        /// <param name="go"></param>
        /// <param name="expand"></param>
        public static void SetExpandedRecursive(GameObject go, bool expand)
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var methodInfo = type.GetMethod("SetExpandedRecursive");

            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            var window = EditorWindow.focusedWindow;

            methodInfo.Invoke(window, new object[] { go.GetInstanceID(), expand });
        }

        [MenuItem("CONTEXT/Component/Collapse All")]
        private static void CollapseAll(MenuCommand command)
        {
            SetAllInspectorsExpanded((command.context as Component).gameObject, false);
        }

        [MenuItem("CONTEXT/Component/Expand All")]
        private static void ExpandAll(MenuCommand command)
        {
            SetAllInspectorsExpanded((command.context as Component).gameObject, true);
        }

        /*
        public static void Expand(GameObject go, bool expanded)
        {
            InternalEditorUtility.SetIsInspectorExpanded(go, expanded);
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
        */

        public static void SetAllInspectorsExpanded(GameObject go, bool expanded)
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component is Renderer)
                {
                    var mats = ((Renderer)component).sharedMaterials;
                    for (int i = 0; i < mats.Length; ++i)
                    {
                        InternalEditorUtility.SetIsInspectorExpanded(mats[i], expanded);
                    }
                }
                InternalEditorUtility.SetIsInspectorExpanded(component, expanded);
            }
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        /// <summary>
        /// repaint an Editor
        /// use: RepaintInspector(typeof(SomeTypeInspector));
        /// </summary>
        /// <param name="t"></param>
        public static void RepaintInspector(System.Type t)
        {
            Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; i++)
            {
                if (ed[i].GetType() == t)
                {
                    ed[i].Repaint();
                    return;
                }
            }
        }


        /////////////////////////utility reflexion

        /// <summary>
        /// for adding, do a GetAllEditorWindowTypes(true);
        /// </summary>
        public enum AllNameAssemblyKnown
        {
            BuildPlayerWindow,
            ConsoleWindow,
            IconSelector,
            ObjectSelector,
            ProjectBrowser,
            ProjectTemplateWindow,
            RagdollBuilder,
            SceneHierarchySortingWindow,
            SceneHierarchyWindow,
            ScriptableWizard,
            AddCurvesPopup,
            AnimationWindow,
            CurveEditorWindow,
            MinMaxCurveEditorWindow,
            AnnotationWindow,
            LayerVisibilityWindow,
            AssetStoreInstaBuyWindow,
            AssetStoreLoginWindow,
            AssetStoreWindow,
            AudioMixerWindow,
            CollabPublishDialog,
            CollabCannotPublishDialog,
            GameView,
            AboutWindow,
            AssetSaveDialog,
            BumpMapSettingsFixingWindow,
            ColorPicker,
            EditorUpdateWindow,
            FallbackEditorWindow,
            GradientPicker,
            PackageExport,
            PackageImport,
            PopupWindow,
            PopupWindowWithoutFocus,
            PragmaFixingWindow,
            SaveWindowLayout,
            DeleteWindowLayout,
            EditorToolWindow,
            SnapSettings,
            TreeViewTestWindow,
            GUIViewDebuggerWindow,
            InspectorWindow,
            PreviewWindow,
            AddShaderVariantWindow,
            AddComponentWindow,
            AdvancedDropdownWindow,
            LookDevView,
            AttachToPlayerPlayerIPWindow,
            HolographicEmulationWindow,
            FrameDebuggerWindow,
            SearchableEditorWindow,
            LightingExplorerWindow,
            LightingWindow,
            LightmapPreviewWindow,
            NavMeshEditorWindow,
            OcclusionCullingWindow,
            PhysicsDebugWindow,
            TierSettingsWindow,
            SceneView,
            SettingsWindow,
            ProjectSettingsWindow,
            PreferenceSettingsWindow,
            PackerWindow,
            SpriteUtilityWindow,
            TroubleshooterWindow,
            UIElementsEditorWindowCreator,
            UndoWindow,
            UnityConnectConsentView,
            UnityConnectEditorWindow,
            MetroCertificatePasswordWindow,
            MetroCreateTestCertificateWindow,
            WindowChange,
            WindowCheckoutFailure,
            WindowPending,
            WindowResolve,
            WindowRevert,
            WebViewEditorStaticWindow,
            WebViewEditorWindow,
            WebViewEditorWindowTabs,
            SearchWindow,
            LicenseManagementWindow,
            PackageManagerWindow,
            ParticleSystemWindow,
            PresetSelector,
            ProfilerWindow,
            UISystemPreviewWindow,
            ConflictResolverWindow,
            DeleteShortcutProfileWindow,
            PromptWindow,
            ShortcutManagerWindow,
            SketchUpImportDlg,
            TerrainWizard,
            ImportRawHeightmap,
            ExportRawHeightmap,
            TreeWizard,
            DetailMeshWizard,
            DetailTextureWizard,
            PlaceTreeWizard,
            FlattenHeightmap,
            TestEditorWindow,
            PanelDebugger,
            UIElementsDebugger,
            PainterSwitcherWindow,
            AllocatorDebugger,
            UIRDebugger,
            UIElementsSamples,
            AnimatorControllerTool,
            LayerSettingsWindow,
            ParameterControllerEditor,
            AddStateMachineBehaviourComponentWindow,
            AndroidKeystoreWindow,
            TimelineWindow,
            TMP_ProjectConversionUtility,
            TMP_SpriteAssetImporter,
            TMPro_FontAssetCreatorWindow,
            CollabHistoryWindow,
            CollabToolbarWindow,
            TestRunnerWindow,
            TMP_PackageResourceImporterWindow,

            ConsoleE_Window,
        }


        public enum AllNameEditorWindowKnown
        {
            Lighting,
            Game,
            Scene,
            Hierarchy,
            Project,
            Inspector
        }

        /// <summary>
        /// Get all editor window type.
        /// If we want just the one open, we can do just:
        /// EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
        /// System.Type[] allUnityWindow = UtilityEditor.GetAllEditorWindowTypes(true);
        /// </summary>
        /// <returns></returns>
        private static System.Type[] GetAllEditorWindowTypes(bool showInConsol = false)
        {
            var result = new System.Collections.Generic.List<System.Type>();
            System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
            System.Type editorWindow = typeof(EditorWindow);
            foreach (var A in AS)
            {
                System.Type[] types = A.GetTypes();
                foreach (var T in types)
                {
                    if (T.IsSubclassOf(editorWindow))
                    {
                        result.Add(T);
                        if (showInConsol)
                        {
                            Debug.Log(T.Name);
                        }
                    }

                }
            }
            return result.ToArray();
        }

        public static System.Type GetEditorWindowTypeByName(string editorToFind, bool showDebug = false)
        {
            var result = new System.Collections.Generic.List<System.Type>();
            System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
            System.Type editorWindow = typeof(EditorWindow);
            foreach (var A in AS)
            {
                System.Type[] types = A.GetTypes();
                foreach (var T in types)
                {
                    if (T.IsSubclassOf(editorWindow))
                    {
                        if (showDebug)
                        {
                            Debug.Log(T.Name);
                        }
                        if (T.Name.Equals(editorToFind))
                        {
                            return (T);
                        }
                    }

                }
            }
            return (null);
        }


        /// <summary>
        /// System.Type animationWindowType = ExtReflexion.GetTypeFromAssembly("AnimationWindow", editorAssembly);
        /// </summary>
        private static MethodInfo[] GetAllMethodeOfType(System.Type type, System.Reflection.BindingFlags bindings, bool showInConsol = false)
        {
            MethodInfo[] allMathod = type.GetMethods(bindings);
            if (showInConsol)
            {
                for (int i = 0; i < allMathod.Length; i++)
                {
                    Debug.Log(allMathod[i].Name);
                }
            }
            return (allMathod);
        }

        public static FieldInfo[] GetAllFieldOfType(System.Type type, System.Reflection.BindingFlags bindings, bool showInConsol = false)
        {
            FieldInfo[] allField = type.GetFields(bindings);
            if (showInConsol)
            {
                for (int i = 0; i < allField.Length; i++)
                {
                    Debug.Log(allField[i].Name);
                }
            }
            return (allField);
        }

        public static PropertyInfo[] GetAllpropertiesOfType(System.Type type, System.Reflection.BindingFlags bindings, bool showInConsol = false)
        {
            PropertyInfo[] allProperties = type.GetProperties(bindings);
            if (showInConsol)
            {
                for (int i = 0; i < allProperties.Length; i++)
                {
                    Debug.Log(allProperties[i].Name);
                }
            }
            return (allProperties);
        }

        /// <summary>
        /// show all opened window
        /// </summary>
        private static EditorWindow[] GetAllOpennedWindow(bool showInConsol = false)
        {
            EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

            if (showInConsol)
            {
                for (int i = 0; i < allWindows.Length; i++)
                {
                    Debug.Log(allWindows[i].titleContent.text);
                }
            }
            return (allWindows);
        }

        private static EditorWindow GetOpennedWindowByName(string editorToFind)
        {
            EditorWindow[] allWIndow = GetAllOpennedWindow();
            for (int i = 0; i < allWIndow.Length; i++)
            {
                if (allWIndow[i].titleContent.text.Equals(editorToFind))
                {
                    return (allWIndow[i]);
                }
            }
            return (null);
        }

        /// <summary>
        /// Get all the most common Binding type of elements
        /// </summary>
        /// <returns></returns>
        public static System.Reflection.BindingFlags GetFullBinding()
        {
            return (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// System.Reflection.Assembly editorAssembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
        /// GetTypeFromAssembly("AnimationWindow", editorAssembly);
        /// </summary>
        /// <returns></returns>
        private static System.Type GetTypeFromAssembly(string typeName, System.Reflection.Assembly assembly, System.StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase, bool showNames = false)
        {
            if (assembly == null)
                return (null);

            System.Type[] types = assembly.GetTypes();
            foreach (System.Type type in types)
            {
                if (showNames)
                {
                    Debug.Log(type.Name);
                }
                if (type.Name.Equals(typeName, ignoreCase) || type.Name.Contains('+' + typeName))
                    return (type);
            }
            return (null);
        }

    }
}