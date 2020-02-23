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
    public class AttractorConeSphereBaseOverride : AttractorConeSphereBase
    {
        public GravityOverrideConeSphereBase GravityOverride;

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = this.GetRightPosWithRange(graviton.Position, Position, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            bool canApplyGravity = true;
            if (outOfRange)
            {
                canApplyGravity = false;
            }
            else
            {
                canApplyGravity = _cone.GetClosestPointIfWeCan(graviton.Position, GravityOverride, out closestPoint);
            }

            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }
    }
}