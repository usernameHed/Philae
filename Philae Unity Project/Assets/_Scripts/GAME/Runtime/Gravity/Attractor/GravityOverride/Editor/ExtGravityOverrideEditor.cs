using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor.gravityOverride
{
    public static class ExtGravityOverrideEditor
    {
        public static GravityOverrideDisc DrawDisc(ExtCircle circle, GravityOverrideDisc disc, Quaternion refRotation, out bool hasChanged)
        {
            hasChanged = false;

            bool topFace = disc.Face;
            bool topExtremity = disc.Borders;

            if (topFace)
            {
                Handles.color = new Color(0, 1, 0, 0.5f);
                Handles.DrawSolidDisc(circle.Point, circle.Normal, circle.Radius);
            }
            if (topExtremity)
            {
                Handles.color = Color.black;
                Handles.DrawSolidDisc(circle.Point, circle.Normal, circle.Radius / 10 * 8);
            }

            if (Handles.Button(circle.Point,
                refRotation * Quaternion.LookRotation(Vector3.up),
                circle.Radius,
                circle.Radius, Handles.CircleHandleCap))
            {
                Debug.Log("extremity pressed");
                disc.Borders = !disc.Borders;

                Event.current.Use();
            }

            if (Handles.Button(circle.Point,
                refRotation * Quaternion.LookRotation(Vector3.up),
                circle.Radius / 10 * 8,
                circle.Radius / 10 * 8, Handles.CircleHandleCap))
            {
                Debug.Log("Face pressed !");
                disc.Face = !disc.Face;
                Event.current.Use();
            }

            return (disc);
        }
    }
}