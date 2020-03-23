﻿using hedCommon.extension.runtime;
using hedCommon.geometry.movable;
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
    public class AttractorCylinderGravityOverride : AttractorCylinder
    {
        public GravityOverrideCylinder GravityOverride;

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            bool canApplyGravity = _movableCylinder.Cylinder.GetClosestPointIfWeCan(graviton.Position, GravityOverride, out closestPoint);

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