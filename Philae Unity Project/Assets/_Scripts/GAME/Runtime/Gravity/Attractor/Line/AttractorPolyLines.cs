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
        public ExtPolyLines _polyLines = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _polyLines = new ExtPolyLines(Position, Rotation, LocalScale);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            bool canApplyGravity = _polyLines.GetClosestPoint(graviton.Position, out closestPoint);
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
        }
#endif
    }
}