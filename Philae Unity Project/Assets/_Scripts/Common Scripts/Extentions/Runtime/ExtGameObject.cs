using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;


namespace hedCommon.extension.runtime
{
    public static class ExtGameObject
    {
        /// <summary>
        /// Find a GameObject even if it's disabled.
        /// </summary>
        /// <param name="name">The name.</param>
        public static GameObject FindWithDisabled(this GameObject go, string name)
        {
            var temp = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            var obj = new GameObject();
            foreach (GameObject o in temp)
            {
                if (o.name == name)
                {
                    obj = o;
                }
            }
            return obj;
        }

        public static void ActiveGameObjects(bool display, params GameObject[] uiGameObjects)
        {
            for (int i = 0; i < uiGameObjects.Length; i++)
            {
                uiGameObjects[i].SetActive(display);
            }
        }

        public static void DestroyImmediate(UnityEngine.Object[] objectToDelete)
        {
            for (int i = 0; i < objectToDelete.Length; i++)
            {
                DestroyImmediate(objectToDelete[i]);
            }
        }

        public static void DestroyImmediate(UnityEngine.Object objectToDelete)
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(objectToDelete);
            }
            else
            {
                GameObject.DestroyImmediate(objectToDelete);
            }
        }

        #region is parent child
        public static bool IsParentOf(this GameObject parent, GameObject possibleChild)
        {
            if (parent == null || possibleChild == null) return false;
            return possibleChild.transform.IsChildOf(parent.transform);
        }

        public static bool IsParentOf(this Transform parent, GameObject possibleChild)
        {
            if (parent == null || possibleChild == null) return false;
            return possibleChild.transform.IsChildOf(parent);
        }

        public static bool IsParentOf(this GameObject parent, Transform possibleChild)
        {
            if (parent == null || possibleChild == null) return false;
            return possibleChild.IsChildOf(parent.transform);
        }

        public static bool IsParentOf(this Transform parent, Transform possibleChild)
        {
            if (parent == null || possibleChild == null) return false;
            /*
             * Since implementation of this, Unity has since added 'IsChildOf' that is far superior in efficiency
             * 
            while (possibleChild != null)
            {
                if (parent == possibleChild.parent) return true;
                possibleChild = possibleChild.parent;
            }
            return false;
            */

            return possibleChild.IsChildOf(parent);
        }

        public static bool IsChildOf(this GameObject go, Transform potentialParent)
        {
            return (go.transform.IsChildOf(potentialParent));
        }
        public static bool IsChildOf(this GameObject go, GameObject potentialParent)
        {
            return (go.transform.IsChildOf(potentialParent.transform));
        }
        public static bool IsChildOf(this Transform go, GameObject potentialParent)
        {
            return (go.IsChildOf(potentialParent.transform));
        }
        #endregion



        /// <summary>
        /// create a gameObject, with a set of components
        /// ExtGameObject.CreateGameObject("game object name", Transform parent, Vector3.zero, Quaternion.identity, Vector3 Vector.One, Component [] components)
        /// set null at components if no component to add
        /// return the created gameObject
        /// </summary>
        public static GameObject CreateLocalGameObject(string name,
            Transform parent,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale,
            int siblingIndex,
            Component[] components)
        {
            GameObject newObject = new GameObject(name);
            //newObject.SetActive(true);
            newObject.transform.SetParent(parent);
            if (siblingIndex < 0 || siblingIndex > parent.childCount)
            {
                newObject.transform.SetAsLastSibling();
            }
            else
            {
                newObject.transform.SetSiblingIndex(siblingIndex);
            }
            newObject.transform.localPosition = localPosition;
            newObject.transform.localRotation = localRotation;
            newObject.transform.localScale = localScale;

            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    newObject.AddComponent(components[i]);
                }
            }
            return (newObject);
        }

        public static GameObject CreateObjectFromPrefab(GameObject prefabs, Vector3 position, Quaternion rotation, Transform parent)
        {
            return (GameObject.Instantiate(prefabs, position, rotation, parent));
        }

        /// <summary>
        /// hide all renderer
        /// </summary>
        /// <param name="toHide">apply this to a transform, or a gameObject</param>
        /// <param name="hide">hide (or not)</param>
        public static void HideAllRenderer(this Transform toHide, bool hide = true, bool includeInnactive = false)
        {
            HideAllRenderer(toHide.gameObject, hide, includeInnactive);
        }
        public static void HideAllUI(this Transform toHide, bool hide = true, bool includeInnactive = false)
        {
            HideAllUI(toHide.gameObject, hide, includeInnactive);
        }

        /// <summary>
        /// return true if all the meshRender of this gameObject are disabled
        /// </summary>
        /// <param name="gameObjectHided"></param>
        /// <returns></returns>
        public static bool AreAllMeshRenderHide(this GameObject gameObjectHided)
        {
            MeshRenderer[] allMesh = gameObjectHided.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < allMesh.Length; i++)
            {
                if (allMesh[i].enabled)
                {
                    return (false);
                }
            }
            return (true);
        }

        public static void HideAllUI(this GameObject toHide, bool hide = true, bool includeInnactive = false)
        {
            Image[] allImage = toHide.GetComponentsInChildren<Image>(includeInnactive);

            for (int i = 0; i < allImage.Length; i++)
            {
                allImage[i].enabled = !hide;
            }

            TextMeshProUGUI[] allText = toHide.GetComponentsInChildren<TextMeshProUGUI>(includeInnactive);

            for (int i = 0; i < allText.Length; i++)
            {
                allText[i].enabled = !hide;
            }
        }

        public static void HideAllRenderer(this GameObject toHide, bool hide = true, bool includeInnactive = false)
        {
            Renderer[] allrenderer = toHide.GetComponentsInChildren<Renderer>(includeInnactive);

            for (int i = 0; i < allrenderer.Length; i++)
            {
                allrenderer[i].enabled = !hide;
            }
        }
        public static void HideAllComponentOfType<T>(this GameObject toHide, bool hide = true)
            where T : Behaviour
        {
            T[] allrenderer = toHide.GetComponentsInChildren<T>();

            for (int i = 0; i < allrenderer.Length; i++)
            {
                allrenderer[i].enabled = !hide;
            }
        }

        /// <summary>
        /// Returns true if the GO is null or inactive
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool IsNullOrInactive(this GameObject go)
        {
            return ((go == null) || (!go.activeSelf));
        }

        /// <summary>
        /// Returns true if the GO is not null and is active
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool IsActive(this GameObject go)
        {
            return ((go != null) && (go.activeSelf));
        }


        /// <summary>
        /// change le layer de TOUT les enfants
        /// </summary>
        //use: myButton.gameObject.SetLayerRecursively(LayerMask.NameToLayer(“UI”));
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform t in gameObject.transform)
            {
                t.gameObject.SetLayerRecursively(layer);
            }
        }
        /// <summary>
        /// change le layer de TOUT les enfants
        /// </summary>
        //use: myButton.gameObject.SetLayerRecursively(LayerMask.NameToLayer(“UI”));
        public static void SetStaticRecursively(this GameObject gameObject, bool isStatic)
        {
            gameObject.isStatic = isStatic;
            foreach (Transform t in gameObject.transform)
            {
                t.gameObject.SetStaticRecursively(isStatic);
            }
        }

        /// <summary>
        /// activate recursivly the Colliders
        /// use: gameObject.SetCollisionRecursively(false);
        /// </summary>
        public static void SetCollisionRecursively(this GameObject gameObject, bool tf)
        {
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = tf;
            }
        }

        

        

        /// <summary>
        /// Is a gameObject grounded or not ?
        /// </summary>
        /// <param name="target">object to test for grounded</param>
        /// <param name="dirUp">normal up of the object</param>
        /// <param name="distToGround">dist to test</param>
        /// <param name="layerMask">layermask</param>
        /// <param name="queryTriggerInteraction"></param>
        /// <param name="marginDistToGround">aditionnal margin to the distance</param>
        /// <returns></returns>
        public static bool IsGrounded(GameObject target, Vector3 dirUp, float distToGround, int layerMask, QueryTriggerInteraction queryTriggerInteraction, float marginDistToGround = 0.1f)
        {
            return Physics.Raycast(target.transform.position, -dirUp, distToGround + marginDistToGround, layerMask, queryTriggerInteraction);
        }
    }
}