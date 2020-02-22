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
    public class AttractorSphere : Attractor
    {
        [SerializeField, ReadOnly]
        private ExtSphere _sphere = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = this.GetRightPosWithRange(graviton.Position, _sphere.Position, _sphere.Radius, _maxRangeWithScale, out bool outOfRange);
            bool canApplyGravity = !outOfRange;
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }

        /// <summary>
        /// called after init, or when transform has changed
        /// </summary>
        public override void Move()
        {
            _sphere.Position = Position;
            _sphere.Radius = _minRangeWithScale;
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            ExtDrawGuizmos.DebugWireSphere(Position, Color.gray, _sphere.Radius);
            ExtDrawGuizmos.DebugWireSphere(Position, color, _maxRangeWithScale);
        }
#endif
    }
}