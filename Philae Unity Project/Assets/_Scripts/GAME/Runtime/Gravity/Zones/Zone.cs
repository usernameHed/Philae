using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.zones
{
    [Serializable]
    public abstract class Zone
    {
        public GravityAttractorZone ZonePhysic;

        public virtual void Init(GravityAttractorZone zone)
        {
            ZonePhysic = zone;
        }

        public abstract void Draw();
        public abstract bool IsInsideShape(Vector3 position);

        public virtual void Move(Vector3 newPosition, Quaternion rotation, Vector3 localScale) { }

        protected Color GetColor()
        {
            if (!ZonePhysic.SettingsLocal.IsActiveZone)
            {
                return (EditorOptions.ColorZonesInactive);
            }
            if (!ZonePhysic.IsZoneEmpty())
            {
                return (EditorOptions.ColorWhenSomethingInside);
            }
            return (EditorOptions.ColorWhenNothinngInside);
        }
    }

}

