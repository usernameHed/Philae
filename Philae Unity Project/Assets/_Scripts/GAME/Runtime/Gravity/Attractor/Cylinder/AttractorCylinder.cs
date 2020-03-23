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
    public class AttractorCylinder : Attractor
    {
        [SerializeField]
        protected MovableCylinder _movableCylinder;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = _movableCylinder.Cylinder.GetClosestPoint(graviton.Position);
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
            _movableCylinder.Cylinder.Draw(color);
            if (_minRangeWithScale > 0)
            {
                _movableCylinder.Cylinder.DrawWithExtraSize(Color.gray, new Vector3(_minRangeWithScale, _minRangeWithScale, _minRangeWithScale));
            }
            if (_maxRangeWithScale > 0)
            {
                _movableCylinder.Cylinder.DrawWithExtraSize(Color.gray, new Vector3(_maxRangeWithScale, _maxRangeWithScale, _maxRangeWithScale));
            }
        }
#endif
    }
}