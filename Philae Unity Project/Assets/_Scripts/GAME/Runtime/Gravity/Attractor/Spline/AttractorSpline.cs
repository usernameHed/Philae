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
    public class AttractorSpline : Attractor
    {
        [SerializeField, OnValueChanged("ChangeSplineSettings", true)]
        protected ExtSpline _spline = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _spline = new ExtSpline(Position, Rotation, LocalScale);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = _spline.GetClosestPoint(graviton.Position);
            closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, SettingsLocal.MinRange, SettingsLocal.MaxRange    , out bool outOfRange);
            
            bool canApplyGravity = true;
            if (outOfRange)
            {
                canApplyGravity = false;
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }

        public void ChangeSplineSettings()
        {
            _spline.MoveSphape(Position, Rotation, LocalScale);
        }

        public override void Move()
        {
            _spline.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _spline.Draw(color);
            if (SettingsLocal.MinRange > 0)
            {
                _spline.DrawWithExtraSize(Color.gray, SettingsLocal.MinRange);
            }
            if (SettingsLocal.MaxRange > 0)
            {
                _spline.DrawWithExtraSize(color, SettingsLocal.MaxRange);
            }
        }
#endif
    }
}