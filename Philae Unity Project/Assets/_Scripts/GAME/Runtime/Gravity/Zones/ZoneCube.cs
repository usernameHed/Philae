using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.zones
{
    [Serializable]
    public class ZoneCube : Zone
    {
        public ExtCube Cube;

        public override void Init(GravityAttractorZone zone)
        {
            base.Init(zone);
            Cube = new ExtCube(ZonePhysic.GetScalerZoneReference.position, ZonePhysic.GetScalerZoneReference.rotation, ZonePhysic.GetScalerZoneReference.localScale);
        }

        public override void Draw()
        {
            Cube.Draw(base.GetColor(), false, false);
        }

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 localScale)
        {
            Cube.MoveSphape(newPosition, rotation, localScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (Cube.IsInsideShape(position));
        }
    }
}