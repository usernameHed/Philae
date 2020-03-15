using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.movable
{
    public class MovableLine : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtLine3d _line3d = default;
        public ExtLine3d Line3d { get { return (_line3d); } }

        public override void InitOnCreation()
        {
            _line3d = new ExtLine3d(Position, Rotation, LocalScale);
        }

        public override void ChangeShapeStucture()
        {
            _line3d.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _line3d.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _line3d.Draw(color);
        }
#endif
    }
}