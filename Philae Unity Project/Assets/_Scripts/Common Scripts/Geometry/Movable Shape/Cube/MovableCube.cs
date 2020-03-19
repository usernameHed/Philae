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
    public class MovableCube : MovableShape
    {
        [SerializeField, OnValueChanged("ChangeShapeStucture", true)]
        protected ExtCube _cube = default;
        public ExtCube Cube { get { return (_cube); } }

        public override void InitOnCreation()
        {
            _cube = new ExtCube(Position, Rotation, LocalScale);
        }

        public override void ChangeShapeStucture()
        {
            _cube.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _cube.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        public override void Draw(Color color)
        {
            _cube.Draw(color, true, true);
        }
#endif
    }
}