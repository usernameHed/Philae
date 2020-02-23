using hedCommon.extension.runtime;
using hedCommon.scriptableObject;
using philae.sound;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static philae.gravity.physicsBody.PhysicBody;

namespace philae.data.gravity
{
    [CreateAssetMenu(fileName = "ZoneSettings", menuName = "Philae/Gravity/ZoneSettings")]
    public class ZoneSettings : ScriptableObject
    {
        public enum JonctionInteraction
        {
            ADDITIVE = 0,
            SUBSTRACTIVE = 1,
        }

        public JonctionInteraction Jonction = JonctionInteraction.ADDITIVE;

        public ConstrainPosition ConstrainPosition;
    }

    [Serializable]
    public class ZoneSettingsLocal
    {
        public enum Shape
        {
            SPHERE = 10,
            CUBE = 20,
            CYLINDER = 30,
            CAPSULE = 40,
            CAPSULE_HALF = 41,
            CONE_SPHERE_BASE = 50,
        }
        [OnValueChanged("IsSphapeZoneChanged")]
        public Shape ShapeZone = Shape.SPHERE;
        [OnValueChanged("IsActiveZoneChanged")]
        public bool IsActiveZone = true;

        public UnityAction IsSphapeZoneChange;
        public UnityAction IsActiveZoneChange;

        /// <summary>
        /// called by Odin Inspector
        /// </summary>
        private void IsSphapeZoneChanged()
        {
            if (IsSphapeZoneChange != null)
            {
                IsSphapeZoneChange.Invoke();
            }
        }

        /// <summary>
        /// called by OdinInspector
        /// </summary>
        private void IsActiveZoneChanged()
        {
            if (IsActiveZoneChange != null)
            {
                IsActiveZoneChange.Invoke();
            }
        }
    }
}