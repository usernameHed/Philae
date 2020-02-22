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
    public class AttractorCapsuleOverride : AttractorCapsule
    {
        public GravityOverrideCapsule GravityOverride;

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 closestPoint = _capsule.GetClosestPointIfWeCan(graviton.Position, out canApplyGravity, GravityOverride);
            Vector3 position = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            if (outOfRange)
            {
                canApplyGravity = false;
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (position);
        }
    }
}