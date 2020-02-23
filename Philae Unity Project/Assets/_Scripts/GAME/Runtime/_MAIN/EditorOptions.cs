using hedCommon.singletons;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOptions : SingletonMono<EditorOptions>
{
    public bool ShowZones = true;
    public bool ShowGravityOverride = true;
    public Color ColorZonesInactive = Color.gray;
    public Color ColorWhenNothinngInside = Color.red;
    public Color ColorWhenSomethingInside = Color.green;
    public bool SimulatePhysics = false;
}
