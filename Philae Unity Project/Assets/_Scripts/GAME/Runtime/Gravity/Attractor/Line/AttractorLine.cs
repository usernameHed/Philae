using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor.line
{
    public class AttractorLine : Attractor
    {
        [SerializeField, OnValueChanged("ChangeLineSettings", true)]
        protected ExtLine3d _line = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _line = new ExtLine3d(Position, Rotation, LocalScale);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = _line.ClosestPointTo(graviton.Position);
            closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, SettingsLocal.MinRange, SettingsLocal.MaxRange, out bool outOfRange);
            
            bool canApplyGravity = true;
            if (outOfRange)
            {
                canApplyGravity = false;
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }

        public void ChangeLineSettings()
        {
            _line.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _line.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _line.Draw(color);
            if (SettingsLocal.MinRange > 0)
            {
                _line.DrawWithExtraSize(Color.gray, SettingsLocal.MinRange);
            }
            if (SettingsLocal.MaxRange > 0)
            {
                _line.DrawWithExtraSize(color, SettingsLocal.MaxRange);
            }
        }
#endif
    }
}