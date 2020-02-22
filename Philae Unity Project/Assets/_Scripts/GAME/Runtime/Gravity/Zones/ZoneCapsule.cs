using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.zones
{
    [Serializable]
    public class ZoneCapsule : Zone
    {
        public ExtCapsule Capsule;

        public override void Init(GravityAttractorZone zone)
        {
            base.Init(zone);
            Capsule = new ExtCapsule(ZonePhysic.GetScalerZoneReference.position,
                ZonePhysic.GetScalerZoneReference.rotation,
                ZonePhysic.GetScalerZoneReference.localScale,
                1f,
                2f, true, true);
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            Capsule.Draw(base.GetColor());
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 localScale)
        {
            Capsule.MoveSphape(newPosition, rotation, localScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (Capsule.IsInsideShape(position));
        }
    }
}