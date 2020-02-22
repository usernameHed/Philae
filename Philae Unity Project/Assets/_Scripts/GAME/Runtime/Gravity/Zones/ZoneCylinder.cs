﻿using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.zones
{
    [Serializable]
    public class ZoneCylinder : Zone
    {
        public ExtCylinder Cylindre;

        public override void Init(GravityAttractorZone zone)
        {
            base.Init(zone);
            Cylindre = new ExtCylinder(ZonePhysic.GetScalerZoneReference.position,
                ZonePhysic.GetScalerZoneReference.rotation,
                ZonePhysic.GetScalerZoneReference.localScale,
                1f,
                2f);
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            Cylindre.Draw(base.GetColor());
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 localScale)
        {
            Cylindre.MoveSphape(newPosition, rotation, localScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (Cylindre.IsInsideShape(position));
        }
    }
}