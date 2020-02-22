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
    public class AttractorCapsule : Attractor
    {
        [SerializeField, OnValueChanged("ChangeCapsuleSettings", true)]
        private ExtCapsule _capsule = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _capsule = new ExtCapsule(Position, Rotation, LocalScale, 0.5f, 4f, true, true);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 closestPoint = _capsule.GetClosestPoint(graviton.Position);
            Vector3 position = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            canApplyGravity = !outOfRange;
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (position);
        }

        public void ChangeCapsuleSettings()
        {
            _capsule.MoveSphape(Position, Rotation, LocalScale, _capsule.Radius, _capsule.Lenght);
        }

        public override void Move()
        {
            _capsule.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            ExtDrawGuizmos.DebugCapsuleFromInsidePoint(_capsule.P1, _capsule.P2, color, _capsule.RealRadius, 0, true, true, _capsule.IsSphereTop, _capsule.IsSphereBottom);
        }
#endif
    }
}