using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.zones
{
    [Serializable]
    public class ZoneSphere : Zone
    {
        public ExtSphere Sphere;

        public override void Init(GravityAttractorZone zone)
        {
            base.Init(zone);
            Sphere = new ExtSphere(ZonePhysic.GetScalerZoneReference.position, 1f);
        }

        public override void Draw()
        {
            Sphere.Draw(base.GetColor());
        }

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 localScale)
        {
            Sphere.MoveSphape(newPosition, ExtVector3.Maximum(localScale) / 2f);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (Sphere.IsInsideShape(position));
        }
    }
}