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
    public class MovableDonut : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtDonut _donut = default;
        public ExtDonut Donut { get { return (_donut); } }

        public override void InitOnCreation()
        {
            _donut = new ExtDonut(Position, Rotation, LocalScale, 0.5f, 0.1f);
        }

        public override void ChangeShapeStucture()
        {
            _donut.MoveSphape(Position, Rotation, LocalScale, _donut.Radius, _donut.ThickNess);
        }

        public override void Move()
        {
            _donut.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Draw(Color color)
        {
            _donut.Draw(color);
        }
    }
}