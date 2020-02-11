using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor.gravityOverride
{
    public static class ExtGravityOverrideEditor
    {
        public static GravityOverrideDisc DrawDisc(ExtCircle circle, GravityOverrideDisc discGravity, Quaternion refRotation, out bool hasChanged)
        {
            hasChanged = false;

            bool topFace = discGravity.Face;
            bool topExtremity = discGravity.Borders;

            Handles.color = Color.clear;
            if (!Event.current.alt && Handles.Button(circle.Point,
                refRotation * Quaternion.LookRotation(Vector3.up),
                circle.Radius,
                circle.Radius, Handles.CircleHandleCap))
            {
                Debug.Log("extremity pressed");
                discGravity.Borders = !discGravity.Borders;
                hasChanged = true;
                Event.current.Use();
            }


            if (!topFace)
            {
                Handles.color = new Color(1, 0, 0, 0.5f);
                Handles.DrawSolidDisc(circle.Point, circle.Normal, circle.Radius / 10 * 8);
            }
            if (!topExtremity)
            {
                Handles.color = new Color(1, 0, 0, 0.5f);
                ExtHandle.DrawCircleThickness(circle, 50, ExtHandle.DrawOutlineType.INSIDE);
            }
            Handles.color = Color.clear;
            if (!Event.current.alt && Handles.Button(circle.Point,
                refRotation * Quaternion.LookRotation(Vector3.up),
                circle.Radius / 10 * 7,
                circle.Radius / 10 * 7, Handles.CircleHandleCap))
            {
                Debug.Log("Face pressed !");
                discGravity.Face = !discGravity.Face;
                hasChanged = true;
                Event.current.Use();
            }

            return (discGravity);
        }

        public static GravityOverrideCylinder DrawCylinder(ExtCylinder cylinder, GravityOverrideCylinder cylinderGravity, out bool hasChanged)
        {
            hasChanged = false;

            if (!cylinderGravity.Trunk)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    Handles.color = new Color(1, 0, 0, 0.5f);
                    Handles.ArrowHandleCap(0,
                                cylinder.Position,
                                cylinder.Rotation * Quaternion.LookRotation(Vector3.up),
                                cylinder.RealRadius * 2 / 10 * 8, EventType.Repaint);

                }
            }

            Handles.color = Color.cyan;
            if (!Event.current.alt && Handles.Button(cylinder.Position,
                cylinder.Rotation * Quaternion.LookRotation(Vector3.up),
                cylinder.RealRadius * 2,
                cylinder.RealRadius, Handles.ArrowHandleCap))
            {
                Debug.Log("extremity pressed");
                cylinderGravity.Trunk = !cylinderGravity.Trunk;

                Event.current.Use();
            }

            return (cylinderGravity);
        }
        //end of class
    }
    //end of nameSpace
}