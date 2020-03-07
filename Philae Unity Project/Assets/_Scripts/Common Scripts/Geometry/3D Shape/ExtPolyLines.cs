﻿using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.gravityOverride;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
    [Serializable]
    public struct ExtPolyLines
    {
        [SerializeField]
        private Vector3 _position;
        public Vector3 Position { get { return (_position); } }
        [SerializeField]
        private Quaternion _rotation;
        public Quaternion Rotation { get { return (_rotation); } }
        [SerializeField]
        private Vector3 _localScale;
        public Vector3 LocalScale { get { return (_localScale); } }

        [SerializeField]
        private ExtLine[] _listLinesLocal;

        [SerializeField]
        private ExtLine[] _listLines;
        [SerializeField]
        private Matrix4x4 _polyLinesMatrix;

#if UNITY_EDITOR
        [SerializeField]
        private List<PointsInfo> _pointsInfos;
#endif

        public ExtPolyLines(Vector3 position,
            Quaternion rotation,
            Vector3 localScale) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;

            _listLinesLocal = new ExtLine[3];
            _listLines = new ExtLine[3];
            _listLinesLocal[0] = new ExtLine(new Vector3(0, 0, 0), new Vector3(-0.3f, 0, -0.2f));
            _listLinesLocal[1] = new ExtLine(new Vector3(0, 0, 0), new Vector3(0.3f, 0, -0.2f));
            _listLinesLocal[2] = new ExtLine(new Vector3(0, 0, 0), new Vector3(0, 0, 0.3f));

            UpdateMatrix();
        }

        public ExtLine LineAt(int index)
        {
            return (_listLines[index]);
        }

        private void UpdateMatrix()
        {
            _polyLinesMatrix = Matrix4x4.TRS(_position, _rotation, _localScale);

            UpdateGlobalLineFromLocalOnes();
        }

        private void UpdateGlobalLineFromLocalOnes()
        {
            for (int i = 0; i < _listLinesLocal.Length; i++)
            {
                _listLines[i].MoveShape(
                    _polyLinesMatrix.MultiplyPoint3x4(_listLinesLocal[i].P1),
                    _polyLinesMatrix.MultiplyPoint3x4(_listLinesLocal[i].P2));
            }
        }

        public void AddLineLocal(Vector3 p1, Vector3 p2)
        {
            ExtLine line = new ExtLine(p1, p2);
            _listLinesLocal = ExtArray.Add(_listLinesLocal, line);
            UpdateGlobalLineFromLocalOnes();
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            UpdateMatrix();
        }

        /// <summary>
        /// Return the closest point from all lines
        ///   
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            return (ExtLine.GetClosestPointFromLines(k, _listLines));
        }

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            for (int i = 0; i < _listLines.Length; i++)
            {
                _listLines[i].Draw(color);
            }
        }

        public void DrawWithExtraSize(Color color, float offset)
        {
            for (int i = 0; i < _listLines.Length; i++)
            {
                ExtDrawGuizmos.DebugCapsuleFromInsidePoint(_listLines[i].P1, _listLines[i].P2, color, offset, 0, true, false);
            }
        }
#endif
        //end class
    }
    //end nameSpace
}