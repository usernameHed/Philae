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
            closestPoint = _polyLines.GetClosestPoint(graviton.Position);
            closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, SettingsLocal.MinRange, SettingsLocal.MaxRange    , out bool outOfRange);
            
            bool canApplyGravity = true;
            if (outOfRange)
            {
                canApplyGravity = false;
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }

    }
}