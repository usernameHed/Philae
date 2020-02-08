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

        public override void InitOnCreation(AttractorListerLogic attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 position = this.GetRightPosWithRange(graviton.Position, _sphere.Position, _sphere.Radius, _maxRangeWithScale, out bool outOfRange);
            canApplyGravity = !outOfRange;
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (position);
        }

        /// <summary>
        /// called after init, or when transform has changed
        /// </summary>
        public override void Move()
        {
            _sphere.Position = transform.position;
            _sphere.Radius = _minRangeWithScale;
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            ExtDrawGuizmos.DebugWireSphere(transform.position, Color.gray, _sphere.Radius);
            ExtDrawGuizmos.DebugWireSphere(transform.position, color, _maxRangeWithScale);
        }
#endif
    }
}