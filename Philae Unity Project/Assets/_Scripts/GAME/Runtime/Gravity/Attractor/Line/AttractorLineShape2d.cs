using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor.line
{
    public class AttractorLineShape2d : Attractor
    {
        [SerializeField]
        public List<ExtLine> Lines;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
        }

        public override bool GetClosestPointIfWeCan(Graviton graviton, out Vector3 closestPoint)
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
            for (int i = Lines.Count - 1; i >= 0; i--)
            {
                Lines[i].Draw(color);
            }
        }
#endif
    }
}