using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorDonut : Attractor
    {
        [SerializeField, OnValueChanged("ChangeDonutSettings", true)]
        protected ExtDonut _donut = default;


        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _donut = new ExtDonut(Position, Rotation, LocalScale, 0.5f, 0.1f);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = _donut.GetClosestPoint(graviton.Position);
            closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);

            bool canApplyGravity = !outOfRange;
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }

        public void ChangeDonutSettings()
        {
            _donut.MoveSphape(Position, Rotation, LocalScale, _donut.Radius, _donut.ThickNess);
        }

        public override void Move()
        {
            _donut.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _donut.Draw(color);
            if (_minRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, Color.gray, _donut.RealRadius + _minRangeWithScale / 2);
            }
            if (_maxRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, color, _donut.RealRadius + _maxRangeWithScale / 2);
            }
        }
#endif
    }
}