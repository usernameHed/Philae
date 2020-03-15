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
    public class MovablePolyLines : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtPolyLines _polyLines = default;
        public ExtPolyLines PolyLines { get { return (_polyLines); } }

        public override void InitOnCreation()
        {
            _polyLines = new ExtPolyLines(Position, Rotation, LocalScale);
        }

        public override void ChangeShapeStucture()
        {
            _polyLines.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _polyLines.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _polyLines.Draw(color);
        }
#endif
    }
}