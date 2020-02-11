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
    public class AttractorCylinderOverride : AttractorCylinder
    {
        public GravityOverrideCylinder GravityOverride;

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 closestPoint = _cylinder.GetClosestPoint(graviton.Position);
            Vector3 position = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            canApplyGravity = !outOfRange;
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);

            return (position);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _cylinder.Draw(color);
            _cylinder.DrawWithExtraSize(Color.gray, new Vector3(_minRangeWithScale, _minRangeWithScale / 2, _minRangeWithScale));
            _cylinder.DrawWithExtraSize(color, new Vector3(_maxRangeWithScale, _maxRangeWithScale / 2, _maxRangeWithScale));
        }
#endif
    }
}