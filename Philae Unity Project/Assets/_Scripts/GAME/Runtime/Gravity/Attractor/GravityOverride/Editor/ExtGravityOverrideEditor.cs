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

        ///
        ///      6 ------------ 7
        ///    / |    3       / |
        ///  5 ------------ 8   |       
        ///  |   |          |   |      
        ///  | 5 |     6    | 2 |     ------8-----  
        ///  |   |   1      |   |                   
        ///  |  2 ----------|-- 3                   
        ///  |/       4     | /     |       3      | 
        ///  1 ------------ 4                       
        ///                                         
        ///          6 ------6----- 5 ------2----- 8 -----10----- 7       -       
        ///          |              |              |              |               
        ///          |              |              |              |               
        ///          5      5       1       1      3       2      11       6       |
        ///          |              |              |              |               
        ///          |              |              |              |               
        ///          2 ------7----- 1 ------4----- 4 ------12---- 3       -
        ///                                         
        ///                                         
        ///                         |       4      |  
        ///                                         
        ///                                         
        ///                           ------9-----       
        public static GravityOverrideCube DrawCube(ExtCube cube, GravityOverrideCube cubeGravity, out bool hasChanged)
        {
            hasChanged = false;
            bool changed = hasChanged;
            /*
            cubeGravity.Face1 = DrawRectangle(cube, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Face2 = DrawRectangle(cube, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Face3 = DrawRectangle(cube, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Face4 = DrawRectangle(cube, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Face5 = DrawRectangle(cube, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Face6 = DrawRectangle(cube, out changed);   hasChanged = (changed) ? true : hasChanged;

            cubeGravity.Line1 = DrawLine(cubeGravity.Line1, (cube.P1 + cube.P5) / 2, cube.Rotation, cube.P1, cube.P5, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line2 = DrawLine(cubeGravity.Line2, cube.Position, cube.Rotation, cube.P5, cube.P8, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line3 = DrawLine(cubeGravity.Line3, cube.Position, cube.Rotation, cube.P8, cube.P4, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line4 = DrawLine(cubeGravity.Line4, cube.Position, cube.Rotation, cube.P1, cube.P4, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line5 = DrawLine(cubeGravity.Line5, cube.Position, cube.Rotation, cube.P6, cube.P2, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line6 = DrawLine(cubeGravity.Line6, cube.Position, cube.Rotation, cube.P6, cube.P5, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line7 = DrawLine(cubeGravity.Line7, cube.Position, cube.Rotation, cube.P2, cube.P1, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line8 = DrawLine(cubeGravity.Line8, cube.Position, cube.Rotation, cube.P6, cube.P7, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line9 = DrawLine(cubeGravity.Line9, cube.Position, cube.Rotation, cube.P2, cube.P3, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line10 = DrawLine(cubeGravity.Line10, cube.Position, cube.Rotation, cube.P8, cube.P7, out changed); hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line11 = DrawLine(cubeGravity.Line11, cube.Position, cube.Rotation, cube.P7, cube.P3, out changed); hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Line12 = DrawLine(cubeGravity.Line12, cube.Position, cube.Rotation, cube.P4, cube.P3, out changed); hasChanged = (changed) ? true : hasChanged;

            cubeGravity.Point1 = DrawPoint(cubeGravity.Point1, cube.P1, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Point2 = DrawPoint(cubeGravity.Point2, cube.P2, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Point3 = DrawPoint(cubeGravity.Point3, cube.P3, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Point4 = DrawPoint(cubeGravity.Point4, cube.P4, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Point5 = DrawPoint(cubeGravity.Point5, cube.P5, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Point6 = DrawPoint(cubeGravity.Point6, cube.P6, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Point7 = DrawPoint(cubeGravity.Point7, cube.P7, out changed);   hasChanged = (changed) ? true : hasChanged;
            cubeGravity.Point8 = DrawPoint(cubeGravity.Point8, cube.P8, out changed);   hasChanged = (changed) ? true : hasChanged;
            */
            return (cubeGravity);
        }

        public static GravityOverrideQuad DrawQuad(ExtQuad quad, GravityOverrideQuad quadGravity, out bool hasChanged)
        {
            hasChanged = false;
            return (quadGravity);
        }


        public static GravityOverrideCylinder DrawCylinder(ExtCylinder cylinder, ExtCircle circle1, ExtCircle circle2, GravityOverrideCylinder cylinderGravity, out bool hasChanged)
        {
            hasChanged = false;
            bool changed = hasChanged;

            cylinderGravity.Disc1 = ExtGravityOverrideEditor.DrawDisc(circle1, cylinderGravity.Disc1/*, cylinder.Rotation*/, out changed);
            hasChanged = (changed) ? true : hasChanged;
            cylinderGravity.Disc2 = ExtGravityOverrideEditor.DrawDisc(circle2, cylinderGravity.Disc2/*, cylinder.Rotation*/, out changed);
            hasChanged = (changed) ? true : hasChanged;
            cylinderGravity.Trunk = ExtGravityOverrideEditor.DrawLine(cylinderGravity.Trunk, cylinder.P1, cylinder.P2, out changed);
            hasChanged = (changed) ? true : hasChanged;
            return (cylinderGravity);
        }

        /// <summary>
        /// draw a disc
        /// </summary>
        public static GravityOverrideDisc DrawDisc(ExtCircle circle, GravityOverrideDisc discGravity, out bool hasChanged)
        {
            hasChanged = false;
            Quaternion rotation = ExtQuaternion.QuaternionFromVectorDirector(circle.Normal);
            bool topFace = discGravity.Face;
            bool topExtremity = discGravity.Borders;

            Handles.color = Color.red;
            if (!Event.current.alt && Handles.Button(circle.Point,
                rotation,
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
                rotation,
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

        /// <summary>
        /// draw a line
        /// </summary>
        public static bool DrawLine(bool trunk, Vector3 p1, Vector3 p2, out bool hasChanged)
        {
            hasChanged = false;

            Vector3 direction = (p1 - p2);
            Vector3 middle = ExtVector3.GetMeanOfXPoints(p1, p2);
            Quaternion rotation = ExtQuaternion.QuaternionFromLine(p1, p2);

            float scaleCylinder = direction.magnitude;
            if (!trunk)
            {
                Matrix4x4 scaleMatrix = Matrix4x4.TRS(middle, rotation, new Vector3(0.5f, 0.5f, scaleCylinder));
                using (new Handles.DrawingScope(scaleMatrix))
                {
                    Handles.CylinderHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
                }
            }
            Handles.color = Color.red;
            if (!Event.current.alt && Handles.Button(
                middle,
                rotation,
                scaleCylinder,
                scaleCylinder,
                LineHandleCap))
            {
                Debug.Log("extremity pressed");
                trunk = !trunk;
                hasChanged = true;
                Event.current.Use();
            }
            return (trunk);
        }

        /// <summary>
        /// do a custom Line Handle Cap
        /// </summary>
        public static void LineHandleCap(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Vector3 sideways = rotation * new Vector3(0, 0, size / 2);
            Vector3 p1 = position - sideways;
            Vector3 p2 = position + sideways;

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

        public static bool DrawPoint(bool point, Vector3 position, out bool hasChanged)
        {
            hasChanged = false;
            return (false);
        }

        //end of class
    }
    //end of nameSpace
}