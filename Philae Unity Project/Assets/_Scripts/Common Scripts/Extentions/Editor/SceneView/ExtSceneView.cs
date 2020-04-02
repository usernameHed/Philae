using UnityEngine;
using UnityEditor;
using hedCommon.extension.runtime;

namespace hedCommon.extension.editor.sceneView
{
    /// <summary>
    /// scene view calculation
    /// </summary>
    public class ExtSceneView : ScriptableObject
    {
        /// <summary>
        /// focus on object and zoom
        /// </summary>
        /// <param name="objToFocus"></param>
        /// <param name="zoom"></param>
        public static void FocusOnSelection(GameObject objToFocus, float zoom = -1f)
        {
            FocusOnSelection(objToFocus.transform.position, zoom);
        }

        public static void FocusOnSelection(Vector3 position, float zoom = -1f)
        {
            if (ExtVector3.IsClose(position, Vector3.zero, 0.1f))
            {
                return;
            }

            if (SceneView.lastActiveSceneView == null)
            {
                return;
            }

            SceneView.lastActiveSceneView.LookAt(position);
            if (zoom != -1)
            {
                ExtSceneView.ViewportPanZoomIn(zoom);
            }
        }

        public static void Frame(Vector3 position)
        {
            SceneView.lastActiveSceneView.Frame(new Bounds(position, new Vector3(2, 2, 2)));
        }

        public static void Repaint()
        {
            if (SceneView.lastActiveSceneView == null)
            {
                return;
            }
            SceneView.lastActiveSceneView.Repaint();
        }

        /// <summary>
        /// display 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="toDisplay"></param>
        public static void DisplayStringIn3D(Vector3 position, string toDisplay, Color color, int fontSize = 20)
        {
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontSize = fontSize;
            textStyle.normal.textColor = color;
            textStyle.alignment = TextAnchor.MiddleCenter;
            Handles.Label(position, toDisplay, textStyle);
        }


        public static Transform GetSceneViewCameraTransform()
        {
            return (SceneView.lastActiveSceneView.camera.gameObject.transform);
        }

        public static Camera Camera()
        {
            return (SceneView.lastActiveSceneView.camera);
        }

        public static void SetGameObjectToPositionOfSceneViewCamera(GameObject gameObject, bool alsoRotate = true, bool dezoomAfter = false)
        {
            gameObject.transform.position = SceneView.lastActiveSceneView.camera.gameObject.transform.position;
            if (alsoRotate)
            {
                gameObject.transform.rotation = SceneView.lastActiveSceneView.rotation;
            }

            if (dezoomAfter)
            {
                Vector3 dirCamera = ExtQuaternion.QuaternionToDir(SceneView.lastActiveSceneView.rotation, Vector3.up);
                SceneView.lastActiveSceneView.LookAt(gameObject.transform.position + dirCamera, SceneView.lastActiveSceneView.camera.transform.rotation);
            }
        }

        /// <summary>
        /// Set the zoom of the camera
        /// </summary>
        public static void ViewportPanZoomIn(float zoom = 5f)
        {
            if (SceneView.lastActiveSceneView.size > zoom)
            {
                SceneView.lastActiveSceneView.size = zoom;
                //SceneView.lastActiveSceneView.pivot = ;
            }

            SceneView.lastActiveSceneView.Repaint();
        }

        public static void PlaceGameObjectInFrontOfSceneView(GameObject go)
        {
            SceneView.lastActiveSceneView.MoveToView(go.transform);
        }

        public static void LockFromUnselect()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        private static Texture2D _staticRectTexture;
        private static GUIStyle _staticRectStyle;

        // Note that this function is only meant to be called from OnGUI() functions.
        public static void GUIDrawRect(Rect position, Color color)
        {
            if (_staticRectTexture == null)
            {
                _staticRectTexture = new Texture2D(1, 1);
            }

            if (_staticRectStyle == null)
            {
                _staticRectStyle = new GUIStyle();
            }

            _staticRectTexture.SetPixel(0, 0, color);
            _staticRectTexture.Apply();

            _staticRectStyle.normal.background = _staticRectTexture;
            GUI.Box(position, GUIContent.none, _staticRectStyle);
        }
    }
}