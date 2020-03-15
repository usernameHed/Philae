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
    public class AttractorPolyLines : Attractor
    {
        [SerializeField, OnValueChanged("ChangePolyLinesSettings", true)]
        protected ExtPolyLines _polyLines = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _polyLines = new ExtPolyLines(Position, Rotation, LocalScale);
        }

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

        public void ChangePolyLinesSettings()
        {
            _polyLines.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _polyLines.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _polyLines.Draw(color);
            if (SettingsLocal.MinRange > 0)
            {
                _polyLines.DrawWithExtraSize(Color.gray, SettingsLocal.MinRange);
            }
            if (SettingsLocal.MaxRange > 0)
            {
                _polyLines.DrawWithExtraSize(color, SettingsLocal.MaxRange);
            }
        }
#endif
    }
}