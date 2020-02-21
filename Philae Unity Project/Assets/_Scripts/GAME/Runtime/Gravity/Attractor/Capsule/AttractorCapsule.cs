using hedCommon.extension.runtime;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorCapsule : AttractorCylinder
    {
        public bool ClosedUp = false;
        public bool ClosedDown = false;

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            canApplyGravity = false;
            return (Vector3.zero);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            //base.DrawRange(color);
            ExtDrawGuizmos.DebugCapsuleFromInsidePoint(_cylinder.P1, _cylinder.P2, color, _cylinder.RealRadius, 0, true, true, !ClosedUp, !ClosedDown);
        }
#endif
    }
}