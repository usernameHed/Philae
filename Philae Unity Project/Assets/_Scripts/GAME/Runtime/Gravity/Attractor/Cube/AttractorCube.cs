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
    public class AttractorCube : Attractor
    {
        [SerializeField, ReadOnly]
        private ExtCube _cube = default;

        public override void InitOnCreation(AttractorListerLogic attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            Debug.Log("here init always ?");
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
            //ExtDrawGuizmos.DebugWireSphere(transform.position, Color.gray, _sphere.Radius);
            ExtDrawGuizmos.DebugWireSphere(transform.position, color, _maxRangeWithScale);
        }
#endif
    }
}