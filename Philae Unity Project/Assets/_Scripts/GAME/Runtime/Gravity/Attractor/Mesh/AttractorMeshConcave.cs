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
    public class AttractorMeshConcave : Attractor
    {
        [SerializeField]
        protected MovableMeshConcave _movableMeshConcave;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
        {
            /*
            bool canApplyGravity = _movableMeshConcave.Disc.GetClosestPoint(graviton.Position, out closestPoint);
            if (canApplyGravity)
            {
                closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
                if (outOfRange)
                {
                    canApplyGravity = false;
                }
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
            */
            closestPoint = Vector3.zero;
            return (false);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            /*
            _movableMeshConcave.Disc.Draw(color);
            if (_minRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, Color.gray, _minRangeWithScale);
            }
            if (_maxRangeWithScale > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Position, Color.gray, _maxRangeWithScale);
            }
            */
        }
#endif
    }
}