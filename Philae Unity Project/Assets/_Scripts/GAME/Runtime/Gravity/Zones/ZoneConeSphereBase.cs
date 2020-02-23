using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.zones
{
    [Serializable]
    public class ZoneConeSphereBase : Zone
    {
        public ExtConeSphereBase ConeSphereBase;

        public override void Init(GravityAttractorZone zone)
        {
            base.Init(zone);
            ConeSphereBase = new ExtConeSphereBase(ZonePhysic.GetScalerZoneReference.position,
                ZonePhysic.GetScalerZoneReference.rotation,
                ZonePhysic.GetScalerZoneReference.localScale,
                0.5f,
                2f);
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            ConeSphereBase.Draw(base.GetColor());
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 localScale)
        {
            ConeSphereBase.MoveSphape(newPosition, rotation, localScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (ConeSphereBase.IsInsideShape(position));
        }
    }
}