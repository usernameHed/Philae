using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtCamera
    {
        public static void IsGameObjectOnCamera(GameObject gameObject, Camera camera, out bool isTotalyVisible, out bool isPartiallyVisible)
        {
            isTotalyVisible = true;
            isPartiallyVisible = false;

            Renderer[] renderers = ExtComponent.GetAllRendererInGameObjects(gameObject, 99, true);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (!renderers[i].isVisible)
                {
                    isTotalyVisible = false;
                }
                else
                {
                    isPartiallyVisible = true;
                }
            }
        }
    }
}