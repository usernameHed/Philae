using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using hedCommon.extension.editor.editorWindow;

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
        public static void SetSearch(string search, ExtEditorWindow.AllNameAssemblyKnown nameEditorWindow = ExtEditorWindow.AllNameAssemblyKnown.SceneView)
        {
            //open animation window
            EditorWindow animationWindowEditor = ExtEditorWindow.OpenEditorWindow(nameEditorWindow, out System.Type animationWindowType);

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
            EditorWindow animatorWindow = ExtEditorWindow.OpenEditorWindow(ExtEditorWindow.AllNameAssemblyKnown.AnimatorControllerTool, out System.Type animatorWindowType);

            //open animation
            EditorWindow animationWindowEditor = ExtEditorWindow.OpenEditorWindow(ExtEditorWindow.AllNameAssemblyKnown.AnimationWindow, out System.Type animationWindowType);

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