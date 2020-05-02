using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace hedCommon.extension.editor.screenCapture
{
    public static class ExtScreenCapture
    {
        private const string DEFAULT_LOCATION_SCREENSHOTS = "Assets/Resources/ScreenShots/";


        public static Color PickColorAtPosition(Vector2 position)
        {
            return (GetPixelsOfScreen(new Rect(position.x, position.y, 1, 1))[0]);
        }

        public static Texture TakeEditorWindowCapture(this EditorWindow editorWindow)
        {
            return (TakeScreenCapture(editorWindow.position));
        }

        public static Texture TakeScreenCapture(Rect rect)
        {
            int width = (int)rect.width;
            int height = (int)rect.height;
            int x = (int)rect.x;
            int y = (int)rect.y;
            Vector2 position = new Vector2(x, y);

            Color[] pixels = ExtScreenCapture.GetPixelsOfScreen(new Rect(position.x, position.y, width, height));

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        public static Color[] GetPixelsOfScreen(Rect rect)
        {
            Color[] pixels = UnityEditorInternal.InternalEditorUtility.ReadScreenPixel(new Vector2(rect.x, rect.y), (int)rect.width, (int)rect.height);
            return (pixels);
        }

        public static Object SaveScreenCapture(this Texture screenCapture, string nameFile = "ScreenShot", string path = DEFAULT_LOCATION_SCREENSHOTS, bool updateAssetDataBase = true)
        {
            Texture2D screenShot2d = (Texture2D)screenCapture;
            string finalPath = path + nameFile + ".png";
            finalPath = ExtPaths.RenameIncrementalFile(finalPath, out int index, false);
            ExtFileEditor.CreateEntirePathIfNotExist(finalPath);
            screenShot2d.SaveToPNG(finalPath);

            if (updateAssetDataBase)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(finalPath);
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
                return (asset);
            }
            return (null);
        }
    }
}