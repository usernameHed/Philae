using UnityEditor;
using System.IO;
using System.Reflection;
using Type = System.Type;
using static UnityEditor.EditorGUILayout;
using UnityEngine;

namespace hedCommon.extension.editor
{
    public static class ExtLayout
    {
        private static MethodInfo _miLoadWindowLayout;
        private static MethodInfo _miSaveWindowLayout;
        private static MethodInfo _miReloadWindowLayoutMenu;
        private static bool _available;
        private static string _layoutsPath;

        static ExtLayout()
        {
            Type tyWindowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
            Type tyEditorUtility = Type.GetType("UnityEditor.EditorUtility,UnityEditor");
            Type tyInternalEditorUtility = Type.GetType("UnityEditorInternal.InternalEditorUtility,UnityEditor");
            if (tyWindowLayout != null && tyEditorUtility != null && tyInternalEditorUtility != null)
            {
                MethodInfo miGetLayoutsPath = tyWindowLayout.GetMethod("GetLayoutsPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                _miLoadWindowLayout = tyWindowLayout.GetMethod("LoadWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(bool) }, null);
                _miSaveWindowLayout = tyWindowLayout.GetMethod("SaveWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                _miReloadWindowLayoutMenu = tyInternalEditorUtility.GetMethod("ReloadWindowLayoutMenu", BindingFlags.Public | BindingFlags.Static);

                if (miGetLayoutsPath == null || _miLoadWindowLayout == null || _miSaveWindowLayout == null || _miReloadWindowLayoutMenu == null)
                    return;

                _layoutsPath = (string)miGetLayoutsPath.Invoke(null, null);
                if (string.IsNullOrEmpty(_layoutsPath))
                    return;

                _available = true;
            }
        }

        public static void DisplaySaveAndLoadButton()
        {
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;
            using (HorizontalScope HorizontalScope = new HorizontalScope())
            {
                if (GUILayout.Button("Save"))
                {
                    ExtLayout.SaveLayoutToAsset("Assets/Editor Default Resources/Layouts/#Us.wlt");
                    ExtUtilityEditor.UpdateProjectWindow();
                }
                else if (GUILayout.Button("Load"))
                {
                    ExtLayout.LoadLayoutFromAsset("Assets/Editor Default Resources/Layouts/#Us.wlt");
                }
            }
            GUI.backgroundColor = oldColor;
        }

        // Gets a value indicating whether all required Unity API functionality is available for usage.
        public static bool IsAvailable
        {
            get { return _available; }
        }

        // Gets absolute path of layouts directory. Returns `null` when not available.
        public static string LayoutsPath
        {
            get { return _layoutsPath; }
        }

        [MenuItem("PERSO/Layouts/Save Layout")]
        static void SaveLayoutHack()
        {
            // Saving the current layout to an asset
            ExtLayout.SaveLayoutToAsset("Assets/Editor Default Resources/Layouts/#Race.wlt");
        }
        [MenuItem("PERSO/Layouts/Load Layout")]
        static void LoadLayoutHack()
        {
            // Loading layout from an asset
            ExtLayout.LoadLayoutFromAsset("Assets/Editor Default Resources/Layouts/#Race.wlt");
        }

        // Save current window layout to asset file. `assetPath` must be relative to project directory.
        public static void SaveLayoutToAsset(string assetPath)
        {
            SaveLayout(Path.Combine(Directory.GetCurrentDirectory(), assetPath));
        }

        // Load window layout from asset file. `assetPath` must be relative to project directory.
        public static void LoadLayoutFromAsset(string assetPath)
        {
            if (_miLoadWindowLayout != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
                _miLoadWindowLayout.Invoke(null, new object[] { path, true });
            }
        }

        // Save current window layout to file. `path` must be absolute.
        public static void SaveLayout(string path)
        {
            if (_miSaveWindowLayout != null)
                _miSaveWindowLayout.Invoke(null, new object[] { path });
        }
    }
}