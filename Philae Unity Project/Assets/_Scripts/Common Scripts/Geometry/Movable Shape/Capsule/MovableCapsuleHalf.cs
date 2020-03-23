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
    public class MovableCapsuleHalf : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtHalfCapsule _capsuleHalf = default;
        public ExtHalfCapsule CapsuleHalf { get { return (_capsuleHalf); } }

        public override void InitOnCreation()
        {
            _capsuleHalf = new ExtHalfCapsule(Position, Rotation, LocalScale, 0.5f, 4f);
        }

        public override void ChangeShapeStucture()
        {
            _capsuleHalf.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _capsuleHalf.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _capsuleHalf.Draw(color);
        }
#endif
    }
}