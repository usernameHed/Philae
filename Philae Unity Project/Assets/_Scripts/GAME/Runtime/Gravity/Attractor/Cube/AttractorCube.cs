using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorCube : Attractor
    {
        [SerializeField, ReadOnly]
        private ExtCube _cube = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            Debug.Log("here init always ?");
            _cube = new ExtCube(Position, Rotation, LocalScale);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 closestPoint = _cube.GetClosestPoint(graviton.Position);
            canApplyGravity = true;
            return (closestPoint);
        }

        public override void Move()
        {
            _cube.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _cube.Draw(color);
            //ExtDrawGuizmos.DebugWireSphere(transform.position, Color.gray, _sphere.Radius);
            ExtDrawGuizmos.DebugWireSphere(Position, color, _maxRangeWithScale);
        }
#endif
    }
}