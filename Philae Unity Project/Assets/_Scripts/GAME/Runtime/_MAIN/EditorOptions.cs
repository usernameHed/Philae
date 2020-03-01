using hedCommon.singletons;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOptions : SingletonMono<EditorOptions>
{
    [Tooltip("Show Zone where gravity apply")]
    public bool ShowZones = true;
    [Tooltip("When true, we can edit gravity override of attractors")]
    public bool ShowGravityOverride = true;
    [Tooltip("Color of inactive Zone")]
    public Color ColorZonesInactive = Color.gray;
    [Tooltip("Color of Zone when no graviton is inside it")]
    public Color ColorWhenNothinngInside = Color.red;
    [Tooltip("Color of Zone when a graviton is inside it")]
    public Color ColorWhenSomethingInside = Color.green;
    [Tooltip("When true, simulate the physics in edit mode")]
    public bool SimulatePhysics = false;

    [Tooltip("When true, we can edit lines position of sphape")]
    public bool SetupLinesOfSphape = false;
    [Tooltip("Size of Points displayed by some editors")]
    public float SizeLinesPoints = 0.5f;

    [Tooltip("Snap value for handles")]
    public float Snap = 0;
}
