using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace philae.editor
{
    public class EditorContants
    {
        //main constants
        public const int FLOAT_FIELDS_MAW_WIDTH = 40;
        public const string IS_EDITOR_SCENE_VIEW_ACTIVE = "IS_EDITOR_SCENE_VIEW_ACTIVE";

        /// <summary>
        /// preference of tinyWindow of the main scene view
        /// </summary>
        public class EditorTinyWindowInSceneViewPreference
        {
            public const string KEY_MAIN_TOOLS_NAVIGATOR = "KEY_MAIN_TOOLS_NAVIGATOR";
            public const string KEY_ACTION_NAVIGATOR = "KEY_ACTION_NAVIGATOR";
        }

        public class EditorOpenPreference
        {
            public const string BASIC_EDITOR_WINDOW_IS_OPEN = "BASIC_EDITOR_WINDOW_IS_OPEN";
        }

        /// <summary>
        /// all preference of tinyWindow of all custom editor of gameObjects
        /// </summary>
        public class EditorTinyWindowPreference
        {
            public const string KEY_EDITOR_PREF_SPLINE_CONTROLLER = "KEY_EDITOR_PREF_SPLINE_CONTROLLER";
        }

        public static void DeleteAllKey()
        {
            EditorPrefs.DeleteKey(EditorTinyWindowInSceneViewPreference.KEY_MAIN_TOOLS_NAVIGATOR);
            EditorPrefs.DeleteKey(EditorTinyWindowInSceneViewPreference.KEY_ACTION_NAVIGATOR);
        }

    }
}