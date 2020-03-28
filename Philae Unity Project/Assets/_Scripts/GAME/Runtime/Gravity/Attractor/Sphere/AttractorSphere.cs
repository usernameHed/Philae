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
    public class AttractorSphere : Attractor
    {
        [SerializeField]
        protected MovableSphere _movableSphere;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = this.GetRightPosWithRange(graviton.Position, _movableSphere.Position, _movableSphere.Radius + _minRangeWithScale, _movableSphere.Radius + _maxRangeWithScale, out bool outOfRange);
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
            _movableSphere.Sphere.Draw(color);
            if (_minRangeWithScale > 0)
            {
                _movableSphere.Sphere.DrawWithExtraRadius(Color.gray, _minRangeWithScale);
            }
            if (_maxRangeWithScale > 0)
            {
                _movableSphere.Sphere.DrawWithExtraRadius(Color.gray, _maxRangeWithScale);
            }
        }
#endif
    }
}