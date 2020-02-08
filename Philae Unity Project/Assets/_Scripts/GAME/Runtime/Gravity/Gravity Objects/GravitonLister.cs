using hedCommon.editorGlobal;
using hedCommon.extension.runtime;
using hedCommon.singletons;
using philae.gravity.zones;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.graviton
{
    public class GravitonLister : SingletonMono<GravitonLister>
    {
        [FoldoutGroup("Debug"), SerializeField, ReadOnly]
        private ZonesLister _zoneLister;
        public ZonesLister ZonesLister { get { return (_zoneLister); } }

        [ReadOnly]
        public List<Graviton> Gravitons = new List<Graviton>();


        public void AddGraviton(Graviton graviton)
        {
            if (Gravitons.AddIfNotContain(graviton))
            {
                graviton.Init(_zoneLister);
            }
        }

        public void RemoveGraviton(Graviton graviton)
        {
            Gravitons.Remove(graviton);
        }

        public void Init(ZonesLister zoneLister)
        {
            Debug.Log("init gravition lister");
            _zoneLister = zoneLister;
            for (int i = 0; i < Gravitons.Count; i++)
            {
                if (Gravitons[i] == null)
                {
                    continue;
                }
                Gravitons[i].Init(_zoneLister);
            }
        }

        public void ClearGravitonZone()
        {
            for (int i = 0; i < Gravitons.Count; i++)
            {
                if (Gravitons[i])
                {
                    Gravitons[i].ClearGravitonZone();
                }
            }
        }
    }
}