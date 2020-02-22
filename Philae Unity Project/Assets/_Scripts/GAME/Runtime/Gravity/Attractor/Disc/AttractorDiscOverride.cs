using hedCommon.geometry.shape3d;
using philae.gravity.attractor.gravityOverride;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorDiscOverride : AttractorDisc
    {
        public GravityOverrideDisc GravityOverride;

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            bool canApplyGravity = _disc.GetClosestPointIfWeCan(graviton.Position, GravityOverride, out closestPoint);

            if (canApplyGravity)
            {
                closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
                canApplyGravity = !(canApplyGravity && outOfRange);
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }
    }
}