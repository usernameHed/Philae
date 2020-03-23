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
    public class MovableDisc : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtDisc _disc = default;
        public ExtDisc Disc { get { return (_disc); } }

        public override void InitOnCreation()
        {
            _disc = new ExtDisc(Position, Rotation, LocalScale, 0.5f);
        }

        public override void ChangeShapeStucture()
        {
            _disc.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _disc.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _disc.Draw(color);
        }
#endif
    }
}