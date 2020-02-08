using hedCommon.singletons;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOptions : SingletonMono<EditorOptions>
{
    [ShowInInspector]
    public static bool ShowZones = true;
    [ShowInInspector]
    public static Color ColorZonesInactive = Color.gray;
    [ShowInInspector]
    public static Color ColorWhenNothinngInside = Color.red;
    [ShowInInspector]
    public static Color ColorWhenSomethingInside = Color.green;
    [ShowInInspector]
    public static bool SimulatePhysics = false;
}
