using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    public class MovableSpline : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtSpline _spline = default;
        public ExtSpline Spline { get { return (_spline); } }

        public override void InitOnCreation()
        {
            _spline = new ExtSpline(Position, Rotation, LocalScale);
        }

        public override void ChangeShapeStucture()
        {
            _spline.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _spline.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _spline.Draw(color);
        }
#endif
    }
}