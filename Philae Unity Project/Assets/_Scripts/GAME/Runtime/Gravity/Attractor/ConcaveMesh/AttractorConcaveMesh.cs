using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorConcaveMesh : Attractor
    {
        public Mesh Mesh;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
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