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
        }
        public Shape ShapeZone = Shape.SPHERE;
        [OnValueChanged("IsActiveZoneChanged")]
        public bool IsActiveZone = true;

        public UnityAction IsActiveZoneChange;

        private void IsActiveZoneChanged()
        {
            IsActiveZoneChange.Invoke();
        }
    }
}