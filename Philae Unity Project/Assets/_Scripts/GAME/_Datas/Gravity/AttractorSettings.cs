using hedCommon.extension.runtime;
using hedCommon.scriptableObject;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static philae.gravity.physicsBody.PhysicBody;

namespace philae.data.gravity
{
    [CreateAssetMenu(fileName = "AttractorSettings", menuName = "Philae/Gravity/AttractorSettings")]
    public class AttractorSettings : ScriptableObject
    {
        public bool IsMovable = true;
        public bool ShowRange = true;
        public bool ShowRangeOnSelect = true;
        public ConstrainPosition ConstrainPositions;
    }

    [Serializable]
    public class AttractorSettingsLocal
    {
        [OnValueChanged("OnGravityChanged")]
        public float Gravity = 9.81f;

        [OnValueChanged("OnAddMinRangeChanged")]
        public bool AddMinRange = false;
        [OnValueChanged("OnAddMaxRangeChanged")]
        public bool AddMaxRange = false;

        [OnValueChanged("OnMinRangeChanged"), Range(0, 10), ShowIf("AddMinRange")]
        public float MinRange = 0f;
        [OnValueChanged("OnMaxRangeChanged"), Range(0, 10), ShowIf("AddMaxRange")]
        public float MaxRange = 0f;
        [OnValueChanged("OnKinematicChange")]
        public bool IsKinematic = true;
       
        public UnityAction GravityChanged;
        public UnityAction MinMaxRangeChanged;
        public UnityAction OnKinematicChanged;

        private void OnGravityChanged()
        {
            if (GravityChanged != null)
            {
                GravityChanged.Invoke();
            }
        }

        private void OnKinematicChange()
        {
            if (OnKinematicChanged != null)
            {
                OnKinematicChanged.Invoke();
            }
        }

        private void OnAddMinRangeChanged()
        {
            if (!AddMinRange)
            {
                MinRange = 0f;
                if (!AddMaxRange)
                {
                    MaxRange = 0f;
                }
            }
            if (MinMaxRangeChanged != null)
            {
                MinMaxRangeChanged.Invoke();
            }
        }

        private void OnAddMaxRangeChanged()
        {
            if (!AddMaxRange)
            {
                MaxRange = MinRange;
            }
            else
            {
                if (MaxRange == MinRange)
                {
                    MaxRange = MinRange + 5;
                }
            }
            MinMaxRangeChanged.Invoke();
        }

        private void OnMaxRangeChanged()
        {
            if (MaxRange < MinRange)
            {
                MaxRange = MinRange + 0.01f;
            }

            MinMaxRangeChanged.Invoke();
        }

        private void OnMinRangeChanged()
        {
            if (!AddMaxRange)
            {
                MaxRange = MinRange;
            }
            else if (MinRange > MaxRange)
            {
                MaxRange = ExtMathf.SetBetween(MaxRange, MinRange, 100);
            }
            MinMaxRangeChanged.Invoke();
        }
    }

}