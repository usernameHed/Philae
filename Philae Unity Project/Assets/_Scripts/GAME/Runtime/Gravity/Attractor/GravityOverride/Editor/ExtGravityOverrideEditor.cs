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

        ///     2 ------------- 3 
        ///   /               /   
        ///  1 ------------ 4  
        public static GravityOverrideQuad DrawQuadWithBorders(ExtQuad quad, GravityOverrideQuad quadGravity, out bool hasChanged)
        {
            hasChanged = false;
            bool changed = hasChanged;

            quadGravity.Face1 = DrawQuadFace(quad, quadGravity.Face1, quad.AllowBottom, 0.5f, out changed);     hasChanged = (changed) ? true : hasChanged;

            quadGravity.Line1 = ExtGravityOverrideEditor.DrawLine(quadGravity.Line1, quad.P1, quad.P2, 0.5f, 0.5f, out changed);    hasChanged = (changed) ? true : hasChanged;
            quadGravity.Line2 = ExtGravityOverrideEditor.DrawLine(quadGravity.Line2, quad.P2, quad.P3, 0.5f, 0.5f, out changed);    hasChanged = (changed) ? true : hasChanged;
            quadGravity.Line3 = ExtGravityOverrideEditor.DrawLine(quadGravity.Line3, quad.P3, quad.P4, 0.5f, 0.5f, out changed);    hasChanged = (changed) ? true : hasChanged;
            quadGravity.Line4 = ExtGravityOverrideEditor.DrawLine(quadGravity.Line4, quad.P4, quad.P1, 0.5f, 0.5f, out changed);    hasChanged = (changed) ? true : hasChanged;

            quadGravity.Point1 = ExtGravityOverrideEditor.DrawPoint(quadGravity.Point1, quad.P1, quad.LocalScale.magnitude / 30, 1f, out changed);  hasChanged = (changed) ? true : hasChanged;
            quadGravity.Point2 = ExtGravityOverrideEditor.DrawPoint(quadGravity.Point2, quad.P2, quad.LocalScale.magnitude / 30, 1f, out changed);  hasChanged = (changed) ? true : hasChanged;
            quadGravity.Point3 = ExtGravityOverrideEditor.DrawPoint(quadGravity.Point3, quad.P3, quad.LocalScale.magnitude / 30, 1f, out changed);  hasChanged = (changed) ? true : hasChanged;
            quadGravity.Point4 = ExtGravityOverrideEditor.DrawPoint(quadGravity.Point4, quad.P4, quad.LocalScale.magnitude / 30, 1f, out changed);  hasChanged = (changed) ? true : hasChanged;

            return (quadGravity);
        }

        public static bool DrawQuadFace(ExtQuad quad, bool face, bool allowBottom, float alpha, out bool hasChanged)
        {
            hasChanged = false;

            float scaleRect = quad.LocalScale.magnitude;
            float xScale = quad.LenghtX / 2;
            float yScale = quad.LenghtY / 2;


            Matrix4x4 scaleMatrix = Matrix4x4.TRS(quad.Position, quad.Rotation, new Vector3(xScale, 1, yScale));
            using (new Handles.DrawingScope(scaleMatrix))
            {
                Vector3 up = scaleMatrix.ExtractRotation() * Vector3.up;
                bool isCameraViewBehindFace = ExtVector3.DotProduct(ExtSceneView.GetSceneViewCameraTransform().forward, up) > 0 && !allowBottom;

                Handles.color = new Color(1, 0, 0, (isCameraViewBehindFace) ? alpha / 2 : alpha);

                float scale = 1f;
                Vector3[] verts = new Vector3[]
                {
                    new Vector3(- scale, 0,  - scale),
                    new Vector3(- scale, 0,  + scale),
                    new Vector3(+ scale, 0,  + scale),
                    new Vector3(+ scale, 0,  - scale)
                };

                if (!face)
                {
                    Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Color.clear);
                }

                Quaternion rotation = Quaternion.identity * Quaternion.LookRotation(Vector3.up);
                

                if (!Event.current.alt && Handles.Button(
                    Vector3.zero,
                    Quaternion.identity * Quaternion.LookRotation(Vector3.up),
                    1,
                    1,
                    Handles.RectangleHandleCap))
                {
                    if (isCameraViewBehindFace)
                    {
                        Debug.Log("not behind face");
                        return (false);
                    }
                    Debug.Log("face pressed");
                    face = !face;
                    hasChanged = true;
                    Event.current.Use();
                }
            }



            
            return (face);
        }

        public static GravityOverrideCylinder DrawCylinder(ExtCylinder cylinder, ExtCircle circle1, ExtCircle circle2, GravityOverrideCylinder cylinderGravity, float alpha, out bool hasChanged)
        {
            hasChanged = false;
            bool changed = hasChanged;

            cylinderGravity.Disc1 = ExtGravityOverrideEditor.DrawDisc(circle1, cylinderGravity.Disc1, false, alpha, out changed);
            hasChanged = (changed) ? true : hasChanged;
            cylinderGravity.Disc2 = ExtGravityOverrideEditor.DrawDisc(circle2, cylinderGravity.Disc2, false, alpha, out changed);
            hasChanged = (changed) ? true : hasChanged;
            cylinderGravity.Trunk = ExtGravityOverrideEditor.DrawLine(cylinderGravity.Trunk, cylinder.P1, cylinder.P2, size: cylinder.LocalScale.magnitude / 20, alpha: 0.8f, out changed);
            hasChanged = (changed) ? true : hasChanged;
            return (cylinderGravity);
        }

        /// <summary>
        /// draw a disc
        /// </summary>
        public static GravityOverrideDisc DrawDisc(ExtCircle circle, GravityOverrideDisc discGravity, bool allowBottom, float alpha, out bool hasChanged)
        {
            hasChanged = false;
            Quaternion rotation = ExtQuaternion.QuaternionFromVectorDirector(circle.Normal);
            bool topFace = discGravity.Face;
            bool topExtremity = discGravity.Borders;

            bool isCameraViewBehindFace = ExtVector3.DotProduct(ExtSceneView.GetSceneViewCameraTransform().forward, circle.Normal) > 0 && !allowBottom;

            Handles.color = new Color(1, 0, 0, alpha);
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
                Handles.color = new Color(1, 0, 0, (isCameraViewBehindFace) ? alpha / 2f : alpha);
                Handles.DrawSolidDisc(circle.Point, circle.Normal, circle.Radius / 10 * 8);
            }
            if (!topExtremity)
            {
                Handles.color = new Color(1, 0, 0, (isCameraViewBehindFace) ? alpha / 2f : alpha);
                ExtHandle.DrawCircleThickness(circle, 50, ExtHandle.DrawOutlineType.INSIDE);
            }
            Handles.color = Color.red;

            if (!Event.current.alt && Handles.Button(circle.Point,
                rotation,
                circle.Radius / 10 * 7,
                circle.Radius / 10 * 7, Handles.CircleHandleCap))
            {
                if (isCameraViewBehindFace)
                {
                    Debug.Log("not behind face");
                }
                else
                {
                    Debug.Log("Face pressed !");
                    discGravity.Face = !discGravity.Face;
                    hasChanged = true;
                    Event.current.Use();
                }
            }

            return (discGravity);
        }

        /// <summary>
        /// draw a line
        /// </summary>
        public static bool DrawLine(bool trunk, Vector3 p1, Vector3 p2, float size, float alpha, out bool hasChanged)
        {
            hasChanged = false;

            Vector3 direction = (p1 - p2);
            Vector3 middle = ExtVector3.GetMeanOfXPoints(p1, p2);
            Quaternion rotation = ExtQuaternion.QuaternionFromLine(p1, p2);

            Handles.color = new Color(1, 0, 0, alpha);
            float scaleCylinder = direction.magnitude;
            if (!trunk)
            {
                Matrix4x4 scaleMatrix = Matrix4x4.TRS(middle, rotation, new Vector3(size, size, scaleCylinder));
                using (new Handles.DrawingScope(scaleMatrix))
                {
                    Handles.CylinderHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
                }
            }
            
            if (!Event.current.alt && Handles.Button(
                middle,
                rotation,
                scaleCylinder,
                scaleCylinder,
                ExtGravityOverrideEditor.LineHandleCap))
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

       

        public static bool DrawPoint(bool point, Vector3 position, float size, float alpha, out bool hasChanged)
        {
            hasChanged = false;

            Handles.color = new Color(1, 0, 0, alpha);
            if (!point)
            {
                Handles.SphereHandleCap(0, position, Quaternion.identity, size, EventType.Repaint);
            }

            Handles.color = Color.clear;
            if (!Event.current.alt && Handles.Button(
                position,
                Quaternion.identity,
                size / 2,
                size,
                Handles.SphereHandleCap))
            {
                Debug.Log("point pressed");
                point = !point;
                hasChanged = true;
                Event.current.Use();
            }
            return (point);
        }


        public static void ApplyModificationToDisc(SerializedProperty disc1, GravityOverrideDisc datas)
        {
            disc1.GetPropertie(nameof(datas.Face)).boolValue = datas.Face;
            disc1.GetPropertie(nameof(datas.Borders)).boolValue = datas.Borders;
            disc1.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
        }

        public static void ApplyModificationToCylinder(SerializedProperty cylinder, GravityOverrideCylinder datas)
        {
            cylinder.GetPropertie(nameof(datas.Trunk)).boolValue = datas.Trunk;
            cylinder.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
            ApplyModificationToDisc(cylinder.GetPropertie(nameof(datas.Disc1)), datas.Disc1);
            ApplyModificationToDisc(cylinder.GetPropertie(nameof(datas.Disc2)), datas.Disc2);
        }

        public static void ApplyModificationToQuad(SerializedProperty quad, GravityOverrideQuad datas)
        {
            quad.GetPropertie(nameof(datas.Face1)).boolValue = datas.Face1;
            quad.GetPropertie(nameof(datas.Line1)).boolValue = datas.Line1;
            quad.GetPropertie(nameof(datas.Line2)).boolValue = datas.Line2;
            quad.GetPropertie(nameof(datas.Line3)).boolValue = datas.Line3;
            quad.GetPropertie(nameof(datas.Line4)).boolValue = datas.Line4;
            quad.GetPropertie(nameof(datas.Point1)).boolValue = datas.Point1;
            quad.GetPropertie(nameof(datas.Point2)).boolValue = datas.Point2;
            quad.GetPropertie(nameof(datas.Point3)).boolValue = datas.Point3;
            quad.GetPropertie(nameof(datas.Point4)).boolValue = datas.Point4;
            quad.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
        }

        public static void ApplyModificationToCube(SerializedProperty cube, GravityOverrideCube datas)
        {
            cube.GetPropertie(nameof(datas.Face1)).boolValue = datas.Face1;
            cube.GetPropertie(nameof(datas.Face2)).boolValue = datas.Face2;
            cube.GetPropertie(nameof(datas.Face3)).boolValue = datas.Face3;
            cube.GetPropertie(nameof(datas.Face4)).boolValue = datas.Face4;
            cube.GetPropertie(nameof(datas.Face5)).boolValue = datas.Face5;
            cube.GetPropertie(nameof(datas.Face6)).boolValue = datas.Face6;
            
            cube.GetPropertie(nameof(datas.Line1)).boolValue = datas.Line1;
            cube.GetPropertie(nameof(datas.Line2)).boolValue = datas.Line2;
            cube.GetPropertie(nameof(datas.Line3)).boolValue = datas.Line3;
            cube.GetPropertie(nameof(datas.Line4)).boolValue = datas.Line4;
            cube.GetPropertie(nameof(datas.Line5)).boolValue = datas.Line5;
            cube.GetPropertie(nameof(datas.Line6)).boolValue = datas.Line6;
            cube.GetPropertie(nameof(datas.Line7)).boolValue = datas.Line7;
            cube.GetPropertie(nameof(datas.Line8)).boolValue = datas.Line8;
            cube.GetPropertie(nameof(datas.Line9)).boolValue = datas.Line9;
            cube.GetPropertie(nameof(datas.Line10)).boolValue = datas.Line10;
            cube.GetPropertie(nameof(datas.Line11)).boolValue = datas.Line11;
            cube.GetPropertie(nameof(datas.Line12)).boolValue = datas.Line12;


            cube.GetPropertie(nameof(datas.Point1)).boolValue = datas.Point1;
            cube.GetPropertie(nameof(datas.Point2)).boolValue = datas.Point2;
            cube.GetPropertie(nameof(datas.Point3)).boolValue = datas.Point3;
            cube.GetPropertie(nameof(datas.Point4)).boolValue = datas.Point4;
            cube.GetPropertie(nameof(datas.Point5)).boolValue = datas.Point5;
            cube.GetPropertie(nameof(datas.Point6)).boolValue = datas.Point6;
            cube.GetPropertie(nameof(datas.Point7)).boolValue = datas.Point7;
            cube.GetPropertie(nameof(datas.Point8)).boolValue = datas.Point8;

            cube.GetPropertie("_canApplyGravityBordersAndFace1").boolValue = datas.CanApplyGravityBordersAndFace1;
            cube.GetPropertie("_canApplyGravityBordersAndFace2").boolValue = datas.CanApplyGravityBordersAndFace2;
            cube.GetPropertie("_canApplyGravityBordersAndFace3").boolValue = datas.CanApplyGravityBordersAndFace3;
            cube.GetPropertie("_canApplyGravityBordersAndFace4").boolValue = datas.CanApplyGravityBordersAndFace4;
            cube.GetPropertie("_canApplyGravityBordersAndFace5").boolValue = datas.CanApplyGravityBordersAndFace5;
            cube.GetPropertie("_canApplyGravityBordersAndFace6").boolValue = datas.CanApplyGravityBordersAndFace6;

            cube.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
        }

        //end of class
    }
    //end of nameSpace
}