using hedCommon.extension.runtime;
using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.persistantPlayMode
{
    [InitializeOnLoad]
    public static class PersistantPlayMode
    {
        private static Dictionary<int, bool> _persistantGameObject = new Dictionary<int, bool>(300);
        private static GUIStyle _persistantIcon = null;
        private static bool _needToSetupListOfGameObject = true;
        private static bool _needToUpdateListOfGameObject = false;
        private static EditorChronoWithNoTimeEditor _editorChronoWithNoTimeEditor = new EditorChronoWithNoTimeEditor();
        private static Texture _saveTexture;
        private static Texture _saveTextureGray;

        private static GUIStyle _toggleButtonStyleNormal = null;
        private static GUIStyle _toggleButtonStyleToggled = null;
        private static GUIContent _contentToggle = null;

        private const float LIMIT_BETWEEN_TWO_LOADING = 1f;
        private const string SAVE_TEXTURE = "save";
        private const string SAVE_TEXTURE_GRAY = "save gray";

        /// <summary>
        /// Static constructor subscribes to projectWindowItemOnGUI delegate.
        /// </summary>
        static PersistantPlayMode()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
            EditorApplication.playModeStateChanged += PlayModeChanged;
            EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
            _needToSetupListOfGameObject = true;

            UpdateTextures();

        }


        private static GUIStyle PersistantIcon
        {
            get
            {
                if (_persistantIcon == null)
                {
                    _persistantIcon = new GUIStyle(EditorStyles.label);
                    _persistantIcon.normal.textColor = Color.blue;
                    _persistantIcon.alignment = TextAnchor.MiddleRight;
                }
                return _persistantIcon;
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            _needToUpdateListOfGameObject = true;
        }

        private static void PlayModeChanged(PlayModeStateChange state)
        {
            Debug.Log("play mode: " + state);
            _needToUpdateListOfGameObject = true;
        }

        private static void OnHierarchyWindowChanged()
        {
            _needToUpdateListOfGameObject = true;
        }

        private static void UpdateOrResetDictionnary()
        {
            _editorChronoWithNoTimeEditor.StartChrono(LIMIT_BETWEEN_TWO_LOADING);

            // Check here
            GameObject[] allGameObjects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];

            if (_needToSetupListOfGameObject)
            {
                _persistantGameObject.Clear();
            }

            foreach (GameObject g in allGameObjects)
            {
                _persistantGameObject.AddIfNotContaint(g.GetInstanceID(), false);
            }

            _needToSetupListOfGameObject = false;
            _needToUpdateListOfGameObject = false;
        }

        private static void UpdateTextures()
        {
            _saveTexture = (Texture)EditorGUIUtility.Load("SceneView/" + SAVE_TEXTURE + ".png");
            _saveTextureGray = (Texture)EditorGUIUtility.Load("SceneView/" + SAVE_TEXTURE_GRAY + ".png");

            /*
            if (_toggleButtonStyleNormal == null)
            {
                _toggleButtonStyleNormal = "Button";
                _toggleButtonStyleToggled = new GUIStyle(_toggleButtonStyleNormal);
                _toggleButtonStyleToggled.normal.background = _toggleButtonStyleToggled.active.background;
            }
            */
        }

        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            if ((_needToSetupListOfGameObject || _needToUpdateListOfGameObject) && _editorChronoWithNoTimeEditor.IsNotRunning())
            {
                UpdateOrResetDictionnary();
            }

            if (!Application.isPlaying)
            {
                return;
            }

            try
            {
                // place the icoon to the right of the list:
                Rect r = new Rect(selectionRect);
                r.x = r.width - 30;
                r.width = 18;


                // Draw the texture if it's a light (e.g.)
                bool gotValue = _persistantGameObject.TryGetValue(instanceID, out bool value);
                if (gotValue)
                {
                    if (value)
                    {
                        _contentToggle = new GUIContent(_saveTexture, "this gameObject & all components is saved");
                    }
                    else
                    {
                        _contentToggle = new GUIContent(_saveTextureGray, "clic to save data when leaving play mode");
                    }
                    bool result = GUI.Toggle(r, value, _contentToggle);
                    _persistantGameObject[instanceID] = result;
                }
            }
            catch { }
        }
    }
}