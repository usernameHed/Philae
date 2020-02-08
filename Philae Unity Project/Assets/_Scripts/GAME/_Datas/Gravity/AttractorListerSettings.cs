using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.data.gravity
{
    [CreateAssetMenu(fileName = "AttractorListerSettings", menuName = "Philae/Gravity/AttractorListerSettings")]
    public class AttractorListerSettings : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        public bool ShowForce = true;
#endif
    }

    [Serializable]
    public class AttractorListerSettingsLocal
    {

    }
}