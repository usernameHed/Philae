using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorQuad : Attractor
    {
        [SerializeField, OnValueChanged("ChangeQuadSettings", true)]
        protected ExtQuad _quad = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _quad = new ExtQuad(Position, Rotation, LocalScale);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 closestPoint = _quad.GetClosestPoint(graviton.Position, out canApplyGravity);
            Vector3 position = closestPoint;
            if (canApplyGravity)
            {
                position = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);

                canApplyGravity = !(canApplyGravity && outOfRange);
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (position);
        }

        public void ChangeQuadSettings()
        {
            _quad.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _quad.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _quad.Draw(color, true, true);
            if (_minRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, Color.gray, LocalScale.magnitude + _minRangeWithScale / 2);
            }
            if (_maxRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, color, LocalScale.magnitude + _maxRangeWithScale / 2);
            }
        }
#endif
    }
}