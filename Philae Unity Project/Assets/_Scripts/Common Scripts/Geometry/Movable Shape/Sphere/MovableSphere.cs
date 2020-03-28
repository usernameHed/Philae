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
    public class MovableSphere : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtSphere _sphere = default;
        public ExtSphere Sphere { get { return (_sphere); } }
        public float Radius { get { return (_sphere.RealRadius); } }

        public override void InitOnCreation()
        {
            _sphere = new ExtSphere(Position, LocalScale, 0.5f);
        }

        public override void ChangeShapeStucture()
        {
            _sphere.MoveSphape(Position, LocalScale, _sphere.Radius);
        }

        public override void Move()
        {
            _sphere.MoveSphape(Position, LocalScale, _sphere.Radius);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _sphere.Draw(color);
        }
#endif
    }
}