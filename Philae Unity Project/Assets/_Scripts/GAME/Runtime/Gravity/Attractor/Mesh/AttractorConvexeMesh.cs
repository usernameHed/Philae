using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorConvexeMesh : Attractor
    {
        public Mesh Mesh;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
        }

        public override bool GetClosestPoint(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = Vector3.zero;
            return (false);
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