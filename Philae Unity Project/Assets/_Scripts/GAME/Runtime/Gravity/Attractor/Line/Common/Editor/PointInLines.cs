using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class PointInLines
    {
        public int IndexLine;           //the index of line, usefull if we have multiple PointInLines
        public int IndexPoint;          //0 or 1: we need to know wich point we are, 0 or 1 on the line
        public PointsInfo PointInfo;

        public SerializedProperty P1PropertieGlobal;
        public SerializedProperty P2PropertieGlobal;

        public SerializedProperty P1PropertieLocal;
        public SerializedProperty P2PropertieLocal;

        public PointInLines(int indexLine,
            int indexPoint,
            SerializedProperty p1Local,
            SerializedProperty p2Local,
            SerializedProperty p1Global,
            SerializedProperty p2Global)
        {
            IndexLine = indexLine;
            IndexPoint = indexPoint;
            PointInfo = new PointsInfo();

            P1PropertieLocal = p1Local;
            P2PropertieLocal = p2Local;

            P1PropertieGlobal = p1Global;
            P2PropertieGlobal = p2Global;
        }

        public Vector3 GetGlobalPointPosition()
        {
            return (IsPoint1()) ? P1PropertieGlobal.vector3Value : P2PropertieGlobal.vector3Value;
        }

        public Vector3 GetLocalPosition()
        {
            return (IsPoint1()) ? P1PropertieLocal.vector3Value : P2PropertieLocal.vector3Value;
        }

        public Vector3 GetOtherLocalPoint()
        {
            return (IsPoint1()) ? P2PropertieLocal.vector3Value : P1PropertieLocal.vector3Value;
        }

        public Vector3 GetMiddleLine()
        {
            Vector3 middleLine = ExtVector3.GetMeanOfXPoints(P1PropertieGlobal.vector3Value, P2PropertieGlobal.vector3Value);
            return (middleLine);
        }

        public void SetGlobalPointPosition(Vector3 pX)
        {
            if (IsPoint1())
            {
                P1PropertieGlobal.vector3Value = pX;
            }
            else
            {
                P2PropertieGlobal.vector3Value = pX;
            }
        }

        public void UpdateLocalPositionFromGlobalPosition(Matrix4x4 inverse)
        {
            P1PropertieLocal.vector3Value = inverse.MultiplyPoint3x4(P1PropertieGlobal.vector3Value);
            P2PropertieLocal.vector3Value = inverse.MultiplyPoint3x4(P2PropertieGlobal.vector3Value);
        }

        public bool IsSelected()
        {
            return (PointInfo.IsSelected);
        }

        public void SetSelected(bool selected)
        {
            PointInfo.IsSelected = selected;
        }

        public bool IsPoint1()
        {
            return (IndexPoint == 0);
        }

        public bool IsPoint2()
        {
            return (IndexPoint == 1);
        }
    }

    public class PointInSplines
    {
        public int IndexPoint;          //0 or 1: we need to know wich point we are, 0 or 1 on the line
        public PointsInfo PointInfo;

        public SerializedProperty PointPropertieGlobal;
        public SerializedProperty PointPropertieLocal;

        public PointInSplines(int indexPoint,
            SerializedProperty p1Local,
            SerializedProperty p1Global)
        {
            IndexPoint = indexPoint;
            PointInfo = new PointsInfo();
            PointPropertieLocal = p1Local;
            PointPropertieGlobal = p1Global;
        }

        public Vector3 GetGlobalPointPosition()
        {
            return (PointPropertieGlobal.vector3Value);
        }

        public Vector3 GetLocalPosition()
        {
            return (PointPropertieLocal.vector3Value);
        }

        public void SetGlobalPointPosition(Vector3 pX)
        {
            PointPropertieGlobal.vector3Value = pX;
        }

        public void UpdateLocalPositionFromGlobalPosition(Matrix4x4 inverse)
        {
            PointPropertieLocal.vector3Value = inverse.MultiplyPoint3x4(PointPropertieGlobal.vector3Value);
        }

        public bool IsSelected()
        {
            return (PointInfo.IsSelected);
        }

        public void SetSelected(bool selected)
        {
            PointInfo.IsSelected = selected;
        }
    }
}