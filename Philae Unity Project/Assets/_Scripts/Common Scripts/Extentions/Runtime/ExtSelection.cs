#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using hedCommon.extension.runtime;

public static class ExtSelection
{
    /// <summary>
    /// return true if one of the parent (or itself), is selected
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsParentItemPresent<T>(T item) where T : Component
    {
        if (Selection.activeGameObject == null)
        {
            return (false);
        }
        GameObject selected = Selection.activeGameObject;
        T camera = selected.GetExtComponentInParents<T>(99, true);
        return (item == camera);
    }

    /// <summary>
    /// return true if one of the parent have the component wanted
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsParentSelectedHaveType<T>(out T component) where T : Component
    {
        component = null;
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            return (false);
        }
        component = selected.GetExtComponentInParents<T>(99, true);
        return (component != null);
    }

    public static void ViewportPanZoomIn(float zoom = 5f)
    {
        //Debug.Log(SceneView.lastActiveSceneView.size);
        if (SceneView.lastActiveSceneView.size > zoom)
            SceneView.lastActiveSceneView.size = zoom;
        SceneView.lastActiveSceneView.Repaint();
    }

    /// <summary>
    /// focus on middle of selection, set zoom to -1 to net zoom
    /// </summary>
    /// <param name="objToFocus"></param>
    /// <param name="zoomAuto"></param>
    /// <param name="zoom"></param>
    public static void FocusOnSelection(List<GameObject> objToFocus, bool zoomAuto = true, float zoom = 5f)
    {
        if (objToFocus.Count < 1)
        {
            return;
        }

        Vector3 sizeBoundingBox = Vector3.zero;
        Vector3 middlePos = ExtVector3.GetMeanOfXPoints(objToFocus.ToArray(), out sizeBoundingBox, true);

        float hightAxis = ExtVector3.GetHighestAxis(sizeBoundingBox) * 2;

        SceneView.lastActiveSceneView.LookAt(middlePos);

        if (objToFocus.Count == 1)
        {
            ViewportPanZoomIn(zoom);
            return;
        }


        if (zoom != -1)
        {
            if (zoomAuto)
            {
                ViewportPanZoomIn(hightAxis);
            }
            else
            {
                ViewportPanZoomIn(zoom);
            }
        }
    }



    public static void FocusOnSelection(GameObject objToFocus, float zoom = 5f, bool allowFocus = true)
    {
        if (!allowFocus)
        {
            return;
        }

        SceneView.lastActiveSceneView.LookAt(objToFocus.transform.position);
        if (zoom != -1)
            ViewportPanZoomIn(zoom);
    }


    public static void FocusOnSelection(GameObject objToFocus, float zoom = 5f)
    {
        SceneView.lastActiveSceneView.LookAt(objToFocus.transform.position);
        if (zoom != -1)
            ViewportPanZoomIn(zoom);
    }

    public static void Ping(Object obj)
    {
        EditorGUIUtility.PingObject(obj);
    }

    public static void Ping(GameObject obj)
    {
        EditorGUIUtility.PingObject(obj);
    }

    public static bool PingAndSelect(Object obj)
    {
        if (obj == null)
            return (false);

        Ping(obj);
        Selection.activeObject = obj;
        return (true);
    }

    public static bool PingAndSelect(GameObject obj)
    {
        if (obj == null)
            return (false);

        EditorGUIUtility.PingObject(obj);
        Selection.activeObject = obj;
        return (true);
    }

    public static void Select(GameObject obj)
    {
        Selection.activeGameObject = obj;
    }

    /// <summary>
    /// lock a gameObject, prevent from being deselected in sceneView
    /// </summary>
    /// <param name="obj"></param>
    public static void LockSelectionGameObject(GameObject obj)
    {
        if (obj != null)
        {
            Selection.activeGameObject = obj.gameObject;
        }
    }
}
#endif