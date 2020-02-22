using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.gravityOverride;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorCapsuleHalfOverride : AttractorCapsuleHalf
    {
        public GravityOverrideLineTopDown GravityOverride;

        public override bool GetClosestPoint(Graviton graviton, out Vector3 closestPoint)
        {
            bool canApplyGravity = _capsuleHalf.GetClosestPointIfWeCan(graviton.Position, GravityOverride, out closestPoint);
            closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            if (outOfRange)
            {
                canApplyGravity = false;
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }
    }
}