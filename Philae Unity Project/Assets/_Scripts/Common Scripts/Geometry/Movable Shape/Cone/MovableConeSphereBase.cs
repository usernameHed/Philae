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
    public class MovableConeSphereBase : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtConeSphereBase _cone = default;
        public ExtConeSphereBase Cone { get { return (_cone); } }

        public override void InitOnCreation()
        {
            _cone = new ExtConeSphereBase(Position, Rotation, LocalScale, 0.5f, 4f);
        }

        public override void ChangeShapeStucture()
        {
            _cone.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _cone.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _cone.Draw(color);
        }
#endif
    }
}