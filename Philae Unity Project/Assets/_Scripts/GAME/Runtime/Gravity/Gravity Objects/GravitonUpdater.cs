using hedCommon.editorGlobal;
using hedCommon.extension.runtime;
using philae.gravity.zones;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.graviton
{
    public class GravitonUpdater : IUpdater
    {
        public override void CustomLoop()
        {
#if UNITY_EDITOR
            GravitonLister.Instance.Gravitons = ExtList.CleanNullFromList(GravitonLister.Instance.Gravitons, out bool hasChanged);
#endif


            for (int i = 0; i < GravitonLister.Instance.Gravitons.Count; i++)
            {
                if (GravitonLister.Instance.Gravitons[i] == null)
                {
                    continue;
                }
                if (!GravitonLister.Instance.Gravitons[i].IsInitialized())
                {
                    GravitonLister.Instance.Gravitons[i].Init(GravitonLister.Instance.ZonesLister);
                }
                if (!GravitonLister.Instance.Gravitons[i].gameObject.activeInHierarchy)
                {
                    continue;
                }
                GravitonLister.Instance.Gravitons[i].CustomPhysicLoop();
            }
        }
    }
}