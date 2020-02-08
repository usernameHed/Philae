using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using hedCommon.extension.runtime;

namespace hedCommon.extension.editor
{
    public class ExtUtilityEditor : ScriptableObject
    {
        public class HitSceneView
        {
            public GameObject objHit;
            public Vector3 pointHit;
            public Vector3 normal;
            public Ray Ray;
            public RaycastHit Hit;
            public RaycastHit2D Hit2d;
        }

       

        /// <summary>
        /// Raycast from mouse Position in scene view and save object hit
        /// </summary>
        /// <param name="newHit">return the info of the hit</param>
        /// <param name="setNullIfNot">if true, set back the last hitObject to null</param>
        public static HitSceneView SetCurrentOverObject(HitSceneView newHit, bool setNullIfNot = true)
        {
            if (Event.current == null)
            {
                return (newHit);
            }
            Vector2 mousePos = Event.current.mousePosition;
            if (mousePos.x < 0 || mousePos.x >= Screen.width || mousePos.y < 0 || mousePos.y >= Screen.height)
            {
                return (newHit);
            }

            RaycastHit _saveRaycastHit;
            Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePos);
            newHit.Ray = worldRay;
            //first a test only for point
            //if (Physics.Raycast(worldRay, out saveRaycastHit, Mathf.Infinity, 1 << LayerMask.NameToLayer(gravityAttractorEditor.layerPoint), QueryTriggerInteraction.Ignore))
            if (Physics.Raycast(worldRay, out _saveRaycastHit, Mathf.Infinity))
            {
                if (_saveRaycastHit.collider.gameObject != null)
                {
                    newHit.objHit = _saveRaycastHit.collider.gameObject;
                    newHit.pointHit = _saveRaycastHit.point;
                    newHit.normal = _saveRaycastHit.normal;
                    newHit.Hit = _saveRaycastHit;
                }
            }
            else
            {
                RaycastHit2D raycast2d = Physics2D.Raycast(worldRay.GetPoint(0), worldRay.direction);
                if (raycast2d.collider != null)
                {
                    newHit.objHit = raycast2d.collider.gameObject;
                    newHit.pointHit = raycast2d.point;
                    newHit.normal = raycast2d.normal;
                    newHit.Hit2d = raycast2d;
                }
                else
                {

                    if (setNullIfNot)
                    {
                        newHit.objHit = null;
                        newHit.pointHit = ExtVector3.GetNullVector();
                    }
                }
            }
            return (newHit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static List<T> GetListSerializedProperty<T>(SerializedProperty property) where T : class
        {
            List<T> newList = new List<T>();
            int size = property.arraySize;
            for (int i = 0; i < size; i++)
            {
                newList.Add(property.GetArrayElementAtIndex(i).objectReferenceValue as T);
            }
            return (newList);
        }

        /// <summary>
        /// update the Hierarchy window
        /// </summary>
        public static void UpdateHierarchyWindow()
        {
            EditorApplication.RepaintHierarchyWindow();
        }

        public static void UpdateProjectWindow()
        {
            AssetDatabase.Refresh();
        }

        

        [MenuItem("PERSO/Ext/CreateEmptyParent #e")]
        public static void CreateEmptyParent()
        {
            if (!Selection.activeGameObject)
                return;
            GameObject newParent = new GameObject("Parent of " + Selection.activeGameObject.name);
            int indexFocused = Selection.activeGameObject.transform.GetSiblingIndex();
            newParent.transform.SetParent(Selection.activeGameObject.transform.parent);
            newParent.transform.position = Selection.activeGameObject.transform.position;

            Selection.activeGameObject.transform.SetParent(newParent.transform);
            newParent.transform.SetSiblingIndex(indexFocused);

            Selection.activeGameObject = newParent;
            ExtReflection.SetExpandedRecursive(newParent, true);
        }

        [MenuItem("PERSO/Ext/DeleteEmptyParent %&e")]
        public static void DeleteEmptyParent()
        {
            if (!Selection.activeGameObject)
                return;

            int sibling = Selection.activeGameObject.transform.GetSiblingIndex();
            Transform parentOfParent = Selection.activeGameObject.transform.parent;
            Transform firstChild = Selection.activeGameObject.transform.GetChild(0);
            while (Selection.activeGameObject.transform.childCount > 0)
            {
                Transform child = Selection.activeGameObject.transform.GetChild(0);
                child.SetParent(parentOfParent);
                child.SetSiblingIndex(sibling);
                sibling++;
            }
            DestroyImmediate(Selection.activeGameObject);

            Selection.activeGameObject = firstChild.gameObject;
        }
    }
}