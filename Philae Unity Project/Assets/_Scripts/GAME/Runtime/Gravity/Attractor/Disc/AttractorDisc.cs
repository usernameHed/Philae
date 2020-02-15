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
    public class AttractorDisc : Attractor
    {
        [SerializeField, OnValueChanged("ChangeDiscSettings", true)]
        protected ExtDisc _disc = default;


        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _disc = new ExtDisc(Position, Rotation, LocalScale, 0.5f);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 closestPoint = _disc.GetClosestPoint(graviton.Position);
            Vector3 position = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            canApplyGravity = !outOfRange;
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);

            return (position);
        }

        public void ChangeDiscSettings()
        {
            _disc.MoveSphape(Position, Rotation, LocalScale, _disc.Radius);
        }

        public override void Move()
        {
            _disc.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _disc.Draw(color);
            if (_minRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, Color.gray, _disc.RealRadius + _minRangeWithScale / 2);
            }
            if (_maxRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, color, _disc.RealRadius + _maxRangeWithScale / 2);
            }
        }
#endif
    }
}