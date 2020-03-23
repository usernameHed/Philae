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
    public class MovableCapsule : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtCapsule _capsule = default;
        public ExtCapsule Capsule { get { return (_capsule); } }

        public override void InitOnCreation()
        {
            _capsule = new ExtCapsule(Position, Rotation, LocalScale, 0.5f, 4f);
        }

        public override void ChangeShapeStucture()
        {
            _capsule.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _capsule.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _capsule.Draw(color);
        }
#endif
    }
}