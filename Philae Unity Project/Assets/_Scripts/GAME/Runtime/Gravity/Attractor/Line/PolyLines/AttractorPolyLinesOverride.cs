using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.gravityOverride;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor.line
{
    public class AttractorPolyLinesOverride : AttractorPolyLines
    {
        public GravityOverrideLineTopDown[] GravityOverride;

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            bool canApplyGravity = _polyLines.GetClosestPointIfWeCan(graviton.Position, out closestPoint, GravityOverride);
            
            if (canApplyGravity)
            {
                closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, SettingsLocal.MinRange, SettingsLocal.MaxRange, out bool outOfRange);
                if (outOfRange)
                {
                    canApplyGravity = false;
                }
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }

    }
}