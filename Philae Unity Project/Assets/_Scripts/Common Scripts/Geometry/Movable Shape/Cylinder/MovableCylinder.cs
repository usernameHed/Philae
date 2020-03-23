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
    public class MovableCylinder : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtCylinder _cylinder = default;
        public ExtCylinder Cylinder { get { return (_cylinder); } }

        public override void InitOnCreation()
        {
            _cylinder = new ExtCylinder(Position, Rotation, LocalScale, 0.5f, 4f);
        }

        public override void ChangeShapeStucture()
        {
            _cylinder.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _cylinder.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _cylinder.Draw(color);
        }
#endif
    }
}