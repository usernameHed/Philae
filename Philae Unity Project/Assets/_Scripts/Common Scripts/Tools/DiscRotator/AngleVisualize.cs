using extUnityComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace hedCommon.tools
{
    /// <summary>
    /// creat an arc for visualizing FOV or whatever
    /// </summary>
    public class AngleVisualize : MonoBehaviour, IEditorOnly
    {
        public bool EnableVisual = true;

        public float radius = 4f;
        public float angle = 90f;
        public Color colorFov = Color.red;

        public Vector3 DefaultUp = new Vector3(0, 0, 0);

        public Component GetReference()
        {
            return (this);
        }
    }
}