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
    public class MovableTriangle : MovableShape
    {

        /*
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtQuad _quad = default;
        public ExtQuad Quad { get { return (_quad); } }
        */

        public override void InitOnCreation()
        {
            //_quad = new ExtQuad(Position, Rotation, LocalScale);
        }

        public override void ChangeShapeStucture()
        {
            //_quad.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            //_quad.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            //_quad.Draw(color, true, true);
        }
#endif
    }
}