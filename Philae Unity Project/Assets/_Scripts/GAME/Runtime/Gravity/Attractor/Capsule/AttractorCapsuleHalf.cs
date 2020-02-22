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
    public class AttractorCapsuleHalf : Attractor
    {
        [SerializeField, OnValueChanged("ChangeCapsuleSettings", true)]
        protected ExtHalfCapsule _capsuleHalf = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _capsuleHalf = new ExtHalfCapsule(Position, Rotation, LocalScale, 0.5f, 4f);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 closestPoint = _capsuleHalf.GetClosestPoint(graviton.Position);
            Vector3 position = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            canApplyGravity = !outOfRange;
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (position);
        }

        public void ChangeCapsuleSettings()
        {
            _capsuleHalf.MoveSphape(Position, Rotation, LocalScale, _capsuleHalf.Radius, _capsuleHalf.Lenght);
        }

        public override void Move()
        {
            _capsuleHalf.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            ExtDrawGuizmos.DebugHalfCapsuleFromInsidePoint(_capsuleHalf.P1, _capsuleHalf.P2, color, _capsuleHalf.RealRadius);
        }
#endif
    }
}