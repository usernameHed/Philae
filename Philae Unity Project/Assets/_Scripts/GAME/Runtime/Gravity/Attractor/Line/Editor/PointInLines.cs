using hedCommon.extension.editor;
using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using UnityEditor;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class PointInLines
    {
        public int IndexLine;
        public int IndexPoint;//0 or 1
        public PointsInfo PointInfo;
        public SerializedProperty ExtLineFromGlobal;
        public SerializedProperty ExtLineFromLocal;

        public SerializedProperty P1PropertieGlobal;
        public SerializedProperty P2PropertieGlobal;

        public SerializedProperty P1PropertieLocal;
        public SerializedProperty P2PropertieLocal;

        public PointInLines(int indexLine,
            int indexPoint,
            PointsInfo pointInfo,
            SerializedProperty extLineFromGlobal,
            SerializedProperty extLineFromLocal)
        {
            IndexLine = indexLine;
            IndexPoint = indexPoint;
            PointInfo = pointInfo;

            ExtLineFromGlobal = extLineFromGlobal;
            ExtLineFromLocal = extLineFromLocal;

            P1PropertieGlobal = extLineFromGlobal.GetPropertie("_p1");
            P2PropertieGlobal = extLineFromGlobal.GetPropertie("_p2");

            P1PropertieLocal = extLineFromLocal.GetPropertie("_p1");
            P2PropertieLocal = extLineFromLocal.GetPropertie("_p2");
        }

        public Vector3 GetGlobalPointPosition()
        {
            return (IsPoint1()) ? P1PropertieGlobal.vector3Value : P2PropertieGlobal.vector3Value;
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
}