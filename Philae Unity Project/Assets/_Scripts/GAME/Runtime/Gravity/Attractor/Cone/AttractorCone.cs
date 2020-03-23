using hedCommon.extension.runtime;
using hedCommon.geometry.movable;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorCone : Attractor
    {
        [SerializeField]
        protected MovableConeSphereBase _movableCone;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = _movableCone.Cone.GetClosestPoint(graviton.Position);
            closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);

            bool canApplyGravity = true;
            if (outOfRange)
            {
                canApplyGravity = false;
            }

            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _movableCone.Cone.Draw(color);
            if (_minRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, Color.gray, _minRangeWithScale);
            }
            if (_maxRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, Color.gray, _maxRangeWithScale);
            }
        }
#endif
    }
}