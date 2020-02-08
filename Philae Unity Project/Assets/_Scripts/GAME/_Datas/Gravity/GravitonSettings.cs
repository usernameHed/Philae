using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using hedCommon.scriptableObject;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace philae.data.gravity
{
    [CreateAssetMenu(fileName = "GravitonSettings", menuName = "Philae/Gravity/GravitonSettings")]
    public class GravitonSettings : ScriptableObject
    {
        [Tooltip("show the force applyed on graviton")]
        public bool ShowForce = true;
        [Range(1, 100), Tooltip("If 2 or more attractors attract the graviton," +
            "this ratio determine how much the attractors farest from the player will attract." +
            "1 = ")]
        public float RatioEaseOutAttractors = 1f;
    }

    [Serializable]
    public class GravitonSettingsLocal
    {
        [OnValueChanged("OnMassChanged")]
        public float Mass = 1f;
        public UnityAction MassChanged;

        [Range(0, 3600), Tooltip("Frequence of search of Zone (in / out)")]
        public float FrequencyToFindZones = 0.1f;
        [Range(0, 3600), Tooltip("Frequence of calculation of closest point from attractors")]
        public float FrequencyToSearchClosestPoints = 0.1f;

        //jetPack
        public float RadiusJetPack = 3f;
        public float JetPackForce = 1f;

        //max speed
        public float MaxSpeed = 10f;

        private void OnMassChanged()
        {
            Mass = Mass.ClampMass();
            MassChanged.Invoke();
        }
    }
}