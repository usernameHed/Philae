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
    public static class OnSceneViewContextClick
    {
        private const int MAX_OBJ_FOUND = 20;
        private const string LEVEL_SEPARATOR = "    ";
        private const string LEVEL_SEPARATOR_END = "  └─ ";
        private const string PREFAB_TAG = "▉ ";

        private static bool _clickDown = false;
        private static Vector2 _clickDownPosition;
        private static MethodInfo _internalPickClosestGameObject;

        static OnSceneViewContextClick()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }
            SceneView.duringSceneGui += OnSceneGUI;
            FindMethodByReflection();
        }

        /// <summary>
        /// get the right click on sceneView
        /// </summary>
        /// <param name="sceneView"></param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (ExtEventEditor.IsRightMouseDown(Event.current))
            {
                _clickDownPosition = e.mousePosition;
                _clickDown = true;
            }
            else if (ExtEventEditor.IsRightMouseUp(Event.current) && _clickDown)
            {
                _clickDown = false;
                if (_clickDownPosition == e.mousePosition)
                {
                    OpenContextMenu(e.mousePosition, sceneView);
                }
            }
        }

        /// <summary>
        /// attempt to create a contextMenu
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="sceneView"></param>
        private static void OpenContextMenu(Vector2 pos, SceneView sceneView)
        {
            Vector2 invertedPos = new Vector2(pos.x, sceneView.position.height - 16 - pos.y);
            GenericMenu contextMenu = new GenericMenu();

            Dictionary<Transform, List<Transform>> parentChildsDict = new Dictionary<Transform, List<Transform>>();
            GameObject[] currArray = null;
            GenerateDictionaryOfParentAndChilds(sceneView, invertedPos, parentChildsDict, ref currArray);

            foreach (var parentChild in parentChildsDict.Where(keyValue => keyValue.Key.parent == null))
            {
                CreateMenuRecu(contextMenu, parentChild.Key, "", parentChildsDict);
            }

            if (parentChildsDict.Count == 0)
            {
                return;
            }

            DisplayDropDownMenu(pos, contextMenu);
        }

        /// <summary>
        /// from click position, generate all parent childs necessary to construct the popup
        /// </summary>
        /// <param name="sceneView"></param>
        /// <param name="invertedPos"></param>
        /// <param name="parentChildsDict"></param>
        /// <param name="currArray"></param>
        private static void GenerateDictionaryOfParentAndChilds(SceneView sceneView, Vector2 invertedPos, Dictionary<Transform, List<Transform>> parentChildsDict, ref GameObject[] currArray)
        {
            GameObject obj;

            for (int i = 0; i <= MAX_OBJ_FOUND; i++)
            {
                if (parentChildsDict.Count > 0)
                {
                    currArray = new GameObject[parentChildsDict.Count];
                    int arrayIndex = 0;
                    foreach (var parent in parentChildsDict)
                    {
                        currArray[arrayIndex] = parent.Key.gameObject;
                        arrayIndex++;
                    }
                }

                obj = PickObjectOnPos(sceneView.camera, ~0, invertedPos, currArray, null, out int matIndex);
                if (obj == null)
                {
                    break;
                }

                parentChildsDict[obj.transform] = new List<Transform>(MAX_OBJ_FOUND);

                Transform currentParent = obj.transform.parent;
                Transform lastParent = obj.transform;
                List<Transform> currentChilds = new List<Transform>(MAX_OBJ_FOUND);
                while (currentParent != null)
                {
                    if (parentChildsDict.TryGetValue(currentParent, out currentChilds))
                        currentChilds.Add(lastParent);
                    else
                        parentChildsDict.Add(currentParent, new List<Transform>() { lastParent });

                    lastParent = currentParent;
                    currentParent = currentParent.parent;
                }
            }
        }

        /// <summary>
        /// Use the event only if we create the dropDown menu !
        /// </summary>
        /// <param name="pos">position of dropDown</param>
        /// <param name="contextMenu">menu to show</param>
        private static void DisplayDropDownMenu(Vector2 pos, GenericMenu contextMenu)
        {
            Event.current.Use();
            contextMenu.DropDown(new Rect(pos, Vector2.zero));
        }

        /// <summary>
        /// generate a GenericMenu recursivly by looping thought transforms & childs
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="current"></param>
        /// <param name="currentPath"></param>
        /// <param name="parentChilds"></param>
        private static void CreateMenuRecu(GenericMenu menu, Transform current, string currentPath, Dictionary<Transform, List<Transform>> parentChilds)
        {
            AddMenuItem(menu, currentPath + LEVEL_SEPARATOR_END + (ExtPrefabs.IsPrefab(current.gameObject) ? PREFAB_TAG : "") + current.name, current);
            List<Transform> childs;
            if (!parentChilds.TryGetValue(current, out childs))
            {
                return;
            }
            if (childs == null)
            {
                return;
            }
            foreach (Transform child in childs)
            {
                CreateMenuRecu(menu, child, currentPath + LEVEL_SEPARATOR, parentChilds);
            }
        }

        private static GameObject PickObjectOnPos(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex) // PICK A GAMEOBJECT FROM SCENE VIEW AT POSITION
        {
            materialIndex = -1;
            return (GameObject)_internalPickClosestGameObject.Invoke(null, new object[] { cam, layers, position, ignore, filter, materialIndex });
        }

        //CONTEXT MENU

        private static void AddMenuItem(GenericMenu menu, string menuPath, Transform asset) //ADD ITEM TO MENU
        {
            menu.AddItem(new GUIContent(menuPath), false, OnItemSelected, asset);
        }

        private static void OnItemSelected(object itemSelected) // ON CLICK ITEM ON LIST
        {
            if (itemSelected != null)
            {
                Selection.activeTransform = itemSelected as Transform;
            }
        }

        //REFLECTION
        

        private static void FindMethodByReflection()
        {
            Assembly editorAssembly = typeof(Editor).Assembly;
            System.Type handleUtilityType = editorAssembly.GetType("UnityEditor.HandleUtility");
            _internalPickClosestGameObject = handleUtilityType.GetMethod("Internal_PickClosestGO", BindingFlags.Static | BindingFlags.NonPublic);
        }
    }
}