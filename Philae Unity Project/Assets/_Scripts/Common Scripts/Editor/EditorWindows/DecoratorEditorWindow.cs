using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using hedCommon.time;

namespace hedCommon.extension.editor.editorWindow
{
    public abstract class DecoratorEditorWindow : EditorWindow
    {
        protected bool _allowScrolling = true;
        protected GUIStyle _textStyle = new GUIStyle();

        private bool _isFirstGUI = false;
        private Vector2 _scrollPosition;
        private EditorChronoWithNoTimeEditor _refreshRateInspector = new EditorChronoWithNoTimeEditor();
        protected float _timeRefreshInspector = 1f;

        #region virtual function
        protected virtual void OnCustomEnable()
        {
            EditorApplication.update += UpdateEditor;
            //EditorPrefs.SetBool(EditorContants.EditorOpenPreference.KEY_EDITOR_PREF_SCRUB_IS_OPEN, true);
        }
        protected virtual void OnCustomDestroy()
        {
            EditorApplication.update -= UpdateEditor;
            //EditorPrefs.SetBool(EditorContants.EditorOpenPreference.KEY_EDITOR_PREF_SCRUB_IS_OPEN, false);
        }
        protected virtual void OnCustomDisable()
        {
            EditorApplication.update -= UpdateEditor;
        }

        /// <summary>
        /// override it with "new" keyword
        /// </summary>
        [MenuItem("PERSO/DecoratorWindow")]
        public static void ShowEditorWindow()
        {
            DecoratorEditorWindow window = EditorWindow.GetWindow<DecoratorEditorWindow>("DecoratorEditorWindow");
            window.InitConstructor();
        }

        protected virtual void InitOnCustomGUI() { }
        protected virtual void OnCustomGUI() { }

        protected virtual void UpdateEditor() { }

        #endregion


        private void OnEnable()
        {
            OnCustomEnable();
        }

        private void OnDestroy()
        {
            OnCustomDestroy();
        }

        private void OnDisable()
        {
            OnCustomDisable();
        }

        /// <summary>
        /// cosntructor
        /// </summary>
        public DecoratorEditorWindow()
        {

        }

        /// <summary>
        /// can't be a constructor
        /// </summary>
        public virtual void InitConstructor()
        {
            ActualizeOnCreationOrAfterCompiling();
            this.wantsMouseMove = true;
        }

        protected virtual void SetMinSize(Vector2 minSize)
        {
            this.minSize = minSize;
        }

        protected virtual void SetMaxSize(Vector2 maxSize)
        {
            this.maxSize = maxSize;
        }

        /// <summary>
        /// called every time we need to actualize the editor (at start, and after compilation)
        /// </summary>
        protected virtual void ActualizeOnCreationOrAfterCompiling()
        {
            _isFirstGUI = false;
        }

        private /*sealed */ void OnGUI()
        {
            if (!_isFirstGUI)
            {
                InitOnCustomGUI();
                _isFirstGUI = true;
            }
            bool allosScroll = _allowScrolling;

            if (allosScroll)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            }

            OnCustomGUI();

            if (allosScroll)
            {
                EditorGUILayout.EndScrollView();
            }
        }

        public void OnInspectorUpdate()
        {
            if (_timeRefreshInspector == 0)
            {
                Repaint();
            }
            else
            {
                // This will only get called 2 times per second.
                if (!_refreshRateInspector.IsRunning())
                {
                    Repaint();
                    _refreshRateInspector.StartChrono(_timeRefreshInspector);
                }
            }
        }
    }
}