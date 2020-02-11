using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor.gravityOverride
{
    [Serializable]
    public struct GravityOverrideDisc
    {
        public bool Face;
        public bool Borders;
    }

    [Serializable]
    public struct GravityOverrideCylinder
    {
        public GravityOverrideDisc Disc1;
        public GravityOverrideDisc Disc2;

        public GravityOverrideCylinder(GravityOverrideDisc disc1, GravityOverrideDisc disc2)
        {
            Disc1 = disc1;
            Disc2 = disc2;
        }
    }
}
