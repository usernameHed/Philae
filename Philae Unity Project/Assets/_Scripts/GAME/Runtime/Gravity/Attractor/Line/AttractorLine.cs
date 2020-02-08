using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor.line
{
    public class AttractorLine : Attractor
    {
        public ExtLine Line;

        public override void InitOnCreation(AttractorListerLogic attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            canApplyGravity = false;
            return (Vector3.zero);
        }

        public override void Move()
        {

        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {

        }
#endif
    }
}