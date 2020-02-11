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
        public bool Trunk;

        public GravityOverrideCylinder(GravityOverrideDisc disc1, GravityOverrideDisc disc2, bool trunk)
        {
            Disc1 = disc1;
            Disc2 = disc2;
            Trunk = trunk;
        }
    }
}
