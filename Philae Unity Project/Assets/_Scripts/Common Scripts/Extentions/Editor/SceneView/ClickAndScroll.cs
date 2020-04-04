using hedCommon.extension.runtime;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace hedCommon.extension.editor.sceneView
{
    /// <summary>
    /// ContectClick
    /// </summary>
    [InitializeOnLoad]
    public static class ClickAndScroll
    {
        private const int MAX_DEPTH = 40;
        private static List<GameObject> _underneethGameObjects = new List<GameObject>(MAX_DEPTH);
        private static MethodInfo _internalPickClosestGameObject;
        private static int _currentIndex = 0;

        static ClickAndScroll()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }
            SceneView.duringSceneGui += OnSceneGUI;
            FindMethodByReflection();
            _currentIndex = 0;
        }


        /// <summary>
        /// Save reference of the Internal_PickClosestGO reflection method
        /// </summary>
        private static void FindMethodByReflection()
        {
            Assembly editorAssembly = typeof(Editor).Assembly;
            System.Type handleUtilityType = editorAssembly.GetType("UnityEditor.HandleUtility");
            _internalPickClosestGameObject = handleUtilityType.GetMethod("Internal_PickClosestGO", BindingFlags.Static | BindingFlags.NonPublic);
        }

        /// <summary>
        /// get the right click on sceneView
        /// </summary>
        /// <param name="sceneView"></param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            if (ExtEventEditor.IsClickOnSceneView(Event.current))
            {
                SaveAllGameObjectUnderneethMouse(Event.current.mousePosition, sceneView);
            }
            else
            {
                AttemptToScroll();
            }
        }

        /// <summary>
        /// attempt to create a contextMenu
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="sceneView"></param>
        private static void SaveAllGameObjectUnderneethMouse(Vector2 pos, SceneView sceneView)
        {
            _underneethGameObjects.Clear();
            Vector2 invertedPos = new Vector2(pos.x, sceneView.position.height - 16 - pos.y);

            for (int i = 0; i <= MAX_DEPTH; i++)
            {
                GameObject clickedGameObject = PickObjectOnPos(sceneView.camera, ~0, invertedPos, _underneethGameObjects.ToArray(), null, out int matIndex);
                if (clickedGameObject != null)
                {
                    _underneethGameObjects.AddIfNotContain(clickedGameObject);
                }
            }
            _currentIndex = 0;
        }

        private static GameObject PickObjectOnPos(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex) // PICK A GAMEOBJECT FROM SCENE VIEW AT POSITION
        {
            materialIndex = -1;
            return (GameObject)_internalPickClosestGameObject.Invoke(null, new object[] { cam, layers, position, ignore, filter, materialIndex });
        }

        private static void AttemptToScroll()
        {
            if (_underneethGameObjects.Count <= 1)
            {
                return;
            }

            bool isScrollingDown = ExtEventEditor.IsScrollingDown(Event.current, out float delta);
            bool isScrollingUp = ExtEventEditor.IsScrollingUp(Event.current, out delta);

            if (!isScrollingDown && !isScrollingUp)
            {
                return;
            }

            bool isShiftHeld = Event.current.shift;
            bool isControlHeld = Event.current.control;
            bool isSpecialKeyHeld = isShiftHeld || isControlHeld;

            if (isSpecialKeyHeld && isScrollingDown)
            {
                if (_currentIndex == 0 && Selection.activeGameObject != _underneethGameObjects[0])
                {
                    _underneethGameObjects.Insert(0, Selection.activeGameObject);
                }
                _currentIndex++;

                SelectItem(isShiftHeld, isControlHeld);
                ExtEventEditor.Use();
            }
            else if (isSpecialKeyHeld && isScrollingUp)
            {
                if (_currentIndex == 0 && Selection.activeGameObject != _underneethGameObjects[0])
                {
                    _underneethGameObjects.Insert(0, Selection.activeGameObject);
                }
                else
                {
                    _currentIndex--;
                }
                SelectItem(isShiftHeld, isControlHeld);
                ExtEventEditor.Use();
            }
        }

        private static void SelectItem(bool isShiftHeld, bool isControlHeld)
        {
            _currentIndex = ExtMathf.SetBetween(_currentIndex, 0, _underneethGameObjects.Count - 1);
            if (isControlHeld)
            {
                ExtSelection.Select(_underneethGameObjects[_currentIndex]);
            }
            if (isShiftHeld)
            {
                ExtSelection.Ping(_underneethGameObjects[_currentIndex]);
            }
        }
    }
}