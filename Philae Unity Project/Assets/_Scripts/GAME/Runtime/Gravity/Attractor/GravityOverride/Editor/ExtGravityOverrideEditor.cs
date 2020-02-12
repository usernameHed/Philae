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

            Handles.color = Color.red;
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
            Handles.color = Color.red;
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

        public static GravityOverrideCylinder DrawCylinder(ExtCylinder cylinder, ExtCircle circle1, ExtCircle circle2, GravityOverrideCylinder cylinderGravity, out bool hasChanged)
        {
            hasChanged = false;

            cylinderGravity.Disc1 = ExtGravityOverrideEditor.DrawDisc(circle1, cylinderGravity.Disc1, cylinder.Rotation, out hasChanged);
            cylinderGravity.Disc2 = ExtGravityOverrideEditor.DrawDisc(circle2, cylinderGravity.Disc2, cylinder.Rotation, out hasChanged);
            cylinderGravity.Trunk = ExtGravityOverrideEditor.DrawLine(cylinderGravity.Trunk, cylinder.Position, cylinder.Rotation * Quaternion.LookRotation(Vector3.up), cylinder.P1, cylinder.P2, out hasChanged);

            return (cylinderGravity);
        }

        public static bool DrawLine(bool trunk, Vector3 position, Quaternion rotation, Vector3 p1, Vector3 p2, out bool hasChanged)
        {
            hasChanged = false;

            if (!trunk)
            {
                float scaleCylinder = (p1 - p2).magnitude;
                Matrix4x4 scaleMatrix = Matrix4x4.TRS(position, rotation, new Vector3(0.5f, 0.5f, scaleCylinder));
                using (new Handles.DrawingScope(scaleMatrix))
                {
                    Handles.CylinderHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
                }
                /*
                if (Event.current.type == EventType.Repaint)
                {
                    Handles.color = new Color(1, 0, 0, 0.5f);
                    Handles.ArrowHandleCap(0,
                                cylinder.Position,
                                cylinder.Rotation * Quaternion.LookRotation(Vector3.up),
                                cylinder.RealRadius * 2 / 10 * 8, EventType.Repaint);

                }
                */
            }
            Handles.color = Color.red;
            if (!Event.current.alt && Handles.Button(
                position,
                rotation,
                (p1 - p2).magnitude,
                (p1 - p2).magnitude,
                LineHandleCap))
            {
                Debug.Log("extremity pressed");
                trunk = !trunk;
                hasChanged = true;
                Event.current.Use();
            }
            return (trunk);
        }

        public static void LineHandleCap(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Vector3 sideways = rotation * new Vector3(0, 0, size / 2);
            Vector3 p1 = position - sideways;
            Vector3 p2 = position + sideways;
            /*
            Vector3 sidewaysHandle = rotation * new Vector3(0, 0, size * 2);
            Vector3 p1Handle = position - sidewaysHandle;
            Vector3 p2Handle = position + sidewaysHandle;
            */
            switch (eventType)
            {
                case (EventType.Layout):
                    float distance = HandleUtility.DistanceToLine(p1, p2);
                    HandleUtility.AddControl(controlId, distance);
                    break;
                case (EventType.Repaint):

                    Handles.DrawPolyLine(p1, p2);
                    break;
            }
        }
        //end of class
    }
    //end of nameSpace
}